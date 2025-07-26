using System;
using System.Collections.Generic;

namespace Seacher.Commons
{
    public interface IDB : IDisposable
    {
        void Open();
        void Close();

        IEnumerable<DBTableSettings> GetTables();
        IEnumerable<string[]> SelectQerry(string qerry);
    }
}
