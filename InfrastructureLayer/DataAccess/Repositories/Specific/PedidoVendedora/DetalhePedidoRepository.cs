﻿using CommonComponents;

using DomainLayer.Models.Catalogos;
using DomainLayer.Models.PedidosVendedoras;

using ServiceLayer.Services.DetalhePedidoServices;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.DataAccess.Repositories.Specific.PedidoVendedora
{
    public class DetalhePedidoRepository : IDetalhePedidoRepository
    {
        private string _connectionString;
        public DetalhePedidoRepository()
        {

        }

        public DetalhePedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DetalhePedidoModel AddNoTamanho(IDetalhePedidoModel detalhePedidoModel)
        {
            int idReturned = 0;
            DetalhePedidoModel detalhePedidoReturned = new DetalhePedidoModel();

            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            string query = "INSERT INTO DetalhesPedidosVendedoras " +
                           " (PedidoId, CatalogoId, CampanhaId, ProdutoId, Descricao, Referencia, MargemVendedora, MargemDistribuidor, ValorProduto, Quantidade, ValorTotalItem, ValorLucroVendedoraItem, ValorLucroDistribuidorItem, ValorPagarFornecedorItem, Faltou) " +
                           " OUTPUT INSERTED.DetalheId " +
                           " VALUES " +
                           " (@PedidoId, @CatalogoId, @CampanhaId, @ProdutoId, @Descricao, @Referencia, @MargemVendedora, @MargemDistribuidor, @ValorProduto, @Quantidade, @ValorTotalItem, @ValorLucroVendedoraItem, @ValorLucroDistribuidorItem, @ValorPagarFornecedorItem, @Faltou) ";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@PedidoId", detalhePedidoModel.PedidoId);
                        cmd.Parameters.AddWithValue("@CatalogoId", detalhePedidoModel.CatalogoId);
                        cmd.Parameters.AddWithValue("@CampanhaId", detalhePedidoModel.CampanhaId);
                        cmd.Parameters.AddWithValue("@ProdutoId", detalhePedidoModel.ProdutoId);
                        cmd.Parameters.AddWithValue("@Referencia", detalhePedidoModel.Referencia);
                        cmd.Parameters.AddWithValue("@Descricao", detalhePedidoModel.Descricao);
                        cmd.Parameters.AddWithValue("@MargemVendedora", detalhePedidoModel.MargemVendedora);
                        cmd.Parameters.AddWithValue("@MargemDistribuidor", detalhePedidoModel.MargemDistribuidor);
                        cmd.Parameters.AddWithValue("@ValorProduto", detalhePedidoModel.ValorProduto);
                        cmd.Parameters.AddWithValue("@Quantidade", detalhePedidoModel.Quantidade);
                        cmd.Parameters.AddWithValue("@ValorTotalItem", detalhePedidoModel.ValorTotalItem);
                        cmd.Parameters.AddWithValue("@ValorLucroVendedoraItem", detalhePedidoModel.ValorLucroVendedoraItem);
                        cmd.Parameters.AddWithValue("@ValorLucroDistribuidorItem", detalhePedidoModel.ValorLucroDistribuidorItem);
                        cmd.Parameters.AddWithValue("@ValorPagarFornecedorItem", detalhePedidoModel.ValorPagarFornecedorItem);
                        cmd.Parameters.AddWithValue("@Faltou", detalhePedidoModel.Faltou);

                        idReturned = (int)cmd.ExecuteScalar();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Nâo foi possível adicionar item ao pedido",
                        e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
                detalhePedidoReturned = GetById(idReturned);
                return detalhePedidoReturned;
            }
        }

        public DetalhePedidoModel AddWithTamanho(IDetalhePedidoModel detalhePedidoModel)
        {
            int idReturned = 0;
            DetalhePedidoModel detalhePedidoReturned = new DetalhePedidoModel();

            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            string query = "INSERT INTO DetalhesPedidosVendedoras " +
                           " (PedidoId, CatalogoId, CampanhaId, ProdutoId, Referencia, Descricao, MargemVendedora, MargemDistribuidor, ValorProduto, Quantidade, TamanhoId, ValorTotalItem, ValorLucroVendedoraItem, ValorLucroDistribuidorItem, ValorPagarFornecedorItem, Faltou) " +
                           " OUTPUT INSERTED.DetalheId " +
                           " VALUES " +
                           " (@PedidoId, @CatalogoId, @CampanhaId, @ProdutoId, @Referencia, @Descricao, @MargemVendedora, @MargemDistribuidor, @ValorProduto, @Quantidade, @TamanhoId, @ValorTotalItem, @ValorLucroVendedoraItem, @ValorLucroDistribuidorItem, @ValorPagarFornecedorItem, @Faltou) ";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@PedidoId", detalhePedidoModel.PedidoId);
                        cmd.Parameters.AddWithValue("@CatalogoId", detalhePedidoModel.CatalogoId);
                        cmd.Parameters.AddWithValue("@CampanhaId", detalhePedidoModel.CampanhaId);
                        cmd.Parameters.AddWithValue("@ProdutoId", detalhePedidoModel.ProdutoId);
                        cmd.Parameters.AddWithValue("@Referencia", detalhePedidoModel.Referencia);
                        cmd.Parameters.AddWithValue("@Descricao", detalhePedidoModel.Descricao);
                        cmd.Parameters.AddWithValue("@MargemVendedora", detalhePedidoModel.MargemVendedora);
                        cmd.Parameters.AddWithValue("@MargemDistribuidor", detalhePedidoModel.MargemDistribuidor);
                        cmd.Parameters.AddWithValue("@ValorProduto", detalhePedidoModel.ValorProduto);
                        cmd.Parameters.AddWithValue("@Quantidade", detalhePedidoModel.Quantidade);
                        cmd.Parameters.AddWithValue("@TamanhoId", detalhePedidoModel.TamanhoId);
                        cmd.Parameters.AddWithValue("@ValorTotalItem", detalhePedidoModel.ValorTotalItem);
                        cmd.Parameters.AddWithValue("@ValorLucroVendedoraItem", detalhePedidoModel.ValorLucroVendedoraItem);
                        cmd.Parameters.AddWithValue("@ValorLucroDistribuidorItem", detalhePedidoModel.ValorLucroDistribuidorItem);
                        cmd.Parameters.AddWithValue("@ValorPagarFornecedorItem", detalhePedidoModel.ValorPagarFornecedorItem);
                        cmd.Parameters.AddWithValue("@Faltou", detalhePedidoModel.Faltou);

                        idReturned = (int)cmd.ExecuteScalar();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Nâo foi possível adicionar item ao pedido",
                        e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
                detalhePedidoReturned = GetById(idReturned);
                return detalhePedidoReturned;
            }
        }

        public void Delete(IDetalhePedidoModel detalhePedidoModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "DELETE FROM DetalhesPedidosVendedoras WHERE DetalheId = @DetalheId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@DetalheId", detalhePedidoModel.DetalheId);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível apagar o item do pedido", e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public IEnumerable<IDetalhePedidoModel> GetAll()
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            List<DetalhePedidoModel> DetalheListModel = new List<DetalhePedidoModel>();

            string query = "SELECT * FROM DetalhesPedidosVendedoras";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DetalhePedidoModel model = new DetalhePedidoModel();

                                model.DetalheId = int.Parse(reader["DetalheId"].ToString());
                                model.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                model.CatalogoId = int.Parse(reader["CatalogoId"].ToString());
                                model.CampanhaId = int.Parse(reader["CampanhaId"].ToString());
                                model.ProdutoId = int.Parse(reader["ProdutoId"].ToString());
                                model.Referencia = reader["Referencia"].ToString();
                                model.Descricao = reader["Descricao"].ToString();
                                model.MargemVendedora = double.Parse(reader["MargemVendedora"].ToString());
                                model.MargemDistribuidor = double.Parse(reader["MargemDistribuidor"].ToString());
                                model.ValorProduto = double.Parse(reader["ValorProduto"].ToString());
                                model.Quantidade = int.Parse(reader["Quantidade"].ToString());
                                model.TamanhoId = string.IsNullOrEmpty(reader["TamanhoId"].ToString()) ? 0 : int.Parse(reader["TamanhoId"].ToString());
