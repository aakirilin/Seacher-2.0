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
    }
}
