﻿using CommonComponents;

using DomainLayer.Models.Vendedora;

using ServiceLayer.Services.RotaServices;
using ServiceLayer.Services.VendedoraServices;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.Reflection;

namespace InfrastructureLayer.DataAccess.Repositories.Specific.Vendedora
{
    public class VendedoraRepository : IVendedoraRepository
    {
        private string _connectionString;
        enum TypeOfExistenceCheck
        {
            DoesExistInDB,
            DoesNotExistInDB
        }
        enum RequestType
        {
            Add,
            Update,
            Read,
            Delete,
            ConfirmAdd,
            ConfirmDelete
        }
        public VendedoraRepository()
        {
        }
        public VendedoraRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IEnumerable<IVendedoraModel> GetAll()
        {
            List<VendedoraModel> vendedoraModelsList = new List<VendedoraModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SQLiteConnection sQLiteConnection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    string sql = "SELECT * FROM Vendedoras ";

                    sQLiteConnection.Open();

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, sQLiteConnection))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VendedoraModel vendedoraModel = new VendedoraModel();

                                vendedoraModel.VendedoraId = Int32.Parse(reader["VendedoraId"].ToString());
                                vendedoraModel.Nome = reader["Nome"].ToString();
                                vendedoraModel.Cpf = reader["Cpf"].ToString();
                                vendedoraModel.Rg = reader["Rg"].ToString();
                                vendedoraModel.RgEmissor = reader["RgEmissor"].ToString();
                                vendedoraModel.DataNascimento = DateTime.Parse(reader["DataNascimento"].ToString());
                                vendedoraModel.Email = reader["Email"].ToString();
                                vendedoraModel.NomePai = reader["NomePai"].ToString();
                                vendedoraModel.NomeMae = reader["NomeMae"].ToString();
                                vendedoraModel.NomeConjuge = reader["NomeConjuge"].ToString();
                                vendedoraModel.Logradouro = reader["Logradouro"].ToString();
                                vendedoraModel.Numero = reader["Numero"].ToString();
                                vendedoraModel.Complemento = reader["Complemento"].ToString();
                                vendedoraModel.Cep = reader["Cep"].ToString();
                                vendedoraModel.UfRgId = Int32.Parse(reader["UfRgId"].ToString());
                                vendedoraModel.EstadoCivilId = Int32.Parse(reader["EstadoCivilId"].ToString());
                                vendedoraModel.RotaId = Int32.Parse(reader["RotaId"].ToString());
                                vendedoraModel.Rota = new RotaModel { RotaId = Int32.Parse(reader["RotaId"].ToString()) };
                                vendedoraModel.EstadoId = Int32.Parse(reader["EstadoId"].ToString());
                                vendedoraModel.CidadeId = Int32.Parse(reader["CidadeId"].ToString());
                                vendedoraModel.BairroId = Int32.Parse(reader["BairroId"].ToString());

