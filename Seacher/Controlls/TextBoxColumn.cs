using Seacher.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Controlls
{
    public class TextBoxColumn : Avalonia.Controls.MaskedTextBox
    {
        public string ColumnName { get; set; }
        public string DBName { get; set; }
        public string DBAliace { get; set; }
    }
}
