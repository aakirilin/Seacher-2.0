using Avalonia.Controls;
using Seacher.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Commons
{
    public class QerryCreator
    {
        private readonly DBSettings db;
        private readonly WrapPanel conditionsPanel;

        private List<DBTableSettings> tables => db.DBTables;


        public QerryCreator(DBSettings db, WrapPanel conditionsPanel)
        {
            this.db = db;
            this.conditionsPanel = conditionsPanel;
        }

        public DBTableSettings GetTable(string name)
        {
            return tables.First(t => t.Name.Equals(name));
        }

        public DBTableSettings GetTables(int index)
        {
            return tables.ElementAt(index);
        }

        public string GetTableAliace(string tablesName)
        {
            var index = tables.IndexOfElement(t => t.Name.Equals(tablesName));
            return $"t{index}";
        }

        public string GetTableName(int index)
        {
            return tables.ElementAt(index).Name;
        }

        public string Create(string mainTableName)
        {
            var grid = conditionsPanel
                .Children
                .OfType<Border>().Select(b => b.Child as Grid);

            var conditions = grid.SelectMany(g => g.Children)
                .OfType<TextBoxColumn>()
                .Select(t => new { Text = t.Text?.Trim().Trim('_') ?? "", t.DBName, t.ColumnName })
                .Where(t => !String.IsNullOrWhiteSpace(t.Text))
                .Select(c => $"{c.DBName}.{c.ColumnName} like '%{c.Text}%'");

            var condition = String.Join(" and ", conditions);
            condition = String.IsNullOrWhiteSpace(condition) ? String.Empty : "where " + condition;

            var mainTableAliace = GetTableAliace(mainTableName);
            var mainTable = GetTable(mainTableName);

            var columns = mainTable.Columns
                .Where(c => c.ShowInData)
                .Select((c, i) => $"{mainTableAliace}.{c.Name} as {mainTableAliace}_c{i}");

            return
                $"""
                    select {string.Join(", ", columns)}
                    from {mainTableName} as {mainTableAliace}
                    {condition}
                """;
        }
    }
}
