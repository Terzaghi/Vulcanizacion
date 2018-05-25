using Common.Database;
using LoggerManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Model.SqlServer
{
    public class Accesor : IDataAccessor
    {
        ILogger log = LogFactory.GetLogger(typeof(Accesor));
        private string _cadenaConexion = "";

        public Accesor(string pConnectionString)
        {
            this._cadenaConexion = pConnectionString;
        }
        public int ExecuteNonQuery(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false)
        {
            int resultado = 0;

            try
            {
                using (var context = new DataContext(_cadenaConexion))
                {


                    using (var cmd = new SqlCommand(sql, context.Context))
                    {
                        cmd.CommandType = IsProcedure ? CommandType.StoredProcedure : CommandType.Text;

                        if (parameters != null && parameters.Count > 0)
                        {
                            List<SqlParameter> listParameters = GetParametersForCurrentContext(parameters);
                            cmd.Parameters.AddRange(listParameters.ToArray());
                        }

                        resultado = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("ExecuteNonQuery()", ex);

                SaveParametersOnLog(sql, parameters);
            }
            return resultado;
        }

        public object ExecuteNonQueryWithResult(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false)
        {
            object resultado = null;

            DateTime fchTiempoRespuesta = DateTime.UtcNow;

            try
            {
                //Para hacer la petición, devolviendo al menos un resultado(widthResult), hace falta al menos un parámetro de salida
                if (parameters == null || parameters.Count == 0)
                {
                    log.Warning("ExecuteNonQueryWithResult(). Necesita al menos un parámetro de salida");
                    return resultado;
                }

                using (var context = new DataContext(_cadenaConexion))
                {


                    using (var cmd = new SqlCommand(sql, context.Context))
                    {
                        cmd.CommandType = IsProcedure ? CommandType.StoredProcedure : CommandType.Text;
                        //cmd.BindByName = true;


                        List<SqlParameter> listParameters = GetParametersForCurrentContext(parameters);
                        cmd.Parameters.AddRange(listParameters.ToArray());
                        
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            var returningParameter = listParameters.FirstOrDefault(a => a.Direction == ParameterDirection.Output);

                            if (returningParameter != null)
                            {
                                switch (returningParameter.SqlDbType)
                                {
                                    case SqlDbType.Decimal:
                                    case SqlDbType.Float:
                                    case SqlDbType.Int:
                                      if (cmd.Parameters[returningParameter.ParameterName].Value != DBNull.Value)
                                        {
                                            int id = 0;

                                            if (int.TryParse(cmd.Parameters[returningParameter.ParameterName].Value.ToString(), out id))
                                                resultado = id;
                                        }
                                        break;
                                    default:
                                        resultado = cmd.Parameters[returningParameter.ParameterName].Value != DBNull.Value ? cmd.Parameters[returningParameter.ParameterName].Value.ToString() : string.Empty;
                                        break;
                                }
                            }
                            else log.Warning("ExecuteNonQueryWithResult. No se ha obtenido el parámetro de retorno");
                        }
                        else log.Warning("ExecuteNonQueryWithResult. No se ha realizado la inserción");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("ExecuteNonQueryWithResult()", ex);

                SaveParametersOnLog(sql, parameters);
            }

            if (log.isDebugEnabled)
            {
                TimeSpan tsTiempoRespuesta = DateTime.UtcNow - fchTiempoRespuesta;
                log.Debug("ExecuteNonQueryWithResult(). Tiempo respuesta SQL: {0}", tsTiempoRespuesta.TotalSeconds);
            }

            return resultado;

        }

        public object ExecuteScalar(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false)
        {
            object resultado = null;

            DateTime fchTiempoRespuesta = DateTime.UtcNow;

            try
            {
                using (var context = new DataContext(_cadenaConexion))
                {
                    using (var cmd = new SqlCommand(sql, context.Context))
                    {
                        cmd.CommandType = IsProcedure ? CommandType.StoredProcedure : CommandType.Text;
                        //cmd.BindByName = true;

                        if (parameters != null && parameters.Count > 0) cmd.Parameters.AddRange(parameters.ToArray());

                        resultado = cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("ExecuteScalar()", ex);

                SaveParametersOnLog(sql, parameters);
            }

            if (log.isDebugEnabled)
            {
                TimeSpan tsTiempoRespuesta = DateTime.UtcNow - fchTiempoRespuesta;
                log.Debug("ExecuteScalar(). Tiempo respuesta SQL: {0}", tsTiempoRespuesta.TotalSeconds);
            }

            return resultado;
        }

        public DataSet FillDataSet(string sql, List<IDataParameter> parameters = null, bool IsProcedure = false)
        {
            DataSet ds = new DataSet();

            try
            {
                using (var context = new DataContext(_cadenaConexion))
                {
                    using (var cmd = new SqlCommand(sql, context.Context))
                    {
                        cmd.CommandType = IsProcedure ? CommandType.StoredProcedure : CommandType.Text;
                        //cmd.BindByName = true;

                        if (parameters != null && parameters.Count > 0)
                        {
                            List<SqlParameter> listParameters = GetParametersForCurrentContext(parameters);
                            cmd.Parameters.AddRange(listParameters.ToArray());
                        }

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("FillDataSet()", ex);

                SaveParametersOnLog(sql, parameters);
            }

            return ds;
        }

        public IDataParameter Parameter(string parameterName, object parameterValue, ParameterDirection? direction = null)
        {
            SqlParameter parameter = new SqlParameter("@" + parameterName, parameterValue);
            if (direction != null)
            {
                parameter.Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), direction.ToString());
            }
            return parameter;
        }

        public string ParameterIdentifierCharacter()
        {
            return "@";
        }

        public string sqlGetNewIdentity(string idField, string positionParameter)
        {
            return " set @" + positionParameter + " = SCOPE_IDENTITY();";
        }

        private void SaveParametersOnLog(string procedimiento, List<IDataParameter> Parametros)
        {
            try
            {
                log.Warning("Parámetros empleados en la petición. Consulta: {0}", procedimiento);
                string strParametros = "";
                if ((Parametros != null) && (Parametros.Count > 0))
                {
                    foreach (var p in Parametros)
                    {
                        if (p.Direction == ParameterDirection.Output)
                        {
                            strParametros += string.Format("{0} (Output parameter)", p.ParameterName);
                        }
                        else
                        {
                            strParametros += string.Format("{0}: {1} ", p.ParameterName, p.Value);
                        }
                    }
                    log.Warning(string.Format("Parámetros en la petición: {0}", strParametros));
                }
            }
            catch (Exception er)
            {
                log.Error("SaveParametersOnLog()", er);
            }
        }

        private List<SqlParameter> GetParametersForCurrentContext(List<IDataParameter> parameters)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            foreach (IDataParameter p in parameters)
            {
                list.Add((SqlParameter)p);

            }
            return list;
        }
    }
}
