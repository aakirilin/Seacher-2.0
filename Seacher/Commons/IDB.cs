using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;

namespace Seacher.Commons
{
    public interface IDB : IDisposable
    {
        void Open();
        void Close();

        IEnumerable<DBTableSettings> GetTables();
        IEnumerable<object> SelectQerry(Type type, string qerry);
    }
}