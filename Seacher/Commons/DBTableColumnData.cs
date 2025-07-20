using Tmds.DBus.Protocol;

namespace Seacher.Commons
{
    public struct DBTableColumnData
    {
        public string Table;
        public string Column;
        public string DataType;
        public int Length;
        public string ReferencedTableName;
        public string ReferencedColumnName;
    }
}
