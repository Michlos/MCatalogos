﻿using DomainLayer.Models.Catalogos;
using DomainLayer.Models.CommonModels.Enums;
using DomainLayer.Models.Distribuidor;
using DomainLayer.Models.PedidosVendedoras;
using DomainLayer.Models.Produtos;
using DomainLayer.Models.Tamanho;
using DomainLayer.Models.Vendedora;

using InfrastructureLayer;
using InfrastructureLayer.DataAccess.Repositories.Specific.Catalogo;
using InfrastructureLayer.DataAccess.Repositories.Specific.Distribuidor;
using InfrastructureLayer.DataAccess.Repositories.Specific.PedidoVendedora;
using InfrastructureLayer.DataAccess.Repositories.Specific.Produto;
using InfrastructureLayer.DataAccess.Repositories.Specific.Tamanho;
using InfrastructureLayer.DataAccess.Repositories.Specific.Vendedora;

using MCatalogos.Views.FormViews.Produtos;

using ServiceLayer.CommonServices;
using ServiceLayer.Services.CatalogoServices;
using ServiceLayer.Services.DetalhePedidoServices;
using ServiceLayer.Services.DistribuidorServices;
using ServiceLayer.Services.PedidosVendedorasServices;
using ServiceLayer.Services.ProdutoServices;
using ServiceLayer.Services.RotaServices;
using ServiceLayer.Services.TamanhoServices;
using ServiceLayer.Services.VendedoraServices;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCatalogos.Views.FormViews.PedidoVendedora
{
    public partial class PedidoAddForm : Form
    {

        GetProductValue GetProductValue = new GetProductValue();
        PedidosListForm PedidoListForm;
        PedidosVendedorasModel PedidoModel = new PedidosVendedorasModel();
        public VendedoraModel VendedoraModel = new VendedoraModel();
        private DistribuidorModel distribuidorModel = new DistribuidorModel();
        private CampanhaModel SelectedCampanha = null;
        private CatalogoModel SelectedCatalogo = null;

        private ProdutoModel ProdutoModelToAddGrid = null;
        private DetalhePedidoModel ItemPedidoModel = new DetalhePedidoModel();

        private RotaModel rota = new RotaModel();
        private RotaLetraModel rotaLetra = new RotaLetraModel();

        private List<TamanhosModel> ListTamanhos = new List<TamanhosModel>();
        private List<CatalogoModel> ListCatalogos = new List<CatalogoModel>();
        private List<CampanhaModel> ListCampanhas = new List<CampanhaModel>();
        private List<PedidosVendedorasModel> ListPedidos = new List<PedidosVendedorasModel>();
        private List<DetalhePedidoModel> ListItemsDetalhe = new List<DetalhePedidoModel>();

        private QueryStringServices _queryString;
        private TamanhoServices _tamanhoServices;
        private DetalhePedidoSerivces _detalheServices;
        private ProdutoServices _produtoServices;
        private PedidosVendedorasServices _pedidoServices;
        private VendedoraServices _vendedoraServices;
        private RotaLetraServices _rotaLetraServices;
        private RotaServices _rotaServices;
        private CatalogoServices _catalogoServices;
        private CampanhaServices _campanhaServices;
        private DistribuidorServices _distribuidorServices;
        private DataGridViewCheckBoxColumn Faltou;

        private RequestType RType;

        private static PedidoAddForm aForm = null;
        internal static PedidoAddForm Instance(VendedoraModel vendedoraModel, PedidosVendedorasModel pedidoModel, PedidosListForm pedidosListForm, RequestType requestType)
        {
            if (aForm == null)
            {
                aForm = new PedidoAddForm(vendedoraModel, pedidoModel, pedidosListForm, requestType);
            }

            return aForm;
        }

        public PedidoAddForm(VendedoraModel vendedoraModel, PedidosVendedorasModel pedidoModel, PedidosListForm pedidosListForm, RequestType requestType)
        {


            _queryString = new QueryStringServices(new QueryString());
            _tamanhoServices = new TamanhoServices(new TamanhoRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _detalheServices = new DetalhePedidoSerivces(new DetalhePedidoRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _produtoServices = new ProdutoServices(new ProdutoRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _pedidoServices = new PedidosVendedorasServices(new PedidoVendedoraRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _vendedoraServices = new VendedoraServices(new VendedoraRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _rotaLetraServices = new RotaLetraServices(new RotaLetraRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _rotaServices = new RotaServices(new RotaRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _catalogoServices = new CatalogoServices(new CatalogoRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _campanhaServices = new CampanhaServices(new CampanhaRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _distribuidorServices = new DistribuidorServices(new DistribuidorRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());

            InitializeComponent();
            VendedoraModel = vendedoraModel;
            PedidoModel = pedidoModel;
            PedidoListForm = pedidosListForm;
            ListPedidos = (List<PedidosVendedorasModel>)_pedidoServices.GetAll();
            distribuidorModel = _distribuidorServices.GetModel();
            RType = requestType;

        }



        private void PedidoAddForm_Load(object sender, EventArgs e)
        {
            LoadComboBoxCatalogos();

            if (RType == RequestType.Add)
            { //NOVO PEDIDO

                VendedoraModel = SelecionarVendedora();
                if (VendedoraModel == null)
                {
                    this.Close();
                }
                else if (VendedoraTemPedidoAberto(VendedoraModel))
                {
                    MessageBox.Show("A Vendedora selecionada já possui um pedido em aberto\nNão é pertido que a vendedora tenha mais de um pedido em Aberto.\nEdite o pedido ou finalize-o");
                    this.Close();
                }
                else
                {
                    Text = "Adicionando Novo Pedido - Vendedora: " + VendedoraModel.Nome;
                    SalvarPedido(VendedoraModel);
                    PreencheCampos(VendedoraModel, PedidoModel, RType);

                }
            }
            else if (RType == RequestType.Edit)
            { //EDIT PEDIDO
                PreencheCampos(VendedoraModel, PedidoModel, RType);
                LoadDetalhesPedido(PedidoModel, null);
                VerificaStatusAtual(PedidoModel);


            }
            else if (RType == RequestType.Confere)
            {
                PedidoModel.StatusPed = (int)StatusPedido.Conferido;
                _pedidoServices.SetStatus(PedidoModel.StatusPed, PedidoModel);
                VerificaStatusAtual(PedidoModel);
            }
            else if (RType == RequestType.Finaliza)
            {

                PedidoModel.StatusPed = (int)StatusPedido.Finalizado;
                _pedidoServices.SetStatus(PedidoModel.StatusPed, PedidoModel);
                PreencheCampos(VendedoraModel, PedidoModel, RType);
                LoadDetalhesPedido(PedidoModel, null);
                dgvDetalhePedido.ReadOnly = true;
                dgvDetalhePedido.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }

        private void VerificaStatusAtual(PedidosVendedorasModel pedidoModel)
        {
            StatusPedido statusAtual = (StatusPedido)pedidoModel.StatusPed;
            if (pedidoModel.StatusPed == ((int)StatusPedido.Finalizado))
            {

                MessageBox.Show($"Pedido \"{((StatusPedido)statusAtual).ToString()}\".\n Tela para simples conferência de informações\n", $"Status: {((StatusPedido)statusAtual).ToString()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                gbAddItem.Enabled = false;
                cbCatalogo.Enabled = false;
                cbCampanha.Enabled = false;
                btnDelete.Enabled = false;
                btnEdit.Enabled = false;
                dgvDetalhePedido.ReadOnly = true;
                dgvDetalhePedido.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            }
            else if (pedidoModel.StatusPed == ((int)StatusPedido.Conferido))
            {
                MessageBox.Show($"Pedido \"{((StatusPedido)statusAtual).ToString()}\".\n Você só é permitido editar as faltas.", $"Status: {((StatusPedido)statusAtual).ToString()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                gbAddItem.Enabled = false;
                cbCatalogo.Enabled = false;
                cbCampanha.Enabled = false;
                btnDelete.Enabled = false;
                btnEdit.Enabled = false;
                PreencheCampos(VendedoraModel, pedidoModel, RequestType.Confere);
                LoadDetalhesPedido(pedidoModel, null);

            }
        }

        private VendedoraModel SelecionarVendedora()
        {
            SelectVendedoraForm selectVendedoraForm = SelectVendedoraForm.Instance(this);
            selectVendedoraForm.WindowState = FormWindowState.Normal;
            selectVendedoraForm.ShowDialog();

            return selectVendedoraForm.SelectedVendedora;

        }

        private void LoadComboBoxCatalogos()
        {
            ListCatalogos = (List<CatalogoModel>)_catalogoServices.GetAll();

            cbCatalogo.ValueMember = "Nome";
            cbCatalogo.DisplayMember = "Nome";
            cbCatalogo.Items.Clear();
            cbCatalogo.Items.Add("== Todos ==");
            foreach (CatalogoModel model in ListCatalogos)
            {
                if (model.Ativo)
                    cbCatalogo.Items.Add(model);
            }
            cbCatalogo.SelectedIndex = 0;
        }
        private void cbCatalogo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCatalogo.SelectedIndex > 0)
            {

                SelectedCatalogo = cbCatalogo.SelectedItem as CatalogoModel;
                gbAddItem.Enabled = LoadComboBoxCampanhas(SelectedCatalogo);
                LoadDetalhesPedido(PedidoModel, SelectedCatalogo);



            }
            else
            {
                cbCampanha.DataSource = null;
                gbAddItem.Enabled = false;
                LoadDetalhesPedido(PedidoModel, null);




            }

        }
        private bool LoadComboBoxCampanhas(CatalogoModel catalogo)
        {
            ListCampanhas = (List<CampanhaModel>)_campanhaServices.GetAll();
            List<CampanhaModel> campanhasList = null;


            campanhasList = ListCampanhas.Where(cat => cat.CatalogoId == catalogo.CatalogoId).ToList();
            //ICollection<CampanhaModel> campanhasList = (ICollection < CampanhaModel > )ListCampanhas.Where(cat => cat.CatalogoId == catalogo.CatalogoId);

            //IList<CampanhaModel> campanhasList = ListCampanhas.Where(cat => cat.CatalogoId == catalogo.CatalogoId);
            if (campanhasList.Count != 0)
            {

                cbCampanha.DataSource = campanhasList;
                cbCampanha.DisplayMember = "Nome";
                cbCampanha.SelectedItem = campanhasList.Last();
                return true;
            }
            else
            {
                MessageBox.Show($"Catálogo sem CAMPANHA cadastrada,\nFavor cadastrar uma nova campanha para o catálogo \n\"{cbCatalogo.Text.ToUpper()}\"");
                cbCampanha.DataSource = null;
                //cbCampanha.SelectedIndex = -1;
                cbCampanha.Text = string.Empty;
                return false;
            }
        }

        private void PreencheCampos(VendedoraModel vendedora, PedidosVendedorasModel pedido, RequestType requestType)
        {
            rota = _rotaServices.GetByVendedoraId(vendedora.VendedoraId);
            rotaLetra = _rotaLetraServices.GetById(vendedora.RotaLetraId);
            mTextCpfVendedora.Text = vendedora.Cpf;
            textNomeVendedora.Text = vendedora.Nome;
            textRotaVendedora.Text = rotaLetra.RotaLetra + "-" + rota.Numero.ToString();

            if (requestType == RequestType.Add)
            {
                if (VendedoraTemPedidoAberto(vendedora))
                {
                    MessageBox.Show("A Vendedora selecionada já possui um pedido em aberto\nNão é pertido que a vendedora tenha mais de um pedido em Aberto.\nEdite o pedido ou finalize-o");
                    this.Close();
                }

            }

        }

        private void LoadDetalhesPedido(PedidosVendedorasModel pedidoModel, CatalogoModel catalogoModel)
        {
            IEnumerable<DetalhePedidoModel> itemsPedido = null;
            if (pedidoModel != null)
            {
                ListItemsDetalhe = (List<DetalhePedidoModel>)_detalheServices.GetAllByPedido(pedidoModel);



                if (catalogoModel != null)
                {
                    itemsPedido = ListItemsDetalhe.Where(p => p.PedidoId == pedidoModel.PedidoId).Where(c => c.CatalogoId == catalogoModel.CatalogoId);
                    CalculaTotais(catalogoModel);
                }
                else
                {
                    itemsPedido = ListItemsDetalhe.Where(p => p.PedidoId == PedidoModel.PedidoId);
                    CalculaTotais(null);
                }
            }

            DataTable tableDetalhes = ModelaTableGrid();
            DataRow row = ModelaRowTable(tableDetalhes, itemsPedido);

            dgvDetalhePedido.DataSource = tableDetalhes;
            ConfiguraDataGridView();




        }

        private DataRow ModelaRowTable(DataTable tableDetalhes, IEnumerable<DetalhePedidoModel> itemsPedido)
        {
            DataRow row = null;
            if (ListItemsDetalhe.Count != 0)
            {
                foreach (var model in itemsPedido)
                {
                    row = tableDetalhes.NewRow();
                    row["DetalheId"] = model.DetalheId;
                    row["PedidoId"] = model.PedidoId;
                    row["CatalogoId"] = model.CatalogoId;
                    row["ProdutoId"] = model.ProdutoId;
                    row["Catalogo"] = _catalogoServices.GetById(model.CatalogoId).Nome;
                    row["Referencia"] = model.Referencia;
                    row["Descricao"] = _produtoServices.GetById(model.ProdutoId).Descricao;
                    row["MargemVendedora"] = model.MargemVendedora;
                    row["MargemDistribuidor"] = model.MargemDistribuidor;
                    row["ValorProduto"] = double.Parse(model.ValorProduto.ToString()).ToString("C2");
                    row["Quantidade"] = model.Quantidade;
                    row["Tamanho"] = _tamanhoServices.GetById(model.TamanhoId).Tamanho;
                    row["ValorTotalItem"] = double.Parse(model.ValorTotalItem.ToString()).ToString("C2");
                    row["ValorLucroVendedoraItem"] = double.Parse(model.ValorLucroVendedoraItem.ToString()).ToString("C2");
                    row["ValorLucroDistribuidorItem"] = double.Parse(model.ValorLucroDistribuidorItem.ToString()).ToString("C2");
                    row["ValorPagarForencedorItem"] = double.Parse(model.ValorPagarFornecedorItem.ToString()).ToString("C2");
                    row["Faltou"] = model.Faltou;


                    tableDetalhes.Rows.Add(row);

                }

            }
            return row;
        }

        private DataTable ModelaTableGrid()
        {
            DataTable tableDetalhes = new DataTable();

            tableDetalhes.Columns.Add("DetalheId", typeof(int));
            tableDetalhes.Columns.Add("PedidoId", typeof(int));
            tableDetalhes.Columns.Add("CatalogoId", typeof(int));
            tableDetalhes.Columns.Add("ProdutoId", typeof(int));
            tableDetalhes.Columns.Add("Catalogo", typeof(string));
            tableDetalhes.Columns.Add("Referencia", typeof(string));
            tableDetalhes.Columns.Add("Descricao", typeof(string));
            tableDetalhes.Columns.Add("MargemVendedora", typeof(double));
            tableDetalhes.Columns.Add("MargemDistribuidor", typeof(double));
            tableDetalhes.Columns.Add("Quantidade", typeof(int));
            tableDetalhes.Columns.Add("Tamanho", typeof(string));
            tableDetalhes.Columns.Add("ValorProduto", typeof(string));
            tableDetalhes.Columns.Add("ValorTotalItem", typeof(string));
            tableDetalhes.Columns.Add("ValorLucroVendedoraItem", typeof(string));
            tableDetalhes.Columns.Add("ValorLucroDistribuidorItem", typeof(string));
            tableDetalhes.Columns.Add("ValorPagarForencedorItem", typeof(string));
            tableDetalhes.Columns.Add("Faltou", typeof(bool));

            return tableDetalhes;
        }

        private void ConfiguraDataGridView()
        {
            if (RType == RequestType.Confere)
            {
                dgvDetalhePedido.ReadOnly = false;
                dgvDetalhePedido.SelectionMode = DataGridViewSelectionMode.CellSelect;
            }
            else
            {
                dgvDetalhePedido.ReadOnly = true;
                dgvDetalhePedido.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }


            dgvDetalhePedido.ForeColor = Color.Black;

            dgvDetalhePedido.Columns["DetalheId"].Visible = false;

            dgvDetalhePedido.Columns["PedidoId"].Visible = false;

            dgvDetalhePedido.Columns["CatalogoId"].Visible = false;

            dgvDetalhePedido.Columns["ProdutoId"].Visible = false;

            dgvDetalhePedido.Columns["Catalogo"].Visible = cbCatalogo.SelectedIndex > 0 ? false : true;
            dgvDetalhePedido.Columns["Catalogo"].ReadOnly = true;
            dgvDetalhePedido.Columns["Catalogo"].Width = 50;

            dgvDetalhePedido.Columns["Referencia"].HeaderText = "Ref.";
            dgvDetalhePedido.Columns["Referencia"].Width = 50;
            dgvDetalhePedido.Columns["Referencia"].ReadOnly = true;

            dgvDetalhePedido.Columns["Descricao"].HeaderText = "Produto";
            dgvDetalhePedido.Columns["Descricao"].Width = 123;
            dgvDetalhePedido.Columns["Descricao"].ReadOnly = true;

            dgvDetalhePedido.Columns["MargemVendedora"].Visible = false;

            dgvDetalhePedido.Columns["MargemDistribuidor"].Visible = false;

            dgvDetalhePedido.Columns["ValorProduto"].HeaderText = "Vlr.Unit.";
            dgvDetalhePedido.Columns["ValorProduto"].Width = 50;
            dgvDetalhePedido.Columns["ValorProduto"].ReadOnly = true;

            dgvDetalhePedido.Columns["Quantidade"].HeaderText = "Qtd.";
            dgvDetalhePedido.Columns["Quantidade"].Width = 20;
            dgvDetalhePedido.Columns["Quantidade"].ReadOnly = true;

            dgvDetalhePedido.Columns["Tamanho"].HeaderText = "Tam.";
            dgvDetalhePedido.Columns["Tamanho"].Width = 25;
            dgvDetalhePedido.Columns["Tamanho"].ReadOnly = true;

            dgvDetalhePedido.Columns["ValorTotalItem"].HeaderText = "Vlr.Total";
            dgvDetalhePedido.Columns["ValorTotalItem"].Width = 50;
            dgvDetalhePedido.Columns["ValorTotalItem"].ReadOnly = true;

            dgvDetalhePedido.Columns["ValorLucroVendedoraItem"].HeaderText = "Lucro Vend.";
            dgvDetalhePedido.Columns["ValorLucroVendedoraItem"].Width = 50;
            dgvDetalhePedido.Columns["ValorLucroVendedoraItem"].ReadOnly = true;

            dgvDetalhePedido.Columns["ValorLucroDistribuidorItem"].HeaderText = "Lucro Dist.";
            dgvDetalhePedido.Columns["ValorLucroDistribuidorItem"].Width = 50;
            dgvDetalhePedido.Columns["ValorLucroDistribuidorItem"].ReadOnly = true;

            dgvDetalhePedido.Columns["ValorPagarForencedorItem"].HeaderText = "Custo";
            dgvDetalhePedido.Columns["ValorPagarForencedorItem"].Width = 50;
            dgvDetalhePedido.Columns["ValorPagarForencedorItem"].ReadOnly = true;

            dgvDetalhePedido.Columns["Faltou"].Visible = RType == RequestType.Confere;
            dgvDetalhePedido.Columns["Faltou"].Width = 50;
            dgvDetalhePedido.Columns["Faltou"].ReadOnly = false;

            for (int i = 0; i < dgvDetalhePedido.Rows.Count; i++)
            {
                bool faltou = bool.Parse(dgvDetalhePedido.Rows[i].Cells["Faltou"].Value.ToString());
                if (faltou)
                {
                    dgvDetalhePedido.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                }
            }






        }

        private void PedidoAddForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (PedidoModel == null)
            {


                if (!VendedoraTemPedidoAberto(VendedoraModel))
                {


                    SalvarPedido(VendedoraModel);
                }

            }
            else
            {
                AtualizaPedido(VendedoraModel, PedidoModel);
            }

            if (Disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(Disposing);
            aForm = null;

        }

        private void AtualizaPedido(VendedoraModel vendedoraModel, PedidosVendedorasModel pedidoModel)
        {
            pedidoModel.ValorLucroDistribuidor = ListItemsDetalhe.Where(faltou => !faltou.Faltou).Sum(valor => valor.ValorLucroDistribuidorItem);
            pedidoModel.ValorLucroVendedora = ListItemsDetalhe.Where(faltou => !faltou.Faltou).Sum(valor => valor.ValorLucroVendedoraItem);
            pedidoModel.ValorTotalPedido = ListItemsDetalhe.Where(faltou => !faltou.Faltou).Sum(valor => valor.ValorTotalItem);
            pedidoModel.QtdCatalogos = ListItemsDetalhe.Select(a => a.CatalogoId).Distinct().Count();
            _pedidoServices.AtualizaTotaisPedido(pedidoModel);

            PedidoListForm.AtualizaDGV();

        }

        private bool VendedoraTemPedidoAberto(VendedoraModel vendedora)
        {
            if (vendedora != null)
            {

                IEnumerable<PedidosVendedorasModel> PedidosCollection = ListPedidos.Where(ped => ped.VendedoraId == vendedora.VendedoraId);
                PedidosCollection = PedidosCollection.Where(ped => ped.StatusPed == ((int)StatusPedido.Aberto));

                if (PedidosCollection.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textReferencia_Leave(object sender, EventArgs e)
        {
            /*
            if (!string.IsNullOrEmpty(textReferencia.Text))
            {
                textReferencia.BackColor = SystemColors.Window;
                if (ProdutoExiste(textReferencia.Text))
                { //SE EXISTE ADICIONA VERIFICA SE TEM TAMANHO PARA DEPOIS ADI9CIONAR AO GRID.
                    ProdutoModelToAddGrid = _produtoServices.GetByReference(textReferencia.Text);

                    if (ProdutoModelToAddGrid.CatalogoId != SelectedCatalogo.CatalogoId)
                    {
                        MessageBox.Show("Produto não pertence ao catálogo selecionado.");
                        textReferencia.Focus();
                    }
                    else if (ProdutoModelToAddGrid.CampanhaId != SelectedCampanha.CampanhaId)
                    {
                        var result = MessageBox.Show($"Produto não pertence à campanha selecionada.\nDeseja Adicionar esse produto à campanha: \n{SelectedCampanha.Nome} ?", "Produto Fora da Campanha", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {//ADICIONAR O PRODUTO CADASTRADO NA CAMPANHA ATUAL
                            ProdutoModel produto = new ProdutoModel();
                            produto = _produtoServices.GetByReference(textReferencia.Text);
                            produto.CampanhaId = SelectedCampanha.CampanhaId;
                            if (produto.MargemVendedora != null)
                                this.ProdutoModelToAddGrid = _produtoServices.AddWithMargens(produto);
                            else
                                this.ProdutoModelToAddGrid = _produtoServices.AddNoMargens(produto);

                            result = MessageBox.Show($"O Produto {this.ProdutoModelToAddGrid.Referencia} foi adicionado à campanha {this.SelectedCampanha}.\n Deseja revisar o preço do produto agora?", "Adicionando Produto à Campanha Atual", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {   // ABRIR EDIÇÃO DE PRODUTO
                                ProdutoAddForm editaProduto = new ProdutoAddForm(ProdutoModelToAddGrid, SelectedCatalogo, SelectedCampanha, this);
                                editaProduto.StartPosition = FormStartPosition.CenterScreen;
                                editaProduto.ShowDialog();
                                this.ProdutoModelToAddGrid = editaProduto.ProdutoModel;
                            }
                            else
                            {
                                MessageBox.Show("Não se equeça de revisar o preço do produto e em seguida editar esse pedido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else if (result == DialogResult.No)
                        {
                            textReferencia.Focus();
                        }


                    }

                }
                else
                { //adicionar produto no catalogo.
                    ProdutoModelToAddGrid = AdicionarNovoProduto(textReferencia.Text);

                }


                if (ProdutoModelToAddGrid != null)
                {

                    ProdutoTemTamanho(ProdutoModelToAddGrid);
                }
                else
                {
                    LimpaAddPanel();
                }

            }
            else
            {
                textReferencia.BackColor = Color.Red;
            }
            */
        }

        private ProdutoModel AdicionarNovoProduto(string referencia)
        {
            ProdutoModel produtoAdded = new ProdutoModel();
            try
            {
                ProdutoAddForm produtoAddForm = new ProdutoAddForm(null, SelectedCatalogo, SelectedCampanha, this);
                produtoAddForm.Text = "Pedido - Adicionando Produto";
                produtoAddForm.StartPosition = FormStartPosition.CenterScreen;
                produtoAddForm.textReferencia.Text = referencia;
                produtoAddForm.ShowDialog();
                produtoAdded = produtoAddForm.ProdutoModel;

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Não foi possível adicionar um novo Produto à Campanha\nMessageError: {ex.Message}\nStackTrace: {ex.StackTrace}\nInnerException: {ex.InnerException}");
            }

            return produtoAdded;
        }



        private bool ProdutoExiste(string referencia)
        {
            ProdutoModel produto = new ProdutoModel();
            produto = _produtoServices.GetByReference(referencia);
            if (produto.ProdutoId != 0)
                return true;
            else
                return false;
        }

        private void ProdutoTemTamanho(ProdutoModel produtoToAddGrid)
        {
            List<TamanhosModel> tamanhos = null;
            tamanhos = (List<TamanhosModel>)_tamanhoServices.GetByProdutoModel(produtoToAddGrid);
            if (tamanhos.Count > 0)
            {//SE PRODUTO TEM TAMANHO HABILITA E CARREGA O COMBOBOX DE TAMANHOS
                ListTamanhos = tamanhos;
                cbTamanho.Enabled = true;
                LoadComboBoxTamanhos(ListTamanhos);
            }
            else
            {//SE NÃO TEM TAMANHO DESABILITA O COMBOBOX E LIMPA SEUS DADOS
                cbTamanho.DataSource = null;
                cbTamanho.Enabled = false;
                cbTamanho.Text = string.Empty;
            }
        }

        /// <summary>
        /// Saída do campo tamanho gera add no Grid automático.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textQtd_Leave(object sender, EventArgs e)
        {
            //verifica se o usuário digitou a referência do produto.
            if (string.IsNullOrEmpty(textReferencia.Text))
            {
                //se o usuário não digitou a referência ele simplesmente vai para o campo de referência.
                textReferencia.Focus();
            }
            else if (!string.IsNullOrEmpty(textQtd.Text)) //verifica se o usuário digitou algum valor no campo quantidade.
            {
                //tenta converter o valor digitado em quantidade em inteiro para checar o campo.
                if (int.TryParse(textQtd.Text, out int qtdParsed))
                {
                    //se valor inteiro retornado for diferente de zero ele continua.
                    if (qtdParsed != 0)
                    {
                        //se combobox tamanho não estiver habilitada quer dizer que o produto não tem tamanho.
                        //se produto não tem tamanho pode adicionar o produto no datagrid.
                        if (!cbTamanho.Enabled)
                        {
                            //chama o métido para adicionar produto no grid do pedido.
                            //valores passados para o método:
                            //- ProdutoModel.
                            //- Quantidade informada no resultado do tryparse int.
                            //- Tamanho do produto como nulo.
                            AddProdutoInDGV(ProdutoModelToAddGrid, qtdParsed, null);

                            //Depois de incluído o produto no grid limpa os dados informados no form dentro do GroupBox Item.
                            LimpaAddPanel();
                        }
                    }

                }
                else //se valor digitado no campo quantidade não for numérico lança um alerta e retorna para o campo quantidade.
                {
                    MessageBox.Show("Só é aceito valor numérico", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textQtd.Focus();
                }
            }
        }

        private void LoadComboBoxTamanhos(List<TamanhosModel> listTamanhos)
        {

            cbTamanho.DataSource = ListTamanhos;
            cbTamanho.DisplayMember = "Tamanho";
            cbTamanho.SelectedIndex = -1;


        }



        private void cbTamanho_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTamanho.SelectedIndex >= 0)
            {
                if (!string.IsNullOrEmpty(textQtd.Text))
                {

                    AddProdutoInDGV(ProdutoModelToAddGrid, int.Parse(textQtd.Text), (TamanhosModel)cbTamanho.SelectedItem);
                    LimpaAddPanel();
                }

            }
        }

        private void LimpaAddPanel()
        {
            cbTamanho.SelectedIndex = -1;
            cbTamanho.Text = string.Empty;
            textReferencia.Text = string.Empty;
            textQtd.Text = string.Empty;
            textReferencia.Focus();
        }

        private void SalvarPedido(VendedoraModel vendedora)
        {
            if (vendedora != null)
            {
                PedidoModel = new PedidosVendedorasModel();
                PedidoModel.VendedoraId = vendedora.VendedoraId;
                PedidoModel.StatusPed = (((int)StatusPedido.Aberto));
                PedidoModel.DataVencimento = DateTime.Parse($"{DateTime.Today.Year}-{DateTime.Today.Month}-{distribuidorModel.DiaVencimento}").AddMonths(1);
                PedidoModel = (PedidosVendedorasModel)_pedidoServices.Add(PedidoModel);

            }


        }

        /// <summary>
        /// Adiciona produto no pedido.
        /// </summary>
        /// <param name="produtoModelToAdd"></param>
        /// <param name="quantidade"></param>
        /// <param name="tamanho"></param>
        private void AddProdutoInDGV(ProdutoModel produtoModelToAdd, int quantidade, TamanhosModel tamanho)
        {
            DataGridViewRow ProdutoNoGrid = new DataGridViewRow();

            //valor do produto pode variar conforme o tamanho.
            //Produtos a partir de GG deve assumir outro valor.
            double valorProduto = GetProductValue.VerificaValorProduto(produtoModelToAdd, tamanho);

            //VERIFICA SE O PRODUTO EXISTE NO PEDIDO.
            ProdutoNoGrid = ProdutoJaEstaNoGrid(produtoModelToAdd.Referencia, tamanho);
            //ItemPedidoModel.Quantidade = quantidade;


            ///SE PRODUTO TEM TAMANHO ENTÃO BUSCA O TAMANHO ID INFORMADO
            ///SENÃO INFORMA TAMNHOID = 0
            ItemPedidoModel.TamanhoId = tamanho != null ? tamanho.TamanhoId : 0;

            if (SelectedCatalogo == null)
            {
                SelectedCatalogo = _catalogoServices.GetById(int.Parse(dgvDetalhePedido.CurrentRow.Cells["CatalogoId"].Value.ToString()));
            }

            if (SelectedCampanha == null)
            {
                SelectedCampanha = (CampanhaModel)_campanhaServices.GetByCatalogoId(SelectedCatalogo.CatalogoId).Last();
            }

            ///SE PRODUTO JÁ EXISTE NO PEDIDO ATUALIZA
            ///SENÃO ADICIONA O PRODUTO NO PEDIDO.
            if (ProdutoNoGrid != null)
            {
                //GERA O MODEL POR MEIO DO PRODUTO EXISTENTE NO PEDIDO
                ItemPedidoModel = _detalheServices.GetById(int.Parse(ProdutoNoGrid.Cells["DetalheId"].Value.ToString()));

                //ADICIONA A QUANTIDADE INFORMADA À QUANTIDADE EXISTENTE NO PEDIDO.
                ItemPedidoModel.Quantidade += quantidade;
                //SE TEM TAXA POR PRODUTO ADICIONA A TAXA SENÃO TAXA RECEBE 0
                ItemPedidoModel.ValorTaxaItem = SelectedCatalogo.TaxaProduto ? SelectedCatalogo.ValorTaxaProduto * ItemPedidoModel.Quantidade : 0; ///////VAI PARA O FINAL
                //Atualiza produto existente no pedido.
                _detalheServices.Update(ItemPedidoModel);

            }
            else
            {
                ItemPedidoModel.PedidoId = PedidoModel.PedidoId;
                ItemPedidoModel.CatalogoId = SelectedCatalogo.CatalogoId;
                ItemPedidoModel.CampanhaId = SelectedCampanha.CampanhaId;
                ItemPedidoModel.ProdutoId = produtoModelToAdd.ProdutoId;
                ItemPedidoModel.Referencia = produtoModelToAdd.Referencia;
                ItemPedidoModel.Descricao = produtoModelToAdd.Descricao;
                ItemPedidoModel.Quantidade = quantidade;
                ItemPedidoModel.ValorTaxaItem = SelectedCatalogo.TaxaProduto ? SelectedCatalogo.ValorTaxaProduto * ItemPedidoModel.Quantidade : 0; ///////VAI PARA O FINAL
                ItemPedidoModel.MargemVendedora = string.IsNullOrEmpty(produtoModelToAdd.MargemVendedora) ? SelectedCatalogo.MargemPadraoVendedora : double.Parse(produtoModelToAdd.MargemVendedora);
                ItemPedidoModel.MargemDistribuidor = string.IsNullOrEmpty(produtoModelToAdd.MargemDistribuidor) ? SelectedCatalogo.MargemPadraoDistribuidor : double.Parse(produtoModelToAdd.MargemDistribuidor);
                ItemPedidoModel.ValorProduto = valorProduto;
                ItemPedidoModel.Faltou = false;

                if (ItemPedidoModel.TamanhoId == 0)
                {
                    _detalheServices.AddNoTamanho(ItemPedidoModel);
                }
                else
                {
                    _detalheServices.AddWithTamanho(ItemPedidoModel);
                }



            }

            LoadDetalhesPedido(PedidoModel, SelectedCatalogo);
            LimpaAddPanel();
        }

        /// <summary>
        /// VERIFICA SE O PRODUTO JÁ EXISTE NO PEDIDO.
        /// PARA POSSIBILITAR ADICIONAR QUANTIDADE A UM ITEM SELECIONADO.
        /// </summary>
        /// <param name="referencia"></param>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        private DataGridViewRow ProdutoJaEstaNoGrid(string referencia, TamanhosModel tamanho)
        {
            DataGridViewRow row = null;
            DetalhePedidoModel itemDetalheModel = new DetalhePedidoModel();


            foreach (DataGridViewRow item in dgvDetalhePedido.Rows)
            {
                itemDetalheModel = _detalheServices.GetById(int.Parse(item.Cells["DetalheId"].Value.ToString()));
                if (item.Cells["Referencia"].Value.ToString() == referencia)
                {
                    if (tamanho != null)
                    {
                        if (tamanho.Tamanho == item.Cells["Tamanho"].Value.ToString())
                        {
                            if (itemDetalheModel.Faltou)
                            {
                                row = null;
                            }
                            else
                            {
                                return item;

                            }

                        }
                        else
                        {
                            row = null;
                        }
                    }
                    else
                    {
                        if (itemDetalheModel.Faltou)
                        {
                            row = null;
                        }
                        else
                        {
                            return item;

                        }
                    }
                }

            }
            return row;



        }

        /// <summary>
        /// SALVA A CAMPANHA SELECIONADA NA MEMÓRIA PARA MANIPULAÇÃO DO PEDIDO/PRODUTO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCampanha_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCampanha.SelectedIndex > -1)
            {
                SelectedCampanha = cbCampanha.SelectedItem as CampanhaModel;
            }
        }

        private void dgvDetalhePedido_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            if (PedidoModel.StatusPed == (((int)StatusPedido.Aberto)))
            {
                EditarItemPedido(dgvDetalhePedido.CurrentRow);

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"Tem certeza que deseja exlcuir o item \"{dgvDetalhePedido.CurrentRow.Cells["Referencia"].Value.ToString()}\"", "Apagando Item do Pedido", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DetalhePedidoModel detalheToDelete = new DetalhePedidoModel();
                detalheToDelete = _detalheServices.GetById(int.Parse(dgvDetalhePedido.CurrentRow.Cells[0].Value.ToString()));
                _detalheServices.Delete(detalheToDelete);
                LoadDetalhesPedido(PedidoModel, SelectedCatalogo);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditarItemPedido(dgvDetalhePedido.CurrentRow);
        }

        private void EditarItemPedido(DataGridViewRow currentRow)
        {

            DetalhePedidoModel itemPedido = _detalheServices.GetById(int.Parse(currentRow.Cells[0].Value.ToString()));
            ItemPedidoEditForm itemPedidoEdit = new ItemPedidoEditForm(itemPedido);
            itemPedidoEdit.Text = $"Editando Item - {itemPedido.Referencia}";
            itemPedidoEdit.StartPosition = FormStartPosition.CenterScreen;
            itemPedidoEdit.ShowDialog();
            CalculaTotais(null);
            LoadDetalhesPedido(PedidoModel, SelectedCatalogo);
            dgvDetalhePedido.CurrentRow.Selected = currentRow.Selected;
        }

        private void CalculaTotais(CatalogoModel catalogo)
        {
            double totalLucroVendedora = 0;
            double totalLucroDistribuidor = 0;
            double totalReceber = 0;
            double totalPedido = 0;

            double taxaPorProduto = 0;
            double taxaPorPedido = 0;



            if (catalogo != null)
            {

                taxaPorPedido = catalogo.TaxaPedido ? catalogo.ValorTaxaPedido : 0;

                taxaPorProduto = catalogo.TaxaProduto ? catalogo.ValorTaxaProduto : 0;
                totalLucroVendedora = ListItemsDetalhe.Where(c => c.CatalogoId == SelectedCatalogo.CatalogoId).Where(ped => !ped.Faltou).Sum(valor => valor.ValorLucroVendedoraItem);
                totalLucroDistribuidor = ListItemsDetalhe.Where(c => c.CatalogoId == SelectedCatalogo.CatalogoId).Where(ped => !ped.Faltou).Sum(valor => valor.ValorLucroDistribuidorItem);
                totalReceber = ListItemsDetalhe.Where(c => c.CatalogoId == SelectedCatalogo.CatalogoId).Where(ped => !ped.Faltou).Sum(valor => valor.ValorTotalItem) - totalLucroVendedora;
                totalPedido = ListItemsDetalhe.Where(c => c.CatalogoId == SelectedCatalogo.CatalogoId).Where(ped => !ped.Faltou).Sum(valor => valor.ValorTotalItem);

                taxaPorPedido = totalPedido * (taxaPorPedido / 100);

                if (taxaPorPedido > 0)
                {
                    totalReceber += taxaPorPedido;
                }



            }
            else
            {
                totalLucroVendedora = ListItemsDetalhe.Where(ped => !ped.Faltou).Sum(valor => valor.ValorLucroVendedoraItem);
                totalLucroDistribuidor = ListItemsDetalhe.Where(ped => !ped.Faltou).Sum(valor => valor.ValorLucroDistribuidorItem);
                totalReceber = ListItemsDetalhe.Where(ped => !ped.Faltou).Sum(valor => valor.ValorTotalItem) - totalLucroVendedora;
                totalPedido = ListItemsDetalhe.Where(ped => !ped.Faltou).Sum(valor => valor.ValorTotalItem);
            }


            textLucroVendedora.Text = totalLucroVendedora.ToString("C2");
            textLucroDistribuidor.Text = totalLucroDistribuidor.ToString("C2");
            textTotalReceber.Text = totalReceber.ToString("C2");
            textTotalPedido.Text = totalPedido.ToString("C2");




        }

        private void dgvDetalhePedido_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            //TODO: CHECAR FALTA DE QUANTIDADE MENOR DO QUE A SOLICITADA.
            ///SISTEMA DEVE PERMITIR PRODUTO COM MESMA REFERÊNCIA DESDE QUE ALGUM TENHA FALTA.
            ///TENHO QUE PERMITIR PEDIR 2 FALTAR 1.
            DetalhePedidoModel itemPedido = _detalheServices.GetById(int.Parse(dgvDetalhePedido.Rows[e.RowIndex].Cells[0].Value.ToString()));
            int qtdAtual = itemPedido.Quantidade;
            int qtdItemFalta = 0;
            int qtdRestante = 0;
            ProdutoModel produtoFaltante = new ProdutoModel();
            TamanhosModel tamanhoProdutoFaltante = new TamanhosModel();

            if (e.ColumnIndex == dgvDetalhePedido.Columns["Faltou"].Index)
            {


                var resultDialog = MessageBox.Show("Faltaram Todos?", "Tirando Falta", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (resultDialog == DialogResult.No)
                {
                    QuantidadeItemFaltaForm qtdFalta = new QuantidadeItemFaltaForm(qtdAtual);
                    qtdFalta.ShowDialog();
                    qtdItemFalta = qtdFalta.valorDigitado;
                    if (qtdItemFalta == qtdAtual)
                    {
                        TirarFaltaItem(itemPedido, qtdAtual, e);
                    }
                    else
                    {
                        //altera a quantidade do item selecionado para a quantidade de falta.
                        itemPedido.Quantidade = qtdItemFalta;
                        //tirar falta do item
                        TirarFaltaItem(itemPedido, qtdItemFalta, e);
                        //adicionar produto com a quantidade restante.
                        qtdRestante = qtdAtual - qtdItemFalta;
                        produtoFaltante = _produtoServices.GetById(itemPedido.ProdutoId);
                        tamanhoProdutoFaltante = itemPedido.TamanhoId != 0 ? _tamanhoServices.GetById(itemPedido.TamanhoId) : null;
                        AddProdutoInDGV(produtoFaltante, qtdRestante, tamanhoProdutoFaltante);
                        CalculaTotais(null);
                        AtualizaPedido(VendedoraModel, PedidoModel);

                    }
                }
                else if (resultDialog == DialogResult.Yes)
                {
                    ///FALTOU TUDO.
                    TirarFaltaItem(itemPedido, qtdAtual, e);

                }
                else
                {

                    LoadDetalhesPedido(PedidoModel, SelectedCatalogo);

                }
            }




        }


        private void TirarFaltaItem(DetalhePedidoModel itemPedido, int qtdIem, DataGridViewCellEventArgs cellRowIndex)
        {
            dgvDetalhePedido.EndEdit();
            itemPedido.Faltou = bool.Parse(dgvDetalhePedido.Rows[cellRowIndex.RowIndex].Cells["Faltou"].Value.ToString());
            _detalheServices.Update(itemPedido);
            CalculaTotais(null);
            AtualizaPedido(VendedoraModel, PedidoModel);
        }

        private void textQtd_Enter(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textReferencia.Text))
            {
                textReferencia.BackColor = SystemColors.Window;
                if (ProdutoExiste(textReferencia.Text))
                { //SE EXISTE ADICIONA VERIFICA SE TEM TAMANHO PARA DEPOIS ADI9CIONAR AO GRID.
                    ProdutoModelToAddGrid = _produtoServices.GetByReference(textReferencia.Text);

                    if (ProdutoModelToAddGrid.CatalogoId != SelectedCatalogo.CatalogoId)
                    {
                        MessageBox.Show("Produto não pertence ao catálogo selecionado.");
                        textReferencia.Focus();
                    }
                    else if (ProdutoModelToAddGrid.CampanhaId != SelectedCampanha.CampanhaId)
                    {
                        var result = MessageBox.Show($"Produto não pertence à campanha selecionada.\nDeseja Adicionar esse produto à campanha: \n{SelectedCampanha.Nome} ?", "Produto Fora da Campanha", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {//ADICIONAR O PRODUTO CADASTRADO NA CAMPANHA ATUAL
                            ProdutoModel produto = new ProdutoModel();
                            produto = _produtoServices.GetByReference(textReferencia.Text);
                            produto.CampanhaId = SelectedCampanha.CampanhaId;
                            if (produto.MargemVendedora != null)
                                this.ProdutoModelToAddGrid = _produtoServices.AddWithMargens(produto);
                            else
                                this.ProdutoModelToAddGrid = _produtoServices.AddNoMargens(produto);

                            result = MessageBox.Show($"O Produto {this.ProdutoModelToAddGrid.Referencia} foi adicionado à campanha {this.SelectedCampanha}.\n Deseja revisar o preço do produto agora?", "Adicionando Produto à Campanha Atual", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {   // ABRIR EDIÇÃO DE PRODUTO
                                ProdutoAddForm editaProduto = new ProdutoAddForm(ProdutoModelToAddGrid, SelectedCatalogo, SelectedCampanha, this);
                                editaProduto.StartPosition = FormStartPosition.CenterScreen;
                                editaProduto.ShowDialog();
                                this.ProdutoModelToAddGrid = editaProduto.ProdutoModel;
                            }
                            else
                            {
                                MessageBox.Show("Não se equeça de revisar o preço do produto e em seguida editar esse pedido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else if (result == DialogResult.No)
                        {
                            textReferencia.Focus();
                        }


                    }

                }
                else
                { //adicionar produto no catalogo.
                    ProdutoModelToAddGrid = AdicionarNovoProduto(textReferencia.Text);

                }


                if (ProdutoModelToAddGrid != null)
                {

                    ProdutoTemTamanho(ProdutoModelToAddGrid);
                }
                else
                {
                    LimpaAddPanel();
                }

            }
            else
            {
                textReferencia.BackColor = Color.Red;
            }
        }
    }
}
