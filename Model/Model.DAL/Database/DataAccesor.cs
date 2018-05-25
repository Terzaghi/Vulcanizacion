using LoggerManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using Common.Database;
using Model.SqlServer;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Model.DAL.Database
{
    public class DataAccesor : IDataAccessor
    {
        ILogger log = LogFactory.GetLogger(typeof(DataAccesor));

        private string _cadenaConexion = "";

        private IDataAccessor accesor;

        public DataAccesor()
        {
            try
            {
                if (ConfigurationManager.ConnectionStrings.Count > 0)
                {
                    // La 0 la crea el por defecto 
                    _cadenaConexion = ConfigurationManager.ConnectionStrings[1].ConnectionString;
                    accesor = new Accesor(_cadenaConexion);
                }
            }
            catch (Exception e)
            {
                log.Error("DataAccessor()", e);
            }
        }

        public DataAccesor(string nameConnectionString)
        {
            try
            {
                string s = ConfigurationManager.ConnectionStrings[nameConnectionString].ConnectionString;

                if (s.Length > 0)
                    _cadenaConexion = s;
            }
            catch (Exception e)
            {
                log.Error(
                    string.Format("DataAccessor({0})", string.IsNullOrEmpty(nameConnectionString) ? "null" : nameConnectionString),
                    e);
            }
          
        }

        public int ExecuteNonQuery(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false)
        {
            return this.accesor.ExecuteNonQuery(sql, parameters, IsProcedure);
        }

     

        public object ExecuteNonQueryWithResult(string sql, List<IDataParameter> parameters, bool IsProcedure = false)
        {
            return this.accesor.ExecuteNonQueryWithResult(sql, parameters, IsProcedure);
        }

      

        public object ExecuteScalar(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false)
        {
            throw new NotImplementedException();
        }

        public DataSet FillDataSet(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false)
        {
           return accesor.FillDataSet(sql, parameters, IsProcedure);
        }

        public IDataParameter Parameter(string parameterName, object parameterValue, ParameterDirection? direction=null)
        {
           
                return accesor.Parameter(parameterName, parameterValue, direction);    
        }

        public string ParameterIdentifierCharacter()
        {
            return this.accesor.ParameterIdentifierCharacter();
        }
        public string sqlGetNewIdentity(string idField, string positionParameter)
        {
            return accesor.sqlGetNewIdentity(idField, positionParameter);
        }
    }
}
