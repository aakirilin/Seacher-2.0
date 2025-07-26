using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Seacher.Commons
{
    public class MsSQLDB : IDB, IDisposable
    {
        private readonly string connectionsString;
        private readonly SqlConnection connection;

        public MsSQLDB(string connectionsString)
        {
            this.connectionsString = connectionsString;

            connection = new SqlConnection();
            connection.ConnectionString = connectionsString;

        }
        public void Close()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public void Open()
        {
            connection.Open();
        }

        public IEnumerable<DBTableSettings> GetTables()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public IEnumerable<string[]> SelectQerry(string qerry)
        {
            throw new NotImplementedException();
        }
    }
}