                                vendedoraModelsList.Add(vendedoraModel);
                            }
                        }
                        sQLiteConnection.Close();
                    }

                }
                catch (SQLiteException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                   customMessage: "Unable to get Vendedora Model list from database", helpLink:e.HelpLink, 
                                   errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                return vendedoraModelsList;
            }
        }
        public VendedoraModel GetById(int vendedoraId)
        {
            VendedoraModel vendedoraModel = new VendedoraModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool MatchingRecordFound = false;
            string sql = "SELECT VendedoraId, Nome, Cpf, Rg, RgEmissor, DataNascimento, Email, NomePai, NomeMae, NomeConjuge, Logradouro, " +
                         " Numero, Complemento, Cep, UfRgId, RotaId, EstadoCivilId, EstadoId, CidadeId, BairroId " +
                         " FROM Vendedoras " +
                         " WHERE VendedoraId = @VendedoraId";
            using (SQLiteConnection sQLiteConnection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    sQLiteConnection.Open();

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, sQLiteConnection))
                    {
                        cmd.CommandText = sql;
                        cmd.Prepare();
                        cmd.Parameters.Add(new SQLiteParameter("@VendedoraId", vendedoraId));

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            MatchingRecordFound = reader.HasRows;
                            while (reader.Read())
                            {
                                vendedoraModel.VendedoraId = Int32.Parse(reader["VendedoraId"].ToString());
                                vendedoraModel.Nome = reader["Nome"].ToString();
                                vendedoraModel.Cpf = reader["Cpf"].ToString();
                                vendedoraModel.Rg = reader["Rg"].ToString();
                                vendedoraModel.RgEmissor = reader["RgEmissor"].ToString();
                                vendedoraModel.DataNascimento = DateTime.Parse(reader["DataNascimento"].ToString()) ;
                                vendedoraModel.Email = reader["Email"].ToString();
                                vendedoraModel.NomePai = reader["NomePai"].ToString();
                                vendedoraModel.NomeMae = reader["NomeMae"].ToString();
                                vendedoraModel.NomeConjuge = reader["NomeConjuge"].ToString();
                                vendedoraModel.Logradouro = reader["Logradouro"].ToString();
                                vendedoraModel.Numero = reader["Numero"].ToString();
                                vendedoraModel.Complemento = reader["Complemento"].ToString();
                                vendedoraModel.Cep = reader["Cep"].ToString();
                                vendedoraModel.UfRgId = Int32.Parse(reader["UfRgId"].ToString());
                                vendedoraModel.EstadoCivilId = Int32.Parse(reader["EstadoCivilId"].ToString());
                                vendedoraModel.EstadoId = Int32.Parse(reader["EstadoId"].ToString());
                                vendedoraModel.CidadeId = Int32.Parse(reader["CidadeId"].ToString());
                                vendedoraModel.BairroId = Int32.Parse(reader["BairroId"].ToString());
                                vendedoraModel.RotaId = Int32.Parse(reader["RotaId"].ToString());


                            }
                        }
                        sQLiteConnection.Close();
                    }
                }
                catch (SQLiteException e)
                {

                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                   customMessage: "Unable to get Department Model list from database", helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);


                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    
                }

                if (!MatchingRecordFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                        customMessage: $"Record not found., Unable to get Vendedora MOdel for Vendedora ID {vendedoraId}. Id {vendedoraId} " +
                        $"does not exist in the database",
                        helpLink: "", errorCode: 0, stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                   
                }
                return vendedoraModel;
            }
        }
        public void Add(IVendedoraModel vendedoraModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            using (SQLiteConnection sQLiteConnection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    sQLiteConnection.Open();
                }
                catch (SQLiteException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                   customMessage: "Unable to add VendedoraModel. Could not open a database connection", helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);


                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string SqlText =
                    "INSERT INTO Vendedoras (Nome, Cpf, Rg, RgEmissor, DataNascimento, Email, NomePai, NomeMae, NomeConjuge, Logradouro, Numero, Complemento, " +
                    "Cep, UfRgId, EstadoCivilId, EstadoId, CidadeId, BairroId ) " +
                    "VALUES (@Nome, @Cpf, @Rg, @RgEmissor, @DataNascimento, @Email, @NomePai, @NomeMae, @NomeConjuge, @Logradouro, @Numero, @Complemento, " +
                    "@Cep, @UfRgId, @EstadoCivilId, @EstadoId, @CidadeId, @BairroId) ";

                using (SQLiteCommand cmd = new SQLiteCommand(sQLiteConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, vendedoraModel, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.Add);
                    }
                    catch (DataAccessException ex)
                    {
                        ex.DataAccessStatusInfo.CustomMessage = "Vendedora model could not be added becouse it is already in the database.";
                        ex.DataAccessStatusInfo.ExceptionMessage = ex.Message;
                        ex.DataAccessStatusInfo.StackTrace = ex.StackTrace;
                        throw ex;
                    }

                    cmd.CommandText = SqlText;

                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Nome", vendedoraModel.Nome);
                    cmd.Parameters.AddWithValue("@Cpf", vendedoraModel.Cpf);
                    cmd.Parameters.AddWithValue("@Rg", vendedoraModel.Rg);
                    cmd.Parameters.AddWithValue("@RgEmissor", vendedoraModel.RgEmissor);
                    cmd.Parameters.AddWithValue("@DataNascimento", vendedoraModel.DataNascimento);
                    cmd.Parameters.AddWithValue("@Email", vendedoraModel.Email);
                    cmd.Parameters.AddWithValue("@NomePai", vendedoraModel.NomePai);
                    cmd.Parameters.AddWithValue("@NomeMae", vendedoraModel.NomeMae);
                    cmd.Parameters.AddWithValue("@NomeConjuge", vendedoraModel.NomeConjuge);
                    cmd.Parameters.AddWithValue("@Logradouro", vendedoraModel.Logradouro);
                    cmd.Parameters.AddWithValue("@Numero", vendedoraModel.Numero);
                    cmd.Parameters.AddWithValue("@Complemento", vendedoraModel.Complemento);
                    cmd.Parameters.AddWithValue("@Cep", vendedoraModel.Cep);
                    cmd.Parameters.AddWithValue("@UfRgId", vendedoraModel.UfRgId);
                    cmd.Parameters.AddWithValue("@EstadoCivilId", vendedoraModel.EstadoCivilId);
                    cmd.Parameters.AddWithValue("@EstadoId", vendedoraModel.EstadoId);
                    cmd.Parameters.AddWithValue("@CidadeId", vendedoraModel.CidadeId);
                    cmd.Parameters.AddWithValue("@BairroId", vendedoraModel.BairroId);
                    //cmd.Parameters.AddWithValue("@RotaId", vendedoraModel.RotaId); //deve ser incluído somente depois de cadastrado a vanededora.

                   

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                   customMessage: "Unable to Add VendedoraModel", helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);


                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    try //Confirm the Vendedora Model was added to the database
                    {
                        RecordExistsCheck(cmd, vendedoraModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.ConfirmAdd);
                    }
                    catch (DataAccessException ex)
                    {
                        ex.DataAccessStatusInfo.Status = "Error";
                        ex.DataAccessStatusInfo.OperationSucceeded = false;
                        ex.DataAccessStatusInfo.CustomMessage = "Failed to find vendedora model in database after add operation completed.";
                        ex.DataAccessStatusInfo.ExceptionMessage = ex.Message;
                        ex.DataAccessStatusInfo.StackTrace = ex.StackTrace;

                        throw ex;
                    }
                    sQLiteConnection.Close();
                }
            }
        }
        public void Update(IVendedoraModel vendedoraModel)
        {
            int result = -1;
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            using (SQLiteConnection sQLiteConnection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    sQLiteConnection.Open();
                }
                catch (SQLiteException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                   customMessage: "Unable to update Vendedora Model. Could not open a database connection", helpLink: e.HelpLink, 
                                   errorCode: e.ErrorCode, stackTrace: e.StackTrace);


                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string updateSql =
                    "UPDATE Vendedoras " +
                    "SET Nome = @Nome, Cpf = @Cpf, Rg = @Rg, RgEmissor = @RgEmissor, DataNascimento = @DataNascimento, " +
                    " Email = @Email, NomePai = @NomePai, NomeMae = @NomeMae, NomeConjuge = @NomeConjuge, Logradouro = @Logradouro, " +
                    " Numero = @Numero, Complemento = @Complemento, Cep = @Cep, UfRgId = @UfRgId, EstadoCivilId = @EstadoCivilId, " +
                    " EstadoId = @EstadoId, CidadeId = @CidadeId, BairroId = @BairroId, RotaId = @RotaId " +
                    " WHERE VendedoraId = @VendedoraId";

                using (SQLiteCommand cmd = new SQLiteCommand(sQLiteConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, vendedoraModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.Update);
                    }
                    catch (DataAccessException ex)
                    {
                        ex.DataAccessStatusInfo.CustomMessage = "Vendedora Model could not be updated becouse it is not in the database.";
                        ex.DataAccessStatusInfo.ExceptionMessage = ex.Message;
                        ex.DataAccessStatusInfo.StackTrace = ex.StackTrace;
                        throw ex;
                    }

                    cmd.CommandText = updateSql;

                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@VendedoraId", vendedoraModel.VendedoraId);
                    cmd.Parameters.AddWithValue("@Nome", vendedoraModel.Nome);
                    cmd.Parameters.AddWithValue("@Cpf", vendedoraModel.Cpf);
                    cmd.Parameters.AddWithValue("@Rg", vendedoraModel.Rg);
                    cmd.Parameters.AddWithValue("@RgEmissor", vendedoraModel.RgEmissor);
                    cmd.Parameters.AddWithValue("@DataNascimento", vendedoraModel.DataNascimento);
                    cmd.Parameters.AddWithValue("@Email", vendedoraModel.Email);
                    cmd.Parameters.AddWithValue("@NomePai", vendedoraModel.NomePai);
                    cmd.Parameters.AddWithValue("@NomeMae", vendedoraModel.NomeMae);
                    cmd.Parameters.AddWithValue("@NomeConjuge", vendedoraModel.NomeConjuge);
                    cmd.Parameters.AddWithValue("@Logradouro", vendedoraModel.Logradouro);
                    cmd.Parameters.AddWithValue("@Numero", vendedoraModel.Numero);
                    cmd.Parameters.AddWithValue("@Complemento", vendedoraModel.Complemento);
                    cmd.Parameters.AddWithValue("@Cep", vendedoraModel.Cep);
                    cmd.Parameters.AddWithValue("@UfRgId", vendedoraModel.UfRgId);
                    cmd.Parameters.AddWithValue("@EstadoCivilId", vendedoraModel.EstadoCivilId);
                    cmd.Parameters.AddWithValue("@EstadoId", vendedoraModel.EstadoId);
                    cmd.Parameters.AddWithValue("@CidadeId", vendedoraModel.CidadeId);
                    cmd.Parameters.AddWithValue("@BairroId", vendedoraModel.BairroId);
                    cmd.Parameters.AddWithValue("@RotaId", vendedoraModel.RotaId); 
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException e)
                    {

                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                   customMessage: "Unable to Execute update Vendedora Model", helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);


                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                }
                sQLiteConnection.Close();
            }
        }
        public void Delete(IVendedoraModel vendedoraModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            using (SQLiteConnection sQLiteConnection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    sQLiteConnection.Open();
                }
                catch (SQLiteException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage:e.Message,
                                    customMessage: "Unable to Delete VendedoraModel. Could not open a database connection", helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);


                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string sqlDeleteText = "DELETE FROM Vendedoras WHERE VendedoraId = @VendedoraId";
                using (SQLiteCommand cmd = new SQLiteCommand(sQLiteConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, vendedoraModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.Delete);
                    }
                    catch (DataAccessException ex)
                    {
                        ex.DataAccessStatusInfo.CustomMessage = "Vendedora model could not be deleted becouse it could not be found int the database.";
                        ex.DataAccessStatusInfo.ExceptionMessage = ex.Message;
                        ex.DataAccessStatusInfo.StackTrace = ex.StackTrace;
                        throw ex;
                    }

                    cmd.CommandText = sqlDeleteText;

                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@VendedoraId", vendedoraModel.VendedoraId);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                    customMessage: "Erro", helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    try //Confirm the Vendedora Model was deleted from the database
                    {
                        RecordExistsCheck(cmd, vendedoraModel, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.ConfirmDelete);
                    }
                    catch (DataAccessException ex)
                    {

                        ex.DataAccessStatusInfo.Status = "Error";
                        ex.DataAccessStatusInfo.OperationSucceeded = false;
                        ex.DataAccessStatusInfo.CustomMessage = "Failed to delete Vendedora Model in database.";
                        ex.DataAccessStatusInfo.ExceptionMessage = ex.Message;
                        ex.DataAccessStatusInfo.StackTrace = ex.StackTrace;

                        throw ex;
                    }
                    sQLiteConnection.Close();
                }
            }
        }
        private bool RecordExistsCheck(SQLiteCommand cmd, IVendedoraModel vendedoraModel, TypeOfExistenceCheck typeOfExistenceCheck, 
            RequestType requestType)
        {
            Int32 countOfRecsFound = 0;
            bool RecordExistsCheckPassed = true;

            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            cmd.Prepare();

            if ((requestType == RequestType.Add) || (requestType == RequestType.ConfirmAdd))
            {
                cmd.CommandText = "Select count(*) from Vendedoras where Cpf=@Cpf";
                cmd.Parameters.AddWithValue("@Cpf", vendedoraModel.Cpf);

            }
            else if (requestType == RequestType.Update || (requestType == RequestType.ConfirmDelete) || (requestType == RequestType.Delete))
            {
                cmd.CommandText = "Select count(*) from Vendedoras where VendedoraId = @VendedoraId";
                cmd.Parameters.AddWithValue("@VendedoraId", vendedoraModel.VendedoraId);
            }

            try
            {
                countOfRecsFound = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SQLiteException e)
            {
                string msg = e.Message;
                throw;
            }

            if ((typeOfExistenceCheck == TypeOfExistenceCheck.DoesNotExistInDB) && (countOfRecsFound > 0))
            {
                dataAccessStatus.Status = "Error";
                RecordExistsCheckPassed = false;

                throw new DataAccessException(dataAccessStatus);
            }
            else if ((typeOfExistenceCheck == TypeOfExistenceCheck.DoesExistInDB) && (countOfRecsFound == 0))
            {
                dataAccessStatus.Status = "Error";
                RecordExistsCheckPassed = false;
                throw new DataAccessException(dataAccessStatus);
            }
            return RecordExistsCheckPassed;
        }
    }
}