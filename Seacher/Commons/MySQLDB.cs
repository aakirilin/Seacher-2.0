using Avalonia.Controls;
using MySql.Data.MySqlClient;
using Seacher.Controlls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            var schema = connectionsString
                .ToLower()
                .Split(';')
                .First(t => t.StartsWith("database"))
                .Split('=')
                .Last();

            var command = new MySqlCommand(
                $"""
                SELECT distinct
                	col.TABLE_NAME,
                    col.COLUMN_NAME,
                    col.DATA_TYPE,
                    col.CHARACTER_MAXIMUM_LENGTH,
                    col_u.REFERENCED_TABLE_NAME,
                    col_u.REFERENCED_COLUMN_NAME
                FROM 
                	INFORMATION_SCHEMA.COLUMNS as col
                	left join INFORMATION_SCHEMA.KEY_COLUMN_USAGE as col_u on col_u.TABLE_NAME = col.TABLE_NAME and col_u.COLUMN_NAME = col.COLUMN_NAME 
                where col.TABLE_SCHEMA = '{schema}'
                order by col.TABLE_NAME, col.COLUMN_NAME
                """, connection);
            var reader = command.ExecuteReader();

            var columns = new List<DBTableColumnData>();

            while (reader.Read())
            {
                var table = reader.IsDBNull(0) ? "" : reader.GetString(0);
                var column = reader.IsDBNull(1) ? "" : reader.GetString(1);
                var dataType = reader.IsDBNull(2) ? "" : reader.GetString(2);
                var length = reader.IsDBNull(3) ? 0 : reader.GetInt64(3);
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

            return columns
                .GroupBy(c => c.Table)
                .Select(columns => new DBTableSettings(columns));
        }

        public IEnumerable<object> SelectQerry(Type type, string qerry)
        {
            var command = new MySqlCommand(qerry, connection);
            var reader = command.ExecuteReader();
            var result = new List<object>();
            var properties = type.GetProperties();

            while (reader.Read())
            {
                var instance = Activator.CreateInstance(type);
                for (int c = 0; c < reader.FieldCount; c++)
                {
                    string value = null;
                    var isNull = reader.IsDBNull(c);
                    var typeField = reader.GetFieldType(c).Name.ToLower();
                    if (!isNull)
                    {
                        switch (typeField)
                        {
                            case "string": value = reader.GetString(c); break;
                            case "int64": value = reader.GetInt64(c).ToString(); break;
                            case "int16": value = reader.GetInt16(c).ToString(); break;
                            case "int32": value = reader.GetInt32(c).ToString(); break;
                            case "uint64": value = reader.GetUInt64(c).ToString(); break;
                            case "uint16": value = reader.GetUInt16(c).ToString(); break;
                            case "uint32": value = reader.GetUInt32(c).ToString(); break;
                            case "bool": value = reader.GetBoolean(c).ToString(); break;
                            case "datetime": value = reader.GetDateTime(c).ToString(); break;
                        }
                    }
                    var property = properties[c];
                    Debug.WriteLine($"{property.Name}-({typeField})-{isNull}-{value}-{reader.GetValue(c)}");
                    property.SetValue(instance, value);
                }

                result.Add(instance);
            }
            return result;
        }
        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
