using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChessDotNET.Settings
{
    internal struct AppSettingsStruct
    {
        internal Dictionary<string, string> EmailServer;
    }
    internal class Settings
    {
        public Settings(string _folderSettings)
        {
            folderSettings = _folderSettings;
        }

        private readonly string folderSettings;
    }
}
