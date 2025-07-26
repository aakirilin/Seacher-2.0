using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Commons
{
    public class Settings
    {
        public List<DBSettings> DBSettings { get; set; }

        public DBSettings this[string dbName]
        {
            get => DBSettings.First(d => d.Name.Equals(dbName));
        }

        public Settings()
        {
            DBSettings = new List<DBSettings>();
        }
    }
}
