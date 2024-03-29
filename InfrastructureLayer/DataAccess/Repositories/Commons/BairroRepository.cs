﻿using CommonComponents;

using DomainLayer.Models.CommonModels.Address;

using ServiceLayer.Services.BairroServices;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace InfrastructureLayer.DataAccess.Repositories.Commons
{
    public class BairroRepository : IBairroRepository
    {
        private string _connectionString;
        enum TypeOfExistenceCheck
        {
            DoesExistInDb,
            DoesNotExsitInDb
        }

        enum RequestType
        {
            Add,
            Update,
            Delete,
            Read,
            ConfirmAdd,
            ConfirmDelete
        }
        public BairroRepository()
        {

        }

        public BairroRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BairroModel Add(IBairroModel bairroModel)
        {
            int idReturned = 0;
            BairroModel bairroAdded = new BairroModel();

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = " INSERT INTO Bairros (Nome, CidadeId) " +
                           " OUTPUT INSERTED.BairroId" +
                           " VALUES (@Nome, @CidadeId) ";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        
                        cmd.Prepare();

                        cmd.Parameters.AddWithValue("@Nome", bairroModel.Nome);
                        cmd.Parameters.AddWithValue("@CidadeId", bairroModel.CidadeId);

                        try
                        {
                            idReturned = (int)cmd.ExecuteScalar();
                            bairroAdded = GetById(idReturned);
                        }
                        catch (SqlException e)
                        {
                            dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                             customMessage: "Unable to Execute Add BairroModel", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                             stackTrace: e.StackTrace);

                            throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                        }

                        
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Execute Add BairroModel", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
                
                return bairroAdded;
            }
        }

        private void RecordExistsCheck(IBairroModel bairroModel, TypeOfExistenceCheck typeOfExistenceCheck,
            RequestType requestType)
        {
            int countOfRecsFound = 0;
            DataAccessStatus dataAccess = new DataAccessStatus();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("", connection))
                    {
                        

                        DataAccessStatus dataAccessStatus = new DataAccessStatus();
                        cmd.Prepare();

                        if ((requestType == RequestType.Add) || (requestType == RequestType.ConfirmAdd))
                        {
                            cmd.CommandText = "SELECT COUNT(*) FROM Bairros WHERE Nome=@Nome AND CidadeId = @CidadeId";
                            cmd.Parameters.AddWithValue("@Nome", bairroModel.Nome);
                            cmd.Parameters.AddWithValue("@CidadeId", bairroModel.CidadeId);

                        }
                        else if ((requestType == RequestType.Update) || (requestType == RequestType.ConfirmDelete) || (requestType == RequestType.Delete))
                        {
                            cmd.CommandText = "SELECT COUNT(*) FROM Bairros WHERE BairroId = @BairroId";
                            cmd.Parameters.AddWithValue("@BairroId", bairroModel.BairroId);
                        }


                        if ((typeOfExistenceCheck == TypeOfExistenceCheck.DoesNotExsitInDb) && (countOfRecsFound > 0))
                        {
                            dataAccessStatus.Status = "Error";
                            throw new DataAccessException(dataAccessStatus);
                        }
                        else if ((typeOfExistenceCheck == TypeOfExistenceCheck.DoesExistInDb) && (countOfRecsFound == 0))
                        {
                            dataAccessStatus.Status = "Error";
                            throw new DataAccessException(dataAccessStatus);
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccess.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to Check if exist register. Colud not open the database connection", helpLink: e.HelpLink,
                       errorCode: e.ErrorCode, stackTrace: e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccess);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void Delete(IBairroModel bairroModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "DELETE FROM Bairros HWERE BairroId = BairroId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        try
                        {
                            RecordExistsCheck(bairroModel, TypeOfExistenceCheck.DoesExistInDb, RequestType.Delete);
                        }
                        catch (DataAccessException e)
                        {
                            e.DataAccessStatusInfo.CustomMessage = "Bairro Model could not be Deleted. BairroModel is not found ind the DataBase.";
                            e.DataAccessStatusInfo.ExceptionMessage = e.Message;
                            e.DataAccessStatusInfo.StackTrace = e.StackTrace;
                            throw e;
                        }

                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@BairroId", bairroModel.BairroId);

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException e)
                        {
                            dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                        customMessage: "Uneble to Execute Delete", helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);
                            throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                        }

                        try
                        {
                            RecordExistsCheck(bairroModel, TypeOfExistenceCheck.DoesNotExsitInDb, RequestType.ConfirmDelete);
                        }
                        catch (DataAccessException e)
                        {
                            e.DataAccessStatusInfo.Status = "Error";
                            e.DataAccessStatusInfo.OperationSucceeded = false;
                            e.DataAccessStatusInfo.CustomMessage = "Failed to delete BairroModel in database.";
                            e.DataAccessStatusInfo.ExceptionMessage = e.Message;
                            e.DataAccessStatusInfo.StackTrace = e.StackTrace;
                            throw e;
                        }

                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Delete RotaModel.", helpLink: e.HelpLink,
                        errorCode: e.ErrorCode, stackTrace: e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        public IEnumerable<IBairroModel> GetAll()
        {
            List<BairroModel> bairroModelList = new List<BairroModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = "SELECT * FROM Bairros";
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BairroModel bairroModel = new BairroModel();

                                bairroModel.BairroId = int.Parse(reader["BairroId"].ToString());
                                bairroModel.Nome = reader["Nome"].ToString();
                                bairroModel.CidadeId = int.Parse(reader["CidadeId"].ToString());

                                bairroModelList.Add(bairroModel);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Rota Model List from Database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
                return bairroModelList;
            }
        }

        public IEnumerable<IBairroModel> GetByCidadeId(int cidadeId)
        {
            string query = "SELECT * FROM Bairros WHERE CidadeId = @CidadeId ORDER BY Nome";
            
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            
            List<BairroModel> bairroModelList = new List<BairroModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@CidadeId", cidadeId));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BairroModel bairroModel = new BairroModel();
                                bairroModel.BairroId = int.Parse(reader["BairroId"].ToString());
                                bairroModel.Nome = reader["Nome"].ToString();
                                bairroModel.CidadeId = int.Parse(reader["CidadeId"].ToString());

                                bairroModelList.Add(bairroModel);

                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage:e.Message,
                        customMessage: "Unable to GetByCidadeId.", helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
                return bairroModelList;
            }
        }

        public BairroModel GetById(int id)
        {
            BairroModel bairroModel = new BairroModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool MatchingRecordFound = false;
            string query = "SELECT BairroId, Nome, CidadeId " +
                           " FROM Bairros " +
                           " WHERE BairroId = @BairroId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@BairroId", id));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            MatchingRecordFound = reader.HasRows;

                            while (reader.Read())
                            {
                                bairroModel.BairroId = int.Parse(reader["BairroId"].ToString());
                                bairroModel.Nome = reader["Nome"].ToString();
                                bairroModel.CidadeId = int.Parse(reader["CidadeId"].ToString());
                            }

                        }
                    }
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Bairro Model from DataBase", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!MatchingRecordFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                        customMessage: $"Record not found. Unable to get Bairro Model for BairroId {id}. Id {id} does not existe in database.",
                        helpLink: "", errorCode: 0, stackTrace: "");
                    throw new DataAccessException(dataAccessStatus);
                }
                return bairroModel;
            }

        }

        public void Update(IBairroModel bairroModel)
        {
            int result = -1;
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            string query = "UPDATE Bairros SET Nome = @Nome, CidadeId = @CidadeId WHERE BairroId = @BairroId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    try //VERIFICA SE O BAIRRO EXISTE
                    {
                        RecordExistsCheck(bairroModel, TypeOfExistenceCheck.DoesExistInDb, RequestType.Update);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Bairro Model could not be updated becouse it is not in the database.";
                        e.DataAccessStatusInfo.ExceptionMessage = e.Message;
                        e.DataAccessStatusInfo.StackTrace = e.StackTrace;

                        throw e;
                    }
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();

                        cmd.Parameters.AddWithValue("@BairroId", bairroModel.BairroId);
                        cmd.Parameters.AddWithValue("@Nome", bairroModel.Nome);
                        cmd.Parameters.AddWithValue("@CidadeId", bairroModel.CidadeId);
                        try
                        {
                            result = cmd.ExecuteNonQuery();
                        }
                        catch (SqlException e)
                        {
                            dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                                customMessage: "Unable to Execute Update BairroModel", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                                stackTrace: e.StackTrace);

                            throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                        }
                    }

                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public BairroModel GetByNomeAndCidadeId(string nome, int cidadeId)
        {
            BairroModel bairroModel = new BairroModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool MathcingRecordFound = false;
            string query = "SELECT BairroId, Nome, CidadeId FROM Bairros WHERE Nome = @Nome AND CidadeId = @CidadeId";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@Nome", nome));
                        cmd.Parameters.Add(new SqlParameter("@CidadeId", cidadeId));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            MathcingRecordFound = reader.HasRows;

                            while (reader.Read())
                            {
                                bairroModel.BairroId = int.Parse(reader["BairroId"].ToString());
                                bairroModel.Nome = reader["Nome"].ToString();
                                bairroModel.CidadeId = int.Parse(reader["CidadeId"].ToString());
                            }
                        }
                    }

                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Bairro Model from DataBase. GetIdByNomeAndCidadeId", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                finally
                {
                    connection.Close();
                }
            }
            return bairroModel;
        }
    }
}
