using Npgsql;
using System;
using System.Collections.Generic;

namespace Seacher.Commons
{
    public class PostgeSQLDB : IDB, IDisposable
    {
        private readonly string connectionsString;
        private readonly NpgsqlConnection connection;

        public PostgeSQLDB(string connectionsString)
        {
            this.connectionsString = connectionsString;

            connection = new NpgsqlConnection();
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
    }
}
