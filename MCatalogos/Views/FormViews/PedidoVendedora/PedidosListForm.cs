﻿using DomainLayer.Models.CommonModels.Enums;
using DomainLayer.Models.Modulos;
using DomainLayer.Models.PedidosVendedoras;
using DomainLayer.Models.TitulosReceber;
using DomainLayer.Models.Vendedora;

using InfrastructureLayer;
using InfrastructureLayer.DataAccess.Repositories.Commons;
using InfrastructureLayer.DataAccess.Repositories.Specific.Modulos;
using InfrastructureLayer.DataAccess.Repositories.Specific.PedidoVendedora;
using InfrastructureLayer.DataAccess.Repositories.Specific.TituloReceber;
using InfrastructureLayer.DataAccess.Repositories.Specific.Vendedora;

using ServiceLayer.CommonServices;
using ServiceLayer.Services.DetalhePedidoServices;
using ServiceLayer.Services.HistoricoTituloReceberServices;
using ServiceLayer.Services.ModulosServices;
using ServiceLayer.Services.PedidosVendedorasServices;
using ServiceLayer.Services.TipoTituloServices;
using ServiceLayer.Services.TitulosReceberServices;
using ServiceLayer.Services.VendedoraServices;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MCatalogos.Views.FormViews.PedidoVendedora
{
    public partial class PedidosListForm : Form
    {
        enum PedidoReceberHistorico
        {
            Novo,
            Update
        }

        #region PROPRIEDADES PARA MOVER A JANELA

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        #endregion

        MainView MainView;
        private List<PedidosVendedorasModel> PedidosList = new List<PedidosVendedorasModel>();
        private List<VendedoraModel> VendedorasList = new List<VendedoraModel>();
        private List<TipoTituloModel> TiposTitulosList = new List<TipoTituloModel>();
        private List<TituloReceberModel> TitulosReceberList = new List<TituloReceberModel>();
        private IEnumerable<DetalhePedidoModel> DetalhePedidoList = new List<DetalhePedidoModel>();
        private IEnumerable<ModulosModel> ModulosList = new List<ModulosModel>();


        private VendedoraModel VendedoraFilter = new VendedoraModel();
        private PedidosVendedorasModel SelectedPedido = new PedidosVendedorasModel();
        private VendedoraModel SelectedVendedora = new VendedoraModel();
        private TipoTituloModel TipoTituloModel = new TipoTituloModel();
        private TituloReceberModel TituloReceberModel = new TituloReceberModel();
        private ModulosModel ModuloFinanceiro = new ModulosModel();


        private QueryStringServices _queryString;
        private PedidosVendedorasServices _pedidosServices;
        private VendedoraServices _vendedoraServices;
        private TituloReceberServices _tituloReceberServices;
        private TipoTituloServices _tipoTituloServices;
        private HistoricoTituloReceberServices _historicoTituloReceberServices;
        private DetalhePedidoSerivces _detalhePedidoSerivces;
        private ModulosSerivces _modulosSerivces;

        private IEnumerable<PedidosVendedorasModel> pedidoVendedoraDGV;

        private StatusPedido status = StatusPedido.Todos;
        private int indexDGV = 0;

        public PedidosListForm(MainView mainView)
        {
            _queryString = new QueryStringServices(new QueryString());
            _pedidosServices = new PedidosVendedorasServices(new PedidoVendedoraRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _vendedoraServices = new VendedoraServices(new VendedoraRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _tituloReceberServices = new TituloReceberServices(new TituloReceberRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _tipoTituloServices = new TipoTituloServices(new TipoTituloRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _historicoTituloReceberServices = new HistoricoTituloReceberServices(new HistoricoTituloReceberRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _detalhePedidoSerivces = new DetalhePedidoSerivces(new DetalhePedidoRepository(_queryString.GetQueryApp()), new ModelDataAnnotationCheck());
            _modulosSerivces = new ModulosSerivces(new ModulosRepository(_queryString.GetQueryApp()));

            InitializeComponent();
            MainView = mainView;
            ModulosList = (List<ModulosModel>)_modulosSerivces.GetAll();
            ModuloFinanceiro = ModulosList.Where(mod => mod.Nome == "Financeiro").First();
        }
        private void PedidosListForm_Load(object sender, EventArgs e)
        {
            DefineDataInicial();
            DefineDataFinal();
            ReadModels();
            LoadComboBoxVendedoras();
            LoadDataGridView();


        }

        private void LoadDataGridView()
        {
            VendedoraModel vendedora = cbNomeVendedora.SelectedIndex != -1 ? cbNomeVendedora.SelectedItem as VendedoraModel : null;

            DateTime? dataRegistroIni = ReceiveDate(dateDataInicio.Text.ToString());
            DateTime? dataRegistroFim = ReceiveDate(dateDataFim.Text.ToString());


            pedidoVendedoraDGV = PedidosList;

            if (vendedora != null)
            {
                pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.VendedoraId == vendedora.VendedoraId);

            }

            if (dataRegistroIni != null && dataRegistroFim != null)
            {
                pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.DataRegistro >= dataRegistroIni && pedido.DataRegistro <= dataRegistroFim);
            }

            pedidoVendedoraDGV = ConfiguraDgvByStatus(status, pedidoVendedoraDGV);

            DataTable tablePedidos = ModelaDataTablePedidos();
            DataRow row = ModelaDataRowTablePedido(tablePedidos, pedidoVendedoraDGV);
            ConfiguraDataGridDetalhePedidos(tablePedidos);


        }

        private IEnumerable<PedidosVendedorasModel> ConfiguraDgvByStatus(StatusPedido status, IEnumerable<PedidosVendedorasModel> pedidoVendedoraDGV)
        {
            switch (status)
            {
                case StatusPedido.Aberto:
                    pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.StatusPed == ((int)StatusPedido.Aberto));
                    break;
                case StatusPedido.Enviado:
                    pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.StatusPed == ((int)StatusPedido.Enviado));
                    break;
                case StatusPedido.Separado:
                    pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.StatusPed == ((int)StatusPedido.Separado));
                    break;
                case StatusPedido.Conferido:
                    pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.StatusPed == ((int)StatusPedido.Conferido));
                    break;
                case StatusPedido.Finalizado:
                    pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.StatusPed == ((int)StatusPedido.Finalizado));
                    break;
                case StatusPedido.Despachado:
                    pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.StatusPed == ((int)StatusPedido.Despachado));
                    break;
                case StatusPedido.Entregue:
                    pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.StatusPed == ((int)StatusPedido.Entregue));
                    break;
                case StatusPedido.Cancelado:
                    pedidoVendedoraDGV = pedidoVendedoraDGV.Where(pedido => pedido.StatusPed == ((int)StatusPedido.Cancelado));
                    break;
                default:

                    break;
            }

            return pedidoVendedoraDGV;
        }

        private void ConfiguraDataGridDetalhePedidos(DataTable tablePedidos)
        {
            dgvPedidos.DataSource = tablePedidos;

            dgvPedidos.Columns[0].HeaderText = "Código";
            dgvPedidos.Columns[0].Width = 80;
            dgvPedidos.Columns[1].HeaderText = "Vendedora";
            dgvPedidos.Columns[1].Width = 346;
            dgvPedidos.Columns[2].HeaderText = "Data Reg.";
            dgvPedidos.Columns[3].HeaderText = "Qtd.Cat.";
            dgvPedidos.Columns[3].Width = 60;
            dgvPedidos.Columns[4].HeaderText = "Val. Total";
            dgvPedidos.Columns[5].HeaderText = "Status";

            dgvPedidos.ForeColor = Color.Black;
        }

        private DataRow ModelaDataRowTablePedido(DataTable tablePedidos, IEnumerable<PedidosVendedorasModel> pedidoVendedoraDGV)
        {
            DataRow row = null;
            if (pedidoVendedoraDGV.Any())
            {
                foreach (PedidosVendedorasModel model in pedidoVendedoraDGV)
                {
                    row = tablePedidos.NewRow();
                    row["CodigoColumn"] = int.Parse(model.PedidoId.ToString());
                    row["VendedoraNomeColumn"] = _vendedoraServices.GetById(model.VendedoraId).Nome;
                    row["DataRegColumn"] = model.DataRegistro;
                    row["QtdCatalogoColumn"] = model.QtdCatalogos != null ? model.QtdCatalogos : 0;
                    row["ValorTotalColumn"] = model.ValorTotalPedido;
                    row["StatusColumn"] = GetStatusPedido(model.StatusPed);

                    tablePedidos.Rows.Add(row);

                }
            }

            return row;
        }

        private DataTable ModelaDataTablePedidos()
        {
            DataTable table = new DataTable();


            table.Columns.Add("CodigoColumn", typeof(int));
            table.Columns.Add("VendedoraNomeColumn", typeof(string));
            table.Columns.Add("DataRegColumn", typeof(DateTime));
            table.Columns.Add("QtdCatalogoColumn", typeof(int));
            table.Columns.Add("ValorTotalColumn", typeof(string));
            table.Columns.Add("StatusColumn", typeof(string));

            return table;
        }

        private object GetStatusPedido(int statusPed)
        {

            switch (statusPed)
            {
                case 0:
                    return StatusPedido.Aberto;
                case 1:
                    return StatusPedido.Enviado;
                case 2:
                    return StatusPedido.Separado;
                case 3:
                    return StatusPedido.Conferido;
                case 4:
                    return StatusPedido.Finalizado;
                case 5:
                    return StatusPedido.Despachado;
                case 6:
                    return StatusPedido.Entregue;
                case 7:
                    return StatusPedido.Cancelado;
                default:
                    return StatusPedido.Aberto;
            }
        }

        private DateTime? ReceiveDate(string date)
        {
            DateTime? dateReturn = null;
            if (date.Replace("/", "").Trim() != "")
            {
                dateReturn = DateTime.Parse(date);
            }

            return dateReturn;
        }

        private void LoadComboBoxVendedoras()
        {
            cbNomeVendedora.DataSource = VendedorasList;
            cbNomeVendedora.DisplayMember = "Nome";

            cbNomeVendedora.SelectedIndex = -1;
        }


        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private static PedidosListForm aForm = null;

        internal static PedidosListForm Instance(MainView mainView)
        {
            if (aForm == null)
            {
                aForm = new PedidosListForm(mainView);
            }
            else
            {
                aForm.BringToFront();
            }
            return aForm;
        }

        private void PedidosListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            //MainView.SetUnselectedButtons();
            base.Dispose(Disposing);
            aForm = null;


        }


        private void ReadModels()
        {
            VendedorasList = (List<VendedoraModel>)_vendedoraServices.GetAll();
            PedidosList = (List<PedidosVendedorasModel>)_pedidosServices.GetAll();
            TitulosReceberList = (List<TituloReceberModel>)_tituloReceberServices.GetAll();
            TiposTitulosList = (List<TipoTituloModel>)_tipoTituloServices.GetAll();

        }

        private void DefineDataFinal()
        {
            DateTime dataFim = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            dateDataFim.Text = dataFim.ToString();
        }

        private void DefineDataInicial()
        {
            DateTime dataIni = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            dateDataInicio.Text = dataIni.ToString();
        }



        #region COMBOX EVENTS
        private void cbNomeVendedora_Leave(object sender, EventArgs e)
        {
            if (cbNomeVendedora.Text == string.Empty)
            {
                mTextCpf.Text = string.Empty;
            }
            SetBgColorUnfocused(cbNomeVendedora);
        }
        private void cbNomeVendedora_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbNomeVendedora.SelectedIndex > -1)
            {
                VendedoraFilter = cbNomeVendedora.SelectedItem as VendedoraModel;
                mTextCpf.Text = VendedoraFilter.Cpf;
                LoadDataGridView();
            }
            else
            {
                mTextCpf.Text = string.Empty;
            }
        }

        #endregion




        #region BUTTONS EVENTS
        private void pictureClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnClearDate_Click(object sender, EventArgs e)
        {
            dateDataFim.Text = string.Empty;
            dateDataInicio.Text = string.Empty;
            dateDataInicio.Focus();
        }
        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            cbNomeVendedora.SelectedIndex = -1;
            UncheckRadiosButtons();
            AtualizaDGV();

        }
        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            LoadDataGridView();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            //SelectedPedido = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(dgvPedidos.CurrentRow.Cells[0].Value.ToString()));
            //EditarPedido(SelectedPedido, RequestType.Add);
            EditarPedido(null, RequestType.Add);
            AtualizaDGV();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SelectedPedido = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(dgvPedidos.CurrentRow.Cells[0].Value.ToString()));
            EditarPedido(SelectedPedido, RequestType.Edit);
            AtualizaDGV();
        }
        private void btnConferir_Click(object sender, EventArgs e)
        {

            SelectedPedido = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(dgvPedidos.CurrentRow.Cells[0].Value.ToString()));
            if (SelectedPedido.StatusPed == (int)StatusPedido.Aberto || SelectedPedido.StatusPed == (int)StatusPedido.Conferido)
            {
                EditarPedido(SelectedPedido, RequestType.Confere);
                AtualizaDGV();
            }
            else
            {
                MessageBox.Show("Somente pedidos em Aberto ou em Conferência podem ser conferidos", "Conferência de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            SelectedPedido = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(dgvPedidos.CurrentRow.Cells[0].Value.ToString()));
            if (rbTodos.Checked)
            { //SÓ LIBERA A OPÇÃO DE FINALIZAR TODOS SE ESTIVEREM TODOS LISTADOS

                var result = MessageBox.Show("Deseja finalizar todos os pedidos?", $"Finalizando Pedido - Vendedora: {dgvPedidos.CurrentRow.Cells[1].Value.ToString()}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    FinalizaTodosPedidos(dgvPedidos);
                else
                {
                    try
                    {
                        if (SelectedPedido.StatusPed == (int)StatusPedido.Finalizado)
                        {
                            throw new InvalidOperationException("já se encontra finalizado");
                        }
                        EditarPedido(SelectedPedido, RequestType.Finaliza);
                        MessageBox.Show($"Pedido {SelectedPedido.PedidoId} Finalizado com sucesso");
                    }
                    catch (InvalidOperationException eInvalid)
                    {
                        MessageBox.Show($"O Pedido {SelectedPedido.PedidoId} {eInvalid.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Não foi possível continuar o processo\n" + "Error Message:" + ex.Message + "\nStackTrace: " + ex.StackTrace);
                    }
                }
            }
            else
            {
                EditarPedido(SelectedPedido, RequestType.Finaliza);
                MessageBox.Show($"Pedido {SelectedPedido.PedidoId} Finalizado com sucesso");
            }

            AtualizaDGV();

        }

        private void FinalizaTodosPedidos(DataGridView dgvPedidos)
        {
            List<PedidosVendedorasModel> ListaPedidos = new List<PedidosVendedorasModel>();
            foreach (DataGridViewRow row in dgvPedidos.Rows)
            {//percorre DGV e coloca cada id na lista de pedidos.
                PedidosVendedorasModel model = new PedidosVendedorasModel();
                model = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(row.Cells[0].Value.ToString()));
                ListaPedidos.Add(model);
            }

            try
            {
                foreach (var pedido in ListaPedidos)
                {//finaliza a lista de pedidos
                    if (pedido.StatusPed != ((int)StatusPedido.Cancelado))
                    {
                        EditarPedido(pedido, RequestType.Finaliza);
                    }
                }

                MessageBox.Show($"Foram finalizados todos os pedidos.");
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível finalizar todos os pedidos.\nMessageError: " + e.Message, e.InnerException);
            }

        }



        #endregion

        private void EditarPedido(PedidosVendedorasModel pedido, RequestType requestType)
        {
            PedidoAddForm pedidoForm = null;

            switch (requestType)
            {
                case RequestType.Add:
                    pedidoForm = PedidoAddForm.Instance(null, null, this, requestType);
                    pedidoForm.Show();
                    break;
                case RequestType.Edit:
                    SelectedVendedora = _vendedoraServices.GetById(pedido.VendedoraId);
                    pedidoForm = PedidoAddForm.Instance(SelectedVendedora, pedido, this, requestType);
                    pedidoForm.Text = $"Editando Pedido - Vendedora: {SelectedVendedora.Nome}";
                    pedidoForm.Show();
                    break;
                case RequestType.Remove:
                    _pedidosServices.Remove(pedido);
                    break;
                case RequestType.Liquidar:
                    break;
                case RequestType.Abater:
                    break;
                case RequestType.Confere:
                    SelectedVendedora = _vendedoraServices.GetById(pedido.VendedoraId);
                    if (pedido.StatusPed != ((int)StatusPedido.Finalizado))
                    {
                        pedidoForm = PedidoAddForm.Instance(SelectedVendedora, pedido, this, requestType);
                        pedidoForm.Text = $"Conferindo Pedido - Vendedora: {SelectedVendedora.Nome}";
                    }
                    else
                    {
                        pedidoForm = PedidoAddForm.Instance(SelectedVendedora, pedido, this, requestType);
                        pedidoForm.Text = $"Conferindo pedido - Vendedora: {SelectedVendedora.Nome}";
                    }
                    pedidoForm.Show();
                    break;
                case RequestType.Finaliza:
                    try
                    {
                        _pedidosServices.SetStatus(((int)StatusPedido.Finalizado), pedido);

                        ///VERIFICA SE MODULO FINANCEIRO ESTÁ TIVO
                        ///SE ESTIVER ATIVO EXECUTA OS MÉTODOS DO FINANCEIRO
                        ///
                        if (ModuloFinanceiro.Ativo)
                        {

                            if (TemContasReceber(pedido))
                            {
                                AtualizaContaReceber(pedido);
                                GeraHistoricoContasReceber(pedido, PedidoReceberHistorico.Update);
                            }
                            else
                            {
                                GeraContasReceber(pedido);
                                GeraHistoricoContasReceber(pedido, PedidoReceberHistorico.Novo);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show($"Não foi possível finalizar o pedido. {pedido.PedidoId} \nMessage: {e.Message}");
                    }
                    break;
                case RequestType.Cancela:
                    try
                    {
                        _pedidosServices.SetStatus(((int)StatusPedido.Cancelado), pedido);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Não foi possível cancelar o pedido {pedido.PedidoId}.\nMessage: {e.Message}");
                    }
                    break;
                default:
                    break;
            }
        }

        private void GeraHistoricoContasReceber(PedidosVendedorasModel pedido, PedidoReceberHistorico tipoHistorico)
        {
            TituloReceberModel titulo = new TituloReceberModel();
            titulo = (TituloReceberModel)_tituloReceberServices.GetByPedidoId(pedido.PedidoId);

            HistoricoTituloReceberModel historico = new HistoricoTituloReceberModel();
            historico.TituloId = titulo.TituloId;
            historico.DataRegistro = DateTime.Parse(DateTime.Now.ToString());
            historico.ValorRegistrado = titulo.ValorTitulo;
            if (tipoHistorico == PedidoReceberHistorico.Novo)
            {

                historico.Descricao = "Registro do título a receber\nGerado autmaticamente pelo sistemaa.";
            }
            else
            {
                historico.Descricao = "Atualização de título a receber\nGerado atuamaticamente pelo sistema.";

            }

            _historicoTituloReceberServices.Add(historico);

        }

        private void GeraContasReceber(PedidosVendedorasModel pedido)
        {
            TituloReceberModel tituloVendedora = new TituloReceberModel();
            //TODO: Configurar o tipo de título em configurações
            TipoTituloModel = _tipoTituloServices.GetById(1);
            //TODO: Configurar a data de vencimento conforme configurações do sistema.
            DateTime dataVencimento = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 10);

            //GERAR CONTAS CONTAS A RECEBER.
            tituloVendedora.VendedoraId = pedido.VendedoraId;
            tituloVendedora.PedidoId = pedido.PedidoId;
            tituloVendedora.TipoTituloId = TipoTituloModel.TipoId;
            tituloVendedora.StatusTitulo = StatusTitulo.Aberto;
            tituloVendedora.ValorTitulo = (double)(pedido.ValorTotalPedido - pedido.ValorLucroVendedora);
            tituloVendedora.ValorParcela = tituloVendedora.ValorTitulo;
            tituloVendedora.DataEmissao = DateTime.Now;
            tituloVendedora.DataRegistro = DateTime.Now;
            tituloVendedora.DataVencimento = dataVencimento;
            tituloVendedora.ValorDesconto = 0;
            tituloVendedora.ValorLiquidado = 0;
            tituloVendedora.QtdParcelas = 1;
            tituloVendedora.Parcelado = false;

            try
            {
                _tituloReceberServices.Add(tituloVendedora);
                MessageBox.Show($"Foi gerado um título a receber no valor de R$ {SelectedPedido.ValorTotalPedido - SelectedPedido.ValorLucroVendedora}");

            }
            catch (Exception e)
            {

                MessageBox.Show("Não foi possível registrar constas a receber.\nMessage: " + e.Message);
            }
        }

        private void AtualizaContaReceber(PedidosVendedorasModel pedido)
        {
            //ATUALIZA CONTAS A RECEBER
            TituloReceberModel tituloVendedora = new TituloReceberModel();
            tituloVendedora = (TituloReceberModel)_tituloReceberServices.GetByPedidoId(pedido.PedidoId);
            tituloVendedora.ValorTitulo = (double)(pedido.ValorTotalPedido - pedido.ValorLucroVendedora);
            tituloVendedora.ValorParcela = tituloVendedora.ValorTitulo;
            _tituloReceberServices.UpdateValor(tituloVendedora);

        }

        private bool TemContasReceber(PedidosVendedorasModel selectedPedido)
        {

            if (TitulosReceberList.Count != 0)
            {
                //VERIFICAR SE A VENDEDORA JÁ TEM UM CONTAS A RECEBER DESSE PEDIDO
                if (TitulosReceberList.Where(tit => tit.PedidoId == selectedPedido.PedidoId).Any())
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

        private void dgvPedidos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectedPedido = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(dgvPedidos.CurrentRow.Cells[0].Value.ToString()));
            EditarPedido(SelectedPedido, RequestType.Edit);
            AtualizaDGV();
        }

        public void AtualizaDGV()
        {
            ReadModels();
            LoadDataGridView();
            if (dgvPedidos.Rows.Count != 0)
            {
                dgvPedidos.Rows[0].Selected = false;
                dgvPedidos.Rows[indexDGV].Selected = true;
            }

        }


        #region RADIOS BUTTONS CHECKED CHANGED
        private void UncheckRadiosButtons()
        {
            rbAberto.Checked = false;
            rbSeparado.Checked = false;
            rbFinalizado.Checked = false;
            rbEntregue.Checked = false;
            rbEnviado.Checked = false;
            rbConferido.Checked = false;
            rbDespachado.Checked = false;
            rbCancelado.Checked = false;
            rbTodos.Checked = true;
        }
        private void rbAberto_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAberto.Checked)
                status = StatusPedido.Aberto;
            LoadDataGridView();
        }

        private void rbSeparado_CheckedChanged(object sender, EventArgs e)
        {

            if (rbSeparado.Checked)
                status = StatusPedido.Separado;
            LoadDataGridView();
        }

        private void rbFinalizado_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFinalizado.Checked)
                status = StatusPedido.Finalizado;
            else
                status = StatusPedido.Todos;
            LoadDataGridView();
        }

        private void rbEntregue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEntregue.Checked)
                status = StatusPedido.Entregue;
            LoadDataGridView();
        }

        private void rbEnviado_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEnviado.Checked)
                status = StatusPedido.Enviado;
            LoadDataGridView();
        }

        private void rbConferido_CheckedChanged(object sender, EventArgs e)
        {
            if (rbConferido.Checked)
                status = StatusPedido.Conferido;
            LoadDataGridView();
        }

        private void rbDespachado_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDespachado.Checked)
                status = StatusPedido.Despachado;
            LoadDataGridView();
        }

        private void rbCancelado_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCancelado.Checked)
                status = StatusPedido.Cancelado;
            LoadDataGridView();
        }
        private void rbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTodos.Checked)
            {
                status = StatusPedido.Todos;
            }
            LoadDataGridView();
        }
        #endregion

        private void dgvPedidos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex != dgvPedidos.NewRowIndex)
            {
                if (dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Aberto")
                {
                    dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LimeGreen;
                    dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.White;

                }
                else if (dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Conferido")
                {
                    dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Yellow;
                    dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Blue;

                }
                else if (dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Finalizado")
                {
                    dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.IndianRed;
                    dgvPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.White;

                }
            }
        }

        private void dgvPedidos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7)
            {
                SelectedPedido = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(dgvPedidos.CurrentRow.Cells[0].Value.ToString()));
                SelectedVendedora = _vendedoraServices.GetById(SelectedPedido.VendedoraId);
                var result = MessageBox.Show($"Tem certeza que deseja \"REABRIR\" o pedido da vendedora \"{SelectedVendedora.Nome}\"??", "Reabertura de Pedido", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    _pedidosServices.SetStatus(((int)StatusPedido.Aberto), SelectedPedido);
                }
            }
            AtualizaDGV();
        }

        private void dgvPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPedidos.CurrentRow != null)
            {
                indexDGV = dgvPedidos.CurrentRow.Index;
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            SelectedPedido = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(dgvPedidos.CurrentRow.Cells[0].Value.ToString()));

            if (SelectedPedido.StatusPed != (int)StatusPedido.Aberto)
            {
                MessageBox.Show("Somente pedidos em Aberto podem ser apagados");
            }
            else
            {
                SelectedVendedora = _vendedoraServices.GetById(SelectedPedido.VendedoraId);
                var result = MessageBox.Show($"Você está prestes a apagar o pedido {SelectedPedido.PedidoId} da Vendedora {SelectedVendedora.Nome}.\n Deseja Continuar?", "Deleção de Pedido", MessageBoxButtons.YesNo, MessageBoxIcon.Warning); ;
                if (result == DialogResult.Yes)
                {
                    LimpaPedido(SelectedPedido);
                    EditarPedido(SelectedPedido, RequestType.Remove);

                }
                AtualizaDGV();
            }

        }

        private void LimpaPedido(PedidosVendedorasModel selectedPedido)
        {
            DetalhePedidoList = (IEnumerable<DetalhePedidoModel>)_detalhePedidoSerivces.GetAllByPedido(selectedPedido);
            foreach (var item in DetalhePedidoList)
            {
                _detalhePedidoSerivces.Delete(item);
            }
        }


        #region SET BG COLOR CONTROLS
        private void SetBgColorFocused(Control control)
        {
            control.BackColor = SystemColors.GradientActiveCaption;
        }
        private void SetBgColorUnfocused(Control control)
        {
            control.BackColor = SystemColors.Window;
        }
        private void dateDataInicio_Enter(object sender, EventArgs e)
        {
            SetBgColorFocused(dateDataInicio);
        }

        private void dateDataFim_Enter(object sender, EventArgs e)
        {
            SetBgColorFocused(dateDataFim);
        }

        private void mTextCpf_Enter(object sender, EventArgs e)
        {
            SetBgColorFocused(mTextCpf);
        }

        private void cbNomeVendedora_Enter(object sender, EventArgs e)
        {
            SetBgColorFocused(cbNomeVendedora);
        }

        private void dateDataInicio_Leave(object sender, EventArgs e)
        {
            SetBgColorUnfocused(dateDataInicio);
        }

        private void dateDataFim_Leave(object sender, EventArgs e)
        {
            SetBgColorUnfocused(dateDataFim);
        }

        private void mTextCpf_Leave(object sender, EventArgs e)
        {
            SetBgColorUnfocused(mTextCpf);
        }
        #endregion

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //TODO: Cancelar contas a receber e contas a pagar.
            SelectedPedido = (PedidosVendedorasModel)_pedidosServices.GetById(int.Parse(dgvPedidos.CurrentRow.Cells[0].Value.ToString()));
            SelectedVendedora = _vendedoraServices.GetById(SelectedPedido.VendedoraId);

            if (SelectedPedido.StatusPed == (int)StatusPedido.Finalizado)
            {
                MessageBox.Show("Pedidos finalizados não podem ser cancelados");
            }
            else
            {

                var result = MessageBox.Show($"Você está prestes a CANCELAR o pedido {SelectedPedido.PedidoId} da Vendedora {SelectedVendedora.Nome}.\n Deseja Continuar?", "Deleção de Pedido", MessageBoxButtons.YesNo, MessageBoxIcon.Warning); ;
                if (result == DialogResult.Yes)
                {
                    LimpaPedido(SelectedPedido);
                    EditarPedido(SelectedPedido, RequestType.Cancela);

                }

                AtualizaDGV();
            }
        }
    }
}
