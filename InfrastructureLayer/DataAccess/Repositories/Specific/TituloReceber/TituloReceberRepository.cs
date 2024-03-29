﻿using CommonComponents;

using DomainLayer.Models.CommonModels.Enums;
using DomainLayer.Models.TitulosReceber;
using DomainLayer.Models.Vendedora;

using ServiceLayer.Services.TitulosReceberServices;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Configuration;

namespace InfrastructureLayer.DataAccess.Repositories.Specific.TituloReceber
{
    public class TituloReceberRepository : ITituloReceberRepository
    {
        private string _connectionString;

        public TituloReceberRepository()
        {

        }

        public TituloReceberRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AbaterValor(ITituloReceberModel tituloReceber, double valorAbatido)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "UPDATE TitulosReceber SET ValorLiquidado = @ValorLiquidado WHERE TituloId = @TituloId";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@ValorLiquidado", valorAbatido));
                        cmd.Parameters.AddWithValue("@TituloId", tituloReceber.TituloId);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível registrar o valor abatido no Título", e.HelpLink, e.ErrorCode,
                        e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public ITituloReceberModel Add(ITituloReceberModel tituloReceber)
        {
            int idReturned;
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "INSERT INTO TitulosReceber " +
                            " (VendedoraId, PedidoId, TipoTituloId, StatusTituloId, ValorTitulo, ValorParcela, DataEmissao, DataVencimento, ValorDesconto, ValorLiquidado, QtdParcelas, Parcelado) " +
                            " OUTPUT INSERTED.TituloId " +
                            " VALUES " +
                            " (@VendedoraId, @PedidoId, @TipoTituloId, @StatusTituloId, @ValorTitulo, @ValorParcela, @DataEmissao, @DataVencimento, @ValorDesconto, @ValorLiquidado, @QtdParcelas, @Parcelado) ";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@VendedoraId", tituloReceber.VendedoraId);
                        cmd.Parameters.AddWithValue("@PedidoId", tituloReceber.PedidoId);
                        cmd.Parameters.AddWithValue("@TipoTituloId", tituloReceber.TipoTituloId);
                        cmd.Parameters.AddWithValue("@StatusTituloId", tituloReceber.StatusTitulo);
                        cmd.Parameters.AddWithValue("@ValorTitulo", tituloReceber.ValorTitulo);
                        cmd.Parameters.AddWithValue("@DataEmissao", tituloReceber.DataEmissao);
                        cmd.Parameters.AddWithValue("@DataVencimento", tituloReceber.DataVencimento);
                        cmd.Parameters.AddWithValue("@ValorDesconto", tituloReceber.ValorDesconto);
                        cmd.Parameters.AddWithValue("@ValorLiquidado", tituloReceber.ValorLiquidado);
                        cmd.Parameters.AddWithValue("@QtdParcelas", tituloReceber.QtdParcelas);
                        cmd.Parameters.AddWithValue("@ValorParcela", tituloReceber.ValorParcela);
                        cmd.Parameters.AddWithValue("@Parcelado", tituloReceber.Parcelado);


                        idReturned = (int)cmd.ExecuteScalar();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível registrar o Título a Receber", e.HelpLink, e.ErrorCode,
                        e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }

