using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Seacher.Commons
{


    public class DBSettings: IDisposable
    {
        [XmlIgnore]
        public IDB db;
        public string Name { get; set; }
        public string ConnectionStrings { get; set; }
        public string SelectDBMS { get; set; }


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

            CreateDB();
        }

        private void Open()
        {
            if (db == null) 
            {
                CreateDB();
            }
            db.Open();
        }

        private void Close()
        {
            db?.Close();
        }

        public void CreateDB()
        {
            IDB db = null;

            switch (SelectDBMS)
            {
                case "MySQL": db = new MySQLDB(ConnectionStrings); break;
                case "PostgeSQL": db = new PostgeSQLDB(ConnectionStrings); break;
                case "MSSQL": db = new MySQLDB(ConnectionStrings); break;
                default: throw new NotImplementedException($"The value ({SelectDBMS}) is not Implemented");
            }

            this.db = db;
        }

        public string TestConnection()
        {
            try
            {
                Open();
                return "test is successfully";
            }
            catch (Exception ex)
            {
                return "test is not successfully: " + ex.Message;
            }
            finally
            {
                Close();
            }
        }

        public void FillFromDB()
        {
            try
            {
                Open();
                DBTables = db.GetTables().ToList();
            }
            finally
            {
                Close();
            }
        }

        public IEnumerable<object> SelectQerry(Type type, string qerry)
        {
            try
            {
                Open();
                return db.SelectQerry(type, qerry);
            }
            finally
            {
                Close();
            }
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
