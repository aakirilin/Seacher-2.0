using System.Reflection.Metadata.Ecma335;

namespace Seacher.Commons
{
    public class QerryCreatorTableColumn
    {
        public QerryCreatorTableColumn(
            int tableIndex,
            int columnIndex,
            string tableName,
            DBColumnSettings column,
            DBColumnSettings foreignKey)
        {
            TableIndex = tableIndex;
            ColumnIndex = columnIndex;
            TableName = tableName;
            Column = column;
            ForeignKey = foreignKey;
        }
        public int TableIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string TableName { get; set; }
        public DBColumnSettings Column { get; set; }
        public DBColumnSettings ForeignKey { get; set; } 
    }
}
