using Google.Protobuf.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Seacher.Commons
{
    public class Settings
    {
        private static string fileName = "settings.xml";
        public List<DBSettings> DBSettings { get; set; }

        public DBSettings this[string dbName]
        {
            get => DBSettings.First(d => d.Name.Equals(dbName));
        }

        public Settings()
        {
            DBSettings = new List<DBSettings>();
        }

        public void Save()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, this);
            }
        }

        public static Settings Load()
        {
            if (!File.Exists(fileName))
            {
                new Settings().Save();
            }
            Settings settings = null;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                settings = (Settings)xmlSerializer.Deserialize(fs);
            }

            return settings;
        }
    }
}
