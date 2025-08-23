using Avalonia;
using Avalonia.Controls;
using Seacher.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Seacher.Commons
{
    public class DBTableSettings
    {
        public string Name { get; set; }
        public List<DBColumnSettings> Columns { get; set; }

        [XmlIgnore]
        public IEnumerable<DBColumnSettings> SownColumns
        {
            get => Columns.Where(c => c.ShowInData);
        }


        public DBTableSettings()
        {

        }

        public DBTableSettings(IEnumerable<DBTableColumnData> columns)
        {
            Name = columns.First().Table;
            Columns = columns.Select(c => new DBColumnSettings(c)).ToList();
        }

        public List<Control> CreateConditions(int tableIndex, string dbAliace)
        {
            var controls = new List<Control>();
            
            foreach (var column in Columns)
            {              
                if (!column.ShowInCondition)
                {
                    continue;
                }
                Control control = null;
                var length = column.Length > 0 ? (int)column.Length : 10;
                var name = $"{Name}_{column.Name}_{dbAliace}";

                switch (column.DataType.ToLower().Trim())
                {
                    case "smallint":
                        control = new TextBoxColumn()
                        {
                            MaxLength = length,
                            Name = name,
                            ColumnName = column.Name,
                            DBName = Name,
                            DBAliace = dbAliace,
                            Mask = new string('9', length)
                        }; break;
                    case "int":
                        control = new TextBoxColumn() { 
                            MaxLength = length, 
                            Name = name,
                            ColumnName = column.Name,
                            DBName = Name,
                            DBAliace = dbAliace,
                            Mask = new string('9', length)
                    }; break;
                    case "bigint":
                        control = new TextBoxColumn()
                        {
                            MaxLength = length,
                            Name = name,
                            ColumnName = column.Name,
                            DBName = Name,
                            DBAliace = dbAliace,
                            Mask = new string('9', length)
                        }; break;
                    case "char":
                        control = new TextBoxColumn(){
                            MaxLength = length,
                            Name = name,
                            DBAliace = dbAliace,
                            ColumnName = column.Name,
                            DBName = Name
                        }; break;
                    case "varchar":
                        control = new TextBoxColumn()
                        {
                            MaxLength = length,
                            Name = name,
                            DBAliace = dbAliace,
                            ColumnName = column.Name,
                            DBName = Name
                        }; break;
                        //default: throw new NotImplementedException($"This column type is not implemented ({column.DataType})");
                }

                if (control == null)
                {
                    continue;
                }
                control.Margin = new Thickness(0,25,0,0);

                var grid = new Grid();

                grid.Margin = new Thickness(5);
                grid.RowDefinitions = new RowDefinitions("Auto,*");
                grid.ColumnDefinitions = new ColumnDefinitions("Auto,*");
                grid.Children.Add(control);
                grid.Children.Add(new TextBlock() { Text = control.Name
                    
                });

                var border = new Border()
                {
                    ZIndex = 1,
                    Padding = new Thickness(5, 10, 5, 0),
                    Margin = new Thickness(5, 0, 5, 0),
                    Child = grid
                };

                controls.Add(border);
            }

            return controls;
        }

        public string CreateQerry(DBSettings db, WrapPanel conditionsPanel)
        {
            var qerryCreator = new QerryCreator(db, conditionsPanel);
            return qerryCreator.Create(Name);
        }
        public string CreateQerry(WrapPanel conditionsPanel)
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


            var mainTableAliace = Name;
            var mainTableName = Name;

            var columns = Columns
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
