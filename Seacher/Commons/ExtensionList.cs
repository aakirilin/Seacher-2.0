using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Commons
{
    public static class ExtensionList
    {
        public static int IndexOfElement<T>(this List<T> collection, Func<T, bool> func)
        {
            var element = collection.First(func);
            return collection.IndexOf(element);
        }
    }
}
