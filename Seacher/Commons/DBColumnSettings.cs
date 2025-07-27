namespace Seacher.Commons
{
    public class DBColumnSettings
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public long Length { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedColumnName { get; set; }
        public bool ShowInData {get; set; }
        public bool ShowInCondition {get; set; }


        public DBColumnSettings() { }

        public DBColumnSettings(DBTableColumnData column)
        {
            Name = column.Column;
            DataType = column.DataType;
            Length = column.Length;
            ReferencedTableName = column.ReferencedTableName;
            ReferencedColumnName = column.ReferencedColumnName;
        }
    }
}
