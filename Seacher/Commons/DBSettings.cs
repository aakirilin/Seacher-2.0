using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Commons
{


    public class DBSettings: IDisposable
    {
        public string Name { get; set; }
        public string ConnectionStrings { get; set; }
        public string SelectDBMS { get; set; }

        public IDB db;

        public List<DBTableSettings> DBTables { get; set; }

        public DBSettings() 
        {
            DBTables = new List<DBTableSettings>();
        }

        public DBSettings(AddDBWindow addDBWindow) :this()
        {
            Name = addDBWindow.DBName.Text;
            ConnectionStrings = addDBWindow.DBCS.Text;
            var selectedDBMSItem = addDBWindow.SelectDBMS.SelectedItem as ComboBoxItem;
            SelectDBMS = selectedDBMSItem?.Content?.ToString() ?? "";

            this.db = CreateDB();
        }

        public IDB CreateDB()
        {
            IDB db = null;

            switch (SelectDBMS)
            {
                case "MySQL": db = new MySQLDB(ConnectionStrings); break;
                case "PostgeSQL": db = new PostgeSQLDB(ConnectionStrings); break;
                case "MSSQL": db = new MySQLDB(ConnectionStrings); break;
                default: throw new NotImplementedException($"The value ({SelectDBMS}) is not Implemented");
            }

            return db;
        }

        public string TestConnection()
        {
            try
            {
                db.Open();
                return "test is successfully";
            }
            catch (Exception ex)
            {
                return "test is not successfully: " + ex.Message;
            }
            finally
            {
                db.Close();
            }
        }

        public void FillFromDB()
        {
            try
            {
                db.Open();
                DBTables = db.GetTables().ToList();
            }
            finally
            {
                db.Close();
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
