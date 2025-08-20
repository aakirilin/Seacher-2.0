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

        public List<QerryCreatorForeignKey> GetJoinedTables(string mainTableName)
        {
            var mainTable = GetTable(mainTableName);
            return mainTable.Columns
                .Where(c => !String.IsNullOrWhiteSpace(c.ReferencedTableName))
                .Select(c => new QerryCreatorForeignKey(tables.First(t => c.ReferencedTableName.Equals(t.Name)), c))
                .ToList();
        }

        public DBTableSettings GetTable(string name)
        {
            return tables.First(t => t.Name.Equals(name));
        }

        public DBTableSettings GetTable(int index)
        {
            return tables.ElementAt(index);
        }

        public string GetTableAliace(string tablesName)
        {
            var index = tables.IndexOfElement(t => t.Name.Equals(tablesName));
            return GetTableAliace(index, tablesName);
        }

        public string GetTableAliace(int index, string tablesName)
        {
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

            var columns = GetTableColumns(mainTableName)
                .Where(c => c.ShowInData)
                .Select((c, i) => new QerryCreatorTableColumn(0, i, mainTableName, c, null));

            var joinedColumns = GetJoinedTables(mainTableName)
                .SelectMany((f, i) => f.Table.SownColumns
                    .Select((c, j) => new QerryCreatorTableColumn(i + 1, j, f.ForeignKey.ReferencedTableName, c, f.ForeignKey)));

            result.AddRange(columns);
            result.AddRange(joinedColumns);

            return result;
        }

        /*
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
        */
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

            var mainTableAliace = GetTableAliace(0, mainTableName);
            var mainTable = GetTable(mainTableName);

            var joinedTables = GetJoinedTables(mainTableName)
                .Select((f, i) => 
                $"""
                    left join {f.Table.Name} as {GetTableAliace(i + 1 , f.Table.Name)} 
                    on {mainTableAliace}.{f.ForeignKey.Name} = {GetTableAliace(i + 1, f.Table.Name)}.{f.ForeignKey.ReferencedColumnName}
                """);

            var columns = GetAllColumns(mainTableName)
                .Where(c => c.Column.ShowInData)
                .Select(c => $"{GetTableAliace(c.TableIndex, c.TableName)}.{c.Column.Name} as {GetTableAliace(c.TableName)}_t{c.TableIndex}_c{c.ColumnIndex}");

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
