using Avalonia;
using Avalonia.Controls;
using Seacher.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Commons
{
    public class DBTableSettings
    {
        public string Name { get; set; }
        public List<DBColumnSettings> Columns { get; set; }


        public DBTableSettings()
        {

        }

        public DBTableSettings(IEnumerable<DBTableColumnData> columns)
        {
            Name = columns.First().Table;
            Columns = columns.Select(c => new DBColumnSettings(c)).ToList();
        }

        public List<Control> CreateConditions()
        {
            var controls = new List<Control>();
            foreach (var column in Columns)
            {
                Control control = null;
                var length = column.Length > 0 ? column.Length : 10;

                switch (column.DataType.ToLower().Trim())
                {
                    case "smallint":
                        control = new TextBoxColumn()
                        {
                            MaxLength = length,
                            Name = column.Name.ToLower(),
                            ColumnName = column.Name,
                            DBName = Name,
                            Mask = new string('9', length)
                        }; break;
                    case "int":
                        control = new TextBoxColumn() { 
                            MaxLength = length, 
                            Name = column.Name.ToLower(),
                            ColumnName = column.Name,
                            DBName = Name,
                            Mask = new string('9', length)
                    }; break;
                    case "bigint":
                        control = new TextBoxColumn()
                        {
                            MaxLength = length,
                            Name = column.Name.ToLower(),
                            ColumnName = column.Name,
                            DBName = Name,
                            Mask = new string('9', length)
                        }; break;
                    case "char":
                        control = new TextBoxColumn(){
                            MaxLength = length,
                            Name = column.Name.ToLower(),
                            ColumnName = column.Name,
                            DBName = Name
                        }; break;
                    case "varchar":
                        control = new TextBoxColumn()
                        {
                            MaxLength = length,
                            Name = column.Name.ToLower(),
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

        public string CreateQerry(WrapPanel conditionsPanel)
        {
            var conditions = new List<string>();

            var mainTableAliace = "t0";
            var mainTableName = Name;

            var columns = Columns
                .Select((c, i) => $"{mainTableAliace}.{c.Name} as {mainTableAliace}_c{i}");

            return 
                $"""
                    select {string.Join(", ", columns)}
                    from {mainTableName} as {mainTableAliace}
                """;
        }
    }
}
