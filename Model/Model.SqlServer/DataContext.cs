using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

namespace Model.SqlServer
{
    public class DataContext : System.IDisposable
    {
        public SqlConnection Context { get; private set; }

        public string User { get; private set; }
        public string Password { get; private set; }
        public string Source { get; private set; }

        public string ConnectionString { get; private set; }

        public DataContext(string pConnectionString)
        {
            ConnectionString = pConnectionString;

            Context = new SqlConnection(pConnectionString);

            var builder = new SqlConnectionStringBuilder(Context.ConnectionString);

            User = builder.UserID;
            Password = builder.Password;
            Source = builder.DataSource;

            Context.Open();
        }

        public void Dispose()
        {
            Context.Close();
        }
    }
}
