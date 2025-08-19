using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Commons
{
    public class QerryCreatorForeignKey
    {
        public DBTableSettings Table { get; set; }
        public DBColumnSettings ForeignKey { get; set; }

        public QerryCreatorForeignKey(DBTableSettings table, DBColumnSettings foreignKey)
        {
            Table = table;
            ForeignKey = foreignKey;
        }
    }
}
