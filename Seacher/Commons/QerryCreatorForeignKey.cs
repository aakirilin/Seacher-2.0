using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Commons
{
    public class QerryCreatorForeignKey
    {
        public int Index { get; set; }
        public string MainTableName { get; set; }
        public DBTableSettings Table { get; set; }
        public DBColumnSettings ForeignKey { get; set; }

        public QerryCreatorForeignKey(int index, string mainTableName, DBTableSettings table, DBColumnSettings foreignKey)
        {
            Index = index;
            MainTableName = mainTableName;
            Table = table;
            ForeignKey = foreignKey;
        }
    }
}
