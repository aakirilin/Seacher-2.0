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

        public List<DBColumnSettings> GetTableColumns(string tablesName)
        {
            var table = GetTable(tablesName);
            return table.Columns;
        }


        public List<QerryCreatorForeignKey> GetForeignKey(string mainTableName)
        {
            var mainTable = GetTable(mainTableName);

            var result = mainTable.Columns
                .Where(c => !String.IsNullOrWhiteSpace(c.ReferencedTableName))
                .Select(c => new QerryCreatorForeignKey(GetTable(c.ReferencedTableName), c))
                .ToList();

            return result;
        }

        public List<QerryCreatorTableColumn> GetAllColumns(string mainTableName)
        {
            var mainTable = GetTable(mainTableName);

            var result = new List<QerryCreatorTableColumn>();

            var mainTableColumns = mainTable.Columns                
                .Select(c => new QerryCreatorTableColumn(mainTableName, c, null));

            var joinedColumns = mainTable.Columns
                .Where(c => !String.IsNullOrWhiteSpace(c.ReferencedTableName))
                .ToDictionary(c=> c, c => GetTableColumns(c.ReferencedTableName))
                .SelectMany(c => c.Value.Select(c1 => new QerryCreatorTableColumn(c.Key.ReferencedTableName, c1, c.Key)));

            result.AddRange(mainTableColumns);
            result.AddRange(joinedColumns);

            return result;
        }

        public List<string> GetTableJoinedColumns(string tablesName)
        {
            var aliace = GetTableAliace(tablesName);
            var table = GetTable(tablesName);
            var columns = table.Columns
                .Where(c => c.ShowInData)
                .Select((c, i) => $"{aliace}.{c.Name} as {aliace}_c{i}");

            return columns.ToList();
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
                .Select(c => $"{GetTableAliace(c.DBName)}.{c.ColumnName} like '%{c.Text}%'");

            var condition = String.Join(" and ", conditions);
            condition = String.IsNullOrWhiteSpace(condition) ? String.Empty : "where " + condition;

            var mainTableAliace = GetTableAliace(mainTableName);
            var mainTable = GetTable(mainTableName);

            //var columns = GetTableJoinedColumns(mainTableName);

            var joinedTables = mainTable
                .Columns
                .Where(c => !String.IsNullOrWhiteSpace(c.ReferencedTableName))
                .Select(c => 
                $"""
                    left join {c.ReferencedTableName} as {GetTableAliace(c.ReferencedTableName)} 
                    on {mainTableAliace}.{c.Name} = {GetTableAliace(c.ReferencedTableName)}.{c.ReferencedColumnName}
                 """);

            var columns = GetAllColumns(mainTableName)
                .Where(c => c.Column.ShowInData)
                .Select((c, i) => $"{GetTableAliace(c.TableName)}.{c.Column.Name} as {GetTableAliace(c.TableName)}_c{i}");

            return
                $"""
                    select {string.Join(", ", columns)}
                    from {mainTableName} as {mainTableAliace}
                    {string.Join(Environment.NewLine, joinedTables)}
                    {condition}
                """;
        }
    }
}
