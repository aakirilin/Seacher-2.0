using Seacher.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Controlls
{
    public class ComboBoxItemTable : Avalonia.Controls.ComboBoxItem
    {
        public string DBName { get; set; }
        public DBTableSettings DBTable { get; set; }
    }
}
