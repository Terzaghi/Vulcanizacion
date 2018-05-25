using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Common.Database
{
    public interface IDataAccessor
    {
        IDataParameter Parameter(string parameterName, object parameterValue, ParameterDirection? direction=null);
        string ParameterIdentifierCharacter();
        string sqlGetNewIdentity(string idField, string positionParameter);
        int ExecuteNonQuery(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false);
        object ExecuteNonQueryWithResult(string sql, List<IDataParameter> parameters, bool IsProcedure = false);
        object ExecuteScalar(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false);
        DataSet FillDataSet(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false);
    }
}
