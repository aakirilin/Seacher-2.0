namespace Seacher.Commons
{
    public class QerryCreatorTableColumn
    {
        public QerryCreatorTableColumn(string tableName, DBColumnSettings column, DBColumnSettings foreignKey)
        {
            TableName = tableName;
            Column = column;
            ForeignKey = foreignKey;
        }

        public string TableName { get; set; }
        public DBColumnSettings Column { get; set; }
        public DBColumnSettings ForeignKey { get; set; } 
    }
}
