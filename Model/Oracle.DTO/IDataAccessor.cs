using System.Collections.Generic;
using System.Data;

namespace Oracle.DTO
{
    public interface IDataAccessor
    {
        int ExecuteNonQuery(string sql, List<OracleParameter> parameters = null, bool IsProcedure = false);
        object ExecuteNonQueryWithResult(string sql, List<OracleParameter> parameters, bool IsProcedure = false);
        object ExecuteScalar(string sql, List<OracleParameter> parameters = null, bool IsProcedure = false);
        DataSet FillDataSet(string sql, List<OracleParameter> parameters = null, bool IsProcedure = false);
    }
}
