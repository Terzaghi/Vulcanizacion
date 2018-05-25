using System;
using System.Collections;
using System.Data;

namespace Oracle.DTO
{
    public class OracleParameter
    {
        public string ParameterName { get; private set; }
        public ParameterDirection Direction { get; private set; }
        public OracleDbType OracleDbType { get; private set; }
        public object Value { get; private set; }

        public OracleParameter(string parameterName, object obj)
        {
            this.ParameterName = parameterName;
            this.Value = obj;

            this.Direction = ParameterDirection.Input;

            if (obj != null && obj != DBNull.Value)
            {
                Type type = obj.GetType();

                if (type == typeof(bool) || type == typeof(sbyte) || (type == typeof(ushort) || type == typeof(uint)) || type == typeof(ulong))
                    throw new ArgumentException();

                this.OracleDbType = (OracleDbType)OraDb_DbTypeTable.s_table[(object)type];
            }
            else
            {
                this.OracleDbType = DTO.OracleDbType.Varchar2;
            }
        }

        public OracleParameter(string parameterName, OracleDbType type, object obj, ParameterDirection direction)
        {
            this.ParameterName = parameterName;
            this.OracleDbType = type;
            this.Value = obj;
            this.Direction = direction;
        }

        public OracleParameter(string parameterName, OracleDbType type, ParameterDirection direction)
        {
            this.ParameterName = parameterName;
            this.OracleDbType = type;
            this.Direction = direction;
        }

        private class OraDb_DbTypeTable
        {
            internal static Hashtable s_table = new Hashtable(91);

            static OraDb_DbTypeTable()
            {
                OraDb_DbTypeTable.InsertTableEntries();
            }

            private static void InsertTableEntries()
            {
                OraDb_DbTypeTable.s_table.Add((object)typeof(byte), (object)Oracle.DTO.OracleDbType.Byte);
                OraDb_DbTypeTable.s_table.Add((object)typeof(byte[]), (object)Oracle.DTO.OracleDbType.Raw);
                OraDb_DbTypeTable.s_table.Add((object)typeof(char), (object)Oracle.DTO.OracleDbType.Varchar2);
                OraDb_DbTypeTable.s_table.Add((object)typeof(char[]), (object)Oracle.DTO.OracleDbType.Varchar2);
                OraDb_DbTypeTable.s_table.Add((object)typeof(DateTime), (object)Oracle.DTO.OracleDbType.TimeStamp);
                OraDb_DbTypeTable.s_table.Add((object)typeof(short), (object)Oracle.DTO.OracleDbType.Int16);
                OraDb_DbTypeTable.s_table.Add((object)typeof(int), (object)Oracle.DTO.OracleDbType.Int32);
                OraDb_DbTypeTable.s_table.Add((object)typeof(long), (object)Oracle.DTO.OracleDbType.Int64);
                OraDb_DbTypeTable.s_table.Add((object)typeof(float), (object)Oracle.DTO.OracleDbType.Single);
                OraDb_DbTypeTable.s_table.Add((object)typeof(double), (object)Oracle.DTO.OracleDbType.Double);
                OraDb_DbTypeTable.s_table.Add((object)typeof(Decimal), (object)Oracle.DTO.OracleDbType.Decimal);
                OraDb_DbTypeTable.s_table.Add((object)typeof(string), (object)Oracle.DTO.OracleDbType.Varchar2);
                OraDb_DbTypeTable.s_table.Add((object)typeof(TimeSpan), (object)Oracle.DTO.OracleDbType.IntervalDS);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleBFile), (object)OracleDbType.BFile);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleBinary), (object)OracleDbType.Raw);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleBlob), (object)OracleDbType.Blob);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleClob), (object)OracleDbType.Clob);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleDate), (object)OracleDbType.Date);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleDecimal), (object)OracleDbType.Decimal);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleIntervalDS), (object)OracleDbType.IntervalDS);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleIntervalYM), (object)OracleDbType.IntervalYM);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleRefCursor), (object)OracleDbType.RefCursor);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleString), (object)OracleDbType.Varchar2);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleTimeStamp), (object)OracleDbType.TimeStamp);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleTimeStampLTZ), (object)OracleDbType.TimeStampLTZ);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleTimeStampTZ), (object)OracleDbType.TimeStampTZ);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleXmlType), (object)OracleDbType.XmlType);
                //OraDb_DbTypeTable.s_table.Add((object)typeof(OracleRef), (object)OracleDbType.Ref);
            }
        }
    }
}