//                                model.ValorTotalItem = double.Parse(reader["ValorTotalItem"].ToString());
                                //model.ValorLucroVendedoraItem = double.Parse(reader["ValorLucroVendedoraItem"].ToString());
                                //model.ValorLucroDistribuidorItem = double.Parse(reader["ValorLucroDistribuidorItem"].ToString());
                                //model.ValorPagarFornecedorItem = double.Parse(reader["ValorPagarFornecedorItem"].ToString());
                                model.Faltou = bool.Parse(reader["Faltou"].ToString());

                                DetalheListModel.Add(model);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível recuperar a Lista de Ítens dos Pedidos.", e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
                return DetalheListModel;
            }
        }

        public IEnumerable<IDetalhePedidoModel> GetAllByCampanha(ICampanhaModel model)
        {
            List<DetalhePedidoModel> detalhes = new List<DetalhePedidoModel>();

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query =  " SELECT DetalheId, PedidoId, CatalogoId, CampanhaId, ProdutoId, Referencia, MargemVendedora, " +
                            " MargemDistribuidor, ValorProduto, Quantidade FROM DetalhesPedidosVendedoras " +
                            " WHERE CampanhaId = @CampanhaId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@CampanhaId", model.CampanhaId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DetalhePedidoModel detalheModel = new DetalhePedidoModel();
                                detalheModel.DetalheId = int.Parse(reader["DetalheId"].ToString());
                                detalheModel.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                detalheModel.CatalogoId = int.Parse(reader["CatalogoId"].ToString());
                                detalheModel.CampanhaId = int.Parse(reader["CampanhaId"].ToString());
                                detalheModel.Referencia = reader["Referencia"].ToString();
                                detalheModel.Descricao = reader["Descricao"].ToString();
                                detalheModel.MargemVendedora = double.Parse(reader["MargemVendedora"].ToString());
                                detalheModel.MargemDistribuidor = double.Parse(reader["MargemDistribuidor"].ToString());
                                detalheModel.ValorProduto = double.Parse(reader["ValorProduto"].ToString());
                                detalheModel.Quantidade = int.Parse(reader["Quantidade"].ToString());

                                detalhes.Add(detalheModel);


                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível recuperar lista de Detalhes de Pedidos pela Campanha Selecionada",
                        e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }

            return detalhes;


        }

        public IEnumerable<IDetalhePedidoModel> GetAllByPedido(IPedidosVendedorasModel pedidosVendedorasModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            List<DetalhePedidoModel> DetalheListModel = new List<DetalhePedidoModel>();
            string query = "SELECT DetalheId, PedidoId, CatalogoId, CampanhaId, ProdutoId, Referencia, Descricao, MargemVendedora, MargemDistribuidor, ValorProduto, Quantidade, TamanhoId, ValorTotalItem, ValorLucroVendedoraItem, ValorLucroDistribuidorItem, ValorPagarFornecedorItem, Faltou " +
                           " FROM DetalhesPedidosVendedoras " +
                           " WHERE PedidoId = @PedidoId ";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@PedidoId", pedidosVendedorasModel.PedidoId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DetalhePedidoModel model = new DetalhePedidoModel();

                                model.DetalheId = int.Parse(reader["DetalheId"].ToString());
                                model.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                model.CatalogoId = int.Parse(reader["CatalogoId"].ToString());
                                model.CampanhaId = int.Parse(reader["CampanhaId"].ToString());
                                model.ProdutoId = int.Parse(reader["ProdutoId"].ToString());
                                model.Referencia = reader["Referencia"].ToString();
                                model.Descricao = reader["Descricao"].ToString();
                                model.MargemVendedora = double.Parse(reader["MargemVendedora"].ToString());
                                model.MargemDistribuidor = double.Parse(reader["MargemDistribuidor"].ToString());
                                model.ValorProduto = double.Parse(reader["ValorProduto"].ToString());
                                model.Quantidade = int.Parse(reader["Quantidade"].ToString());
                                model.TamanhoId = string.IsNullOrEmpty(reader["TamanhoId"].ToString()) ? 0 : int.Parse(reader["TamanhoId"].ToString());
                                //model.ValorTotalItem = double.Parse(reader["ValorTotalItem"].ToString());
                                //model.ValorLucroVendedoraItem = double.Parse(reader["ValorLucroVendedoraItem"].ToString());
                                //model.ValorLucroDistribuidorItem = double.Parse(reader["ValorLucroDistribuidorItem"].ToString());
                                //model.ValorPagarFornecedorItem = double.Parse(reader["ValorPagarFornecedorItem"].ToString());
                                model.Faltou = bool.Parse(reader["Faltou"].ToString());

                                DetalheListModel.Add(model);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível recuperar a Lista de Ítens dos Pedidos.", e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
            return DetalheListModel;
        }

        public IEnumerable<IDetalhePedidoModel> GetAllByPedidoCatalogo(IPedidosVendedorasModel pedidosVendedorasModel, ICatalogoModel catalogoModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            List<DetalhePedidoModel> DetalheListModel = new List<DetalhePedidoModel>();
            string query = "SELECT DetalheId, PedidoId, CatalogoId, CampanhaId, ProdutoId, Referencia, Descricao,MargemVendedora, MargemDistribuidor, ValorProduto, Quantidade, Tamanho, ValorTotalItem, ValorLucroVendedoraItem, ValorLucroDistribuidorItem, ValorPagarFornecedorItem, Faltou " +
                           " FROM DetalhesPedidosVendedoras " +
                           " WHERE PedidoId = @PedidoId AND CatalogoId = @CatalogoId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@PedidoId", pedidosVendedorasModel.PedidoId);
                        cmd.Parameters.AddWithValue("@CatalogoId", catalogoModel.CatalogoId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DetalhePedidoModel model = new DetalhePedidoModel();

                                model.DetalheId = int.Parse(reader["DetalheId"].ToString());
                                model.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                model.CatalogoId = int.Parse(reader["CatalogoId"].ToString());
                                model.CampanhaId = int.Parse(reader["CampanhaId"].ToString());
                                model.ProdutoId = int.Parse(reader["ProdutoId"].ToString());
                                model.Referencia = reader["Referencia"].ToString();
                                model.Descricao = reader["Descricao"].ToString();
                                model.MargemVendedora = double.Parse(reader["MargemVendedora"].ToString());
                                model.MargemDistribuidor = double.Parse(reader["MargemDistribuidor"].ToString());
                                model.ValorProduto = double.Parse(reader["ValorProduto"].ToString());
                                model.Quantidade = int.Parse(reader["Quantidade"].ToString());
                                model.TamanhoId = int.Parse(reader["TamanhoId"].ToString());
                                //model.ValorTotalItem = double.Parse(reader["ValorTotalItem"].ToString());
                                //model.ValorLucroVendedoraItem = double.Parse(reader["ValorLucroVendedoraItem"].ToString());
                                //model.ValorLucroDistribuidorItem = double.Parse(reader["ValorLucroDistribuidorItem"].ToString());
                                //model.ValorPagarFornecedorItem = double.Parse(reader["ValorPagarFornecedorItem"].ToString());
                                model.Faltou = bool.Parse(reader["Faltou"].ToString());

                                DetalheListModel.Add(model);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível recuperar a Lista de Ítens dos Pedidos.", e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
            return DetalheListModel;
        }

        public DetalhePedidoModel GetById(int detalheId)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            DetalhePedidoModel model = new DetalhePedidoModel();
            string query = "SELECT DetalheId, PedidoId, CatalogoId, CampanhaId, ProdutoId, Referencia, Descricao, MargemVendedora, MargemDistribuidor, ValorProduto, Quantidade, TamanhoId, ValorTotalItem, ValorLucroVendedoraItem, ValorLucroDistribuidorItem, ValorPagarFornecedorItem, Faltou " +
                           " FROM DetalhesPedidosVendedoras " +
                           " WHERE DetalheId = @DetalheId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@DetalheId", detalheId));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                model.DetalheId = int.Parse(reader["DetalheId"].ToString());
                                model.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                model.CatalogoId = int.Parse(reader["CatalogoId"].ToString());
                                model.CampanhaId = int.Parse(reader["CampanhaId"].ToString());
                                model.ProdutoId = int.Parse(reader["ProdutoId"].ToString());
                                model.Referencia = reader["Referencia"].ToString();
                                model.Descricao = reader["Descricao"].ToString();
                                model.MargemVendedora = double.Parse(reader["MargemVendedora"].ToString());
                                model.MargemDistribuidor = double.Parse(reader["MargemDistribuidor"].ToString());
                                model.ValorProduto = double.Parse(reader["ValorProduto"].ToString());
                                model.Quantidade = int.Parse(reader["Quantidade"].ToString());
                                model.TamanhoId = string.IsNullOrEmpty(reader["TamanhoId"].ToString()) ? 0 : int.Parse(reader["TamanhoId"].ToString());
                                //model.ValorTotalItem = double.Parse(reader["ValorTotalItem"].ToString());
                                //model.ValorLucroVendedoraItem = double.Parse(reader["ValorLucroVendedoraItem"].ToString());
                                //model.ValorLucroDistribuidorItem = double.Parse(reader["ValorLucroDistribuidorItem"].ToString());
                                //model.ValorPagarFornecedorItem = double.Parse(reader["ValorPagarFornecedorItem"].ToString());
                                model.Faltou = bool.Parse(reader["Faltou"].ToString());

                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível recuperar a Lista de Ítens dos Pedidos.", e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
            return model;
        }

        public void Update(IDetalhePedidoModel detalhePedidoModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "UPDATE DetalhesPedidosVendedoras SET " +
                           "PedidoId = @PedidoId, " +
                           "CatalogoId = @CatalogoId, " +
                           "CampanhaId = @CampanhaId, " +
                           "ProdutoId = @ProdutoId, " +
                           "Referencia = @Referencia , " +
                           "Descricao = @Descricao, " +
                           "MargemVendedora = @MargemVendedora, " +
                           "MargemDistribuidor = @MargemDistribuidor, " +
                           "ValorProduto = @ValorProduto, " +
                           "Quantidade = @Quantidade, " +
                           "TamanhoId = @TamanhoId, " +
                           "ValorTotalItem = @ValorTotalItem, " +
                           "ValorLucroVendedoraItem = @ValorLucroVendedoraItem, " +
                           "ValorLucroDistribuidorItem = @ValorLucroDistribuidorItem, " +
                           "ValorPagarFornecedorItem = @ValorPagarFornecedorItem, " +
                           "Faltou = @Faltou " +
                           "WHERE  DetalheId = @DetalheId";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {


                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();

                        cmd.Parameters.AddWithValue("@DetalheId", detalhePedidoModel.DetalheId);
                        cmd.Parameters.AddWithValue("@PedidoId", detalhePedidoModel.PedidoId);
                        cmd.Parameters.AddWithValue("@CatalogoId", detalhePedidoModel.CatalogoId);
                        cmd.Parameters.AddWithValue("@CampanhaId", detalhePedidoModel.CampanhaId);
                        cmd.Parameters.AddWithValue("@ProdutoId", detalhePedidoModel.ProdutoId);
                        cmd.Parameters.AddWithValue("@Referencia", detalhePedidoModel.Referencia);
                        cmd.Parameters.AddWithValue("@Descricao", detalhePedidoModel.Descricao);
                        cmd.Parameters.AddWithValue("@MargemVendedora", detalhePedidoModel.MargemVendedora);
                        cmd.Parameters.AddWithValue("@MargemDistribuidor", detalhePedidoModel.MargemDistribuidor);
                        cmd.Parameters.AddWithValue("@ValorProduto", detalhePedidoModel.ValorProduto);
                        cmd.Parameters.AddWithValue("@Quantidade", detalhePedidoModel.Quantidade);
                        cmd.Parameters.AddWithValue("@TamanhoId", detalhePedidoModel.TamanhoId);
                        cmd.Parameters.AddWithValue("@ValorTotalItem", detalhePedidoModel.ValorTotalItem);
                        cmd.Parameters.AddWithValue("@ValorLucroVendedoraItem", detalhePedidoModel.ValorLucroVendedoraItem);
                        cmd.Parameters.AddWithValue("@ValorLucroDistribuidorItem", detalhePedidoModel.ValorLucroDistribuidorItem);
                        cmd.Parameters.AddWithValue("@ValorPagarFornecedorItem", detalhePedidoModel.ValorPagarFornecedorItem);
                        cmd.Parameters.AddWithValue("@Faltou", detalhePedidoModel.Faltou);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível Atualizar o Item do Pedido.", e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
