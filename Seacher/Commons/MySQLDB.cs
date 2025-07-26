using Avalonia.Controls;
using MySql.Data.MySqlClient;
using Seacher.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
using Tmds.DBus.Protocol;

namespace Seacher.Commons
{
    public class MySQLDB : IDB, IDisposable
    {
        private readonly string connectionsString;
        private readonly MySqlConnection connection;

        public MySQLDB(string connectionsString)
        {
            this.connectionsString = connectionsString;

            connection = new MySqlConnection();
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
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public IEnumerable<DBTableSettings> GetTables()
        {
            Open();
            var command = new MySqlCommand(
                """
                SELECT 
                	col_u.TABLE_NAME,
                    col_u.COLUMN_NAME,
                    col.DATA_TYPE,
                    col.CHARACTER_MAXIMUM_LENGTH,
                    col_u.REFERENCED_TABLE_NAME,
                    col_u.REFERENCED_COLUMN_NAME
                FROM 
                	INFORMATION_SCHEMA.KEY_COLUMN_USAGE as col_u
                	left join INFORMATION_SCHEMA.COLUMNS as col on col_u.TABLE_NAME = col.TABLE_NAME and col_u.COLUMN_NAME = col.COLUMN_NAME 
                """, connection);
            var reader = command.ExecuteReader();

            var columns = new List<DBTableColumnData>();

            while (reader.Read())
            {
                var table = reader.IsDBNull(0) ? "" : reader.GetString(0);
                var column = reader.IsDBNull(1) ? "" : reader.GetString(1);
                var dataType = reader.IsDBNull(2) ? "" : reader.GetString(2);
                var length = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                var referencedTableName = reader.IsDBNull(4) ? "" : reader.GetString(4);
                var referencedColumnName = reader.IsDBNull(5) ? "" : reader.GetString(5);

                columns.Add(new DBTableColumnData() { 
                    Table = table, 
                    Column = column, 
                    DataType = dataType, 
                    Length = length,
                    ReferencedTableName = referencedTableName,
                    ReferencedColumnName = referencedColumnName,
                });
            }

            Close();

            return columns
                .GroupBy(c => c.Table)
                .Select(columns => new DBTableSettings(columns));
        }

        public IEnumerable<string[]> SelectQerry(string qerry)
        {
            Open();

            var command = new MySqlCommand(qerry, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var row = new string[reader.FieldCount];
                for (int c = 0; c < reader.FieldCount; c++)
                {
                    if (!reader.IsDBNull(c))
                    {
                        var type = reader.GetFieldType(c).Name.ToLower();
                        switch (type)
                        {
                            case "string": row[c] = reader.GetString(c); break;
                            case "int64": row[c] = reader.GetInt64(c).ToString(); break;
                            case "int16": row[c] = reader.GetInt16(c).ToString(); break;
                            case "int32": row[c] = reader.GetInt32(c).ToString(); break;
                        }
                    }
                }

                yield return row;
            }

            Close();
        }



        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