                tituloReceber.TituloId = idReturned;
                return tituloReceber;

            }
        }

        public void Cancelar(ITituloReceberModel tituloReceber)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "UPDATE TitulosReceber SET StatusTituloId = @StatusTituloId WHERE TituloId = @TituloId";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@CanceladoId", tituloReceber.StatusTitulo);
                        cmd.Parameters.AddWithValue("@TituloId", tituloReceber.TituloId);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível cancelar o Título a Receber", e.HelpLink, e.ErrorCode,
                        e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public ITituloReceberModel GetById(int id)
        {
            TituloReceberModel tituloReceber = new TituloReceberModel();

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "SELECT * FROM TitulosReceber WHERE TituloId = @TituloId";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@TituloId", id));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tituloReceber.TituloId = int.Parse(reader["TituloId"].ToString());
                                tituloReceber.VendedoraId = int.Parse(reader["VendedoraId"].ToString());
                                tituloReceber.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                tituloReceber.TipoTituloId = int.Parse(reader["TipoTituloId"].ToString());
                                tituloReceber.StatusTitulo = (StatusTitulo)Enum.Parse(typeof(StatusTitulo), reader["StatusTituloId"].ToString());
                                tituloReceber.ValorTitulo = double.Parse(reader["ValorTitulo"].ToString());
                                tituloReceber.ValorParcela = double.Parse(reader["ValorParcela"].ToString());
                                tituloReceber.DataEmissao = DateTime.Parse(reader["DataEmissao"].ToString());
                                tituloReceber.DataRegistro = DateTime.Parse(reader["DataRegistro"].ToString());
                                tituloReceber.DataVencimento = DateTime.Parse(reader["DataVencimento"].ToString());
                                tituloReceber.ValorDesconto = double.Parse(reader["ValorDesconto"].ToString());
                                tituloReceber.ValorLiquidado = double.Parse(reader["ValorLiquidado"].ToString());
                                tituloReceber.QtdParcelas = int.Parse(reader["QtdParcelas"].ToString());
                                tituloReceber.Parcelado = bool.Parse(reader["Parcelado"].ToString());
                            }

                        }

                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível recuperar o Título pelo Id", e.HelpLink, e.ErrorCode,
                        e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }

                return tituloReceber;
            }
        }

        public IEnumerable<ITituloReceberModel> GetAll()
        {
            List<TituloReceberModel> modelList = new List<TituloReceberModel>();

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "SELECT * FROM TitulosReceber";
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


                                TituloReceberModel model = new TituloReceberModel();

                                model.TituloId = int.Parse(reader["TituloId"].ToString());
                                model.VendedoraId = int.Parse(reader["VendedoraId"].ToString());
                                model.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                model.StatusTitulo = (StatusTitulo)Enum.Parse(typeof(StatusTitulo), reader["StatusTituloId"].ToString());
                                model.TipoTituloId = int.Parse(reader["TipoTituloId"].ToString());
                                model.ValorTitulo = double.Parse(reader["ValorTitulo"].ToString());
                                model.ValorParcela = double.Parse(reader["ValorParcela"].ToString());
                                model.DataEmissao = DateTime.Parse(reader["DataEmissao"].ToString());
                                model.DataRegistro = DateTime.Parse(reader["DataRegistro"].ToString());
                                model.DataVencimento = DateTime.Parse(reader["DataVencimento"].ToString());
                                model.ValorDesconto = double.Parse(reader["ValorDesconto"].ToString());
                                model.ValorLiquidado = double.Parse(reader["ValorLiquidado"].ToString());
                                model.QtdParcelas = int.Parse(reader["QtdParcelas"].ToString());
                                model.Parcelado = bool.Parse(reader["Parcelado"].ToString());

                                modelList.Add(model);
                            }
                        }

                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível recuperar a lista de Títulos a Receber", e.HelpLink, e.ErrorCode,
                        e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }

                return modelList;
            }
        }

        public IEnumerable<ITituloReceberModel> GetAllByVendedora(IVendedoraModel vendedoraModel)
        {
            List<TituloReceberModel> modelList = new List<TituloReceberModel>();

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "SELECT * FROM TitulosReceber WHERE VendedoraId = @VendedoraId";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@VendedoraId", vendedoraModel.VendedoraId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {


                            while (reader.Read())
                            {
                                TituloReceberModel model = new TituloReceberModel();
                                model.TituloId = int.Parse(reader["TituloId"].ToString());
                                model.VendedoraId = int.Parse(reader["VendedoraId"].ToString());
                                model.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                model.TipoTituloId = int.Parse(reader["TipoTituloId"].ToString());
                                model.StatusTitulo = (StatusTitulo)Enum.Parse(typeof(StatusTitulo), reader["StatusTituloId"].ToString());
                                model.ValorTitulo = double.Parse(reader["ValorTitulo"].ToString());
                                model.ValorParcela = double.Parse(reader["ValorParcela"].ToString());
                                model.DataEmissao = DateTime.Parse(reader["DataEmissao"].ToString());
                                model.DataRegistro = DateTime.Parse(reader["DataRegistro"].ToString());
                                model.DataVencimento = DateTime.Parse(reader["DataVencimento"].ToString());
                                model.ValorDesconto = double.Parse(reader["ValorDesconto"].ToString());
                                model.ValorLiquidado = double.Parse(reader["ValorLiquidado"].ToString());
                                model.QtdParcelas = int.Parse(reader["QtdParcelas"].ToString());
                                model.Parcelado = bool.Parse(reader["Parcelado"].ToString());
                                modelList.Add(model);
                            }



                        }

                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não recuperar os títulos da Vendedora", e.HelpLink, e.ErrorCode,
                        e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }

                return modelList;
            }
        }

        public void LiquidarTitulo(ITituloReceberModel tituloReceber)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "UPDATE TitulosReceber SET ValorLiquidado = @ValorLiquidado, StatusTituloId = @StatusTituloId WHERE TituloId = @TituloId";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@TituloId", tituloReceber.TituloId);
                        cmd.Parameters.AddWithValue("@ValorLiquidado", tituloReceber.ValorLiquidado);
                        cmd.Parameters.AddWithValue("@StatusTituloId", tituloReceber.StatusTitulo);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível liquidar o título selecionado", e.HelpLink, e.ErrorCode,
                        e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void Protestar(ITituloReceberModel tituloReceber)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "UPDATE TitulosReceber SET StatusTituloId = @StatusTituloId WHERE TituloId = @TituloId";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@TituloId", tituloReceber.TituloId);
                        cmd.Parameters.AddWithValue("@StatusTituloId", tituloReceber.StatusTitulo);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível liquidar o título selecionado", e.HelpLink, e.ErrorCode,
                        e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void UpdateValor(ITituloReceberModel tituloVendedora)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "UPDATE TitulosReceber SET ValorTitulo = @ValorTitulo, ValorParcela = @ValorParcela WHERE TituloId = @TituloId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@ValorTitulo", tituloVendedora.ValorTitulo);
                        cmd.Parameters.AddWithValue("@ValorParcela", tituloVendedora.ValorParcela);
                        cmd.Parameters.AddWithValue("@TituloId", tituloVendedora.TituloId);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível atualiar os valores do título a receber", e.HelpLink, e.ErrorCode, e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public ITituloReceberModel GetByPedidoId(int pedidoId)
        {
            TituloReceberModel model = new TituloReceberModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = " SELECT * FROM TitulosReceber WHERE PedidoId = @PedidoId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@PedidoId", pedidoId));
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                model.TituloId = int.Parse(reader["TituloId"].ToString());
                                model.VendedoraId = int.Parse(reader["VendedoraId"].ToString());
                                model.PedidoId = int.Parse(reader["PedidoId"].ToString());
                                model.TipoTituloId = int.Parse(reader["TipoTituloId"].ToString());
                                model.StatusTitulo = (StatusTitulo)Enum.Parse(typeof(StatusTitulo), reader["StatusTituloId"].ToString());
                                model.ValorTitulo = double.Parse(reader["ValorTitulo"].ToString());
                                model.ValorParcela = double.Parse(reader["ValorParcela"].ToString());
                                model.DataEmissao = DateTime.Parse(reader["DataEmissao"].ToString());
                                model.DataRegistro = DateTime.Parse(reader["DataRegistro"].ToString());
                                model.DataVencimento = DateTime.Parse(reader["DataVencimento"].ToString());
                                model.ValorDesconto = double.Parse(reader["ValorDesconto"].ToString());
                                model.ValorLiquidado = double.Parse(reader["ValorLiquidado"].ToString());
                                model.QtdParcelas = int.Parse(reader["QtdParcelas"].ToString());
                                model.Parcelado = bool.Parse(reader["Parcelado"].ToString());
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues("Error", false, e.Message, "Não foi possível recuperar o título pelo Pedido Id", e.HelpLink, e.ErrorCode, e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }

                return model;
            }

        }
    }
}
