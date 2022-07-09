using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChessDotNET.Settings
{
    internal struct AppSettingsStruct
    {
        internal Dictionary<string, string> EmailServer;
    }
    internal class AppSettings
    {
        public AppSettings(string _appSettingsFolder)
        {
            appSettingsFolder = _appSettingsFolder;
        }
        internal AppSettingsStruct LoadSettings()
        {
            AppSettingsStruct appSettingsStruct = new AppSettingsStruct();

            string settingsFilename = Path.Combine(appSettingsFolder, "settings.json");
            if (File.Exists(settingsFilename))
            {
                using (var settingsFile = File.OpenText(settingsFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Dictionary<string, object> settingsDictObject = (Dictionary<string, object>)serializer.Deserialize(settingsFile, typeof(Dictionary<string, object>));
                    Dictionary<string, Dictionary<string, string>> settingsDict = JObject.FromObject(settingsDictObject).ToObject<Dictionary<string, Dictionary<string, string>>>();
                    if (settingsDict.ContainsKey("EmailServer"))
                    {
                        
                        Dictionary<string, string> EmailServer = settingsDict["EmailServer"];
                        appSettingsStruct.EmailServer = EmailServer;
                    }
                }
            }
            else
            {
                var emailServer = new Dictionary<string, string>()
                {
                    ["email_address"] = "user@server.com",
                    ["pop3_server"] = "pop3.server.com",
                    ["smtp_server"] = "smtp.server.com",
                    ["pop3_port"] = "995",
                    ["smtp_port"] = "587",
                    ["password"] = "password",
                };

                Dictionary<string, object> settingsDict = new Dictionary<string, object>
                {
                    ["EmailServer"] = emailServer
                };

                using (var file = File.CreateText(settingsFilename))
                {
                    var settingsDictJson = JsonConvert.SerializeObject(settingsDict, Formatting.Indented);
                    file.WriteLine(settingsDictJson);
                }
                appSettingsStruct.EmailServer = emailServer;
            }

            return appSettingsStruct;
        }

        public void ChangeEmailServer(Dictionary<string, string> EmailServerDict)
        {
            string filename = Path.Combine(appSettingsFolder, "settings.json");

            AppSettingsStruct appSettingsStruct = new AppSettingsStruct();

            Dictionary<string, object> oldDict;
            using (var fileLoad = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                oldDict = (Dictionary<string, object>)serializer.Deserialize(fileLoad, typeof(Dictionary<string, object>));

                if (oldDict.ContainsKey("EmailServer"))
                {
                    Dictionary<string, Dictionary<string, string>> tempDict = JObject.FromObject(oldDict).ToObject<Dictionary<string, Dictionary<string, string>>>();

                    appSettingsStruct.EmailServer = tempDict["EmailServer"];
                }
            }

            Dictionary<string, object> dict = new Dictionary<string, object>(oldDict)
            {
                ["EmailServer"] = EmailServerDict
            };

            using (var fileSave = File.CreateText(@filename))
            {
                var dictJson = JsonConvert.SerializeObject(dict, Formatting.Indented);
                fileSave.WriteLine(dictJson);
            }
        }

        private readonly string appSettingsFolder;
    }
}
