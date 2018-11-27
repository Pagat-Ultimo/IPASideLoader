using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPASideLoader.Constants;
using Newtonsoft.Json;

namespace IPASideLoader.Model
{
    public class IPASettingsModel : IIPASettingsModel
    {
        public string BundleIdentifier { get; set; } = "com.bundle.identifier";
        public string VersionName { get; set; } = "1.0.0";
        public string FilePath { get; set; } = "";

        public bool SaveSettings()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this);
                if (!Directory.Exists(FilePaths.IPASettingsDirectory))
                {
                    Directory.CreateDirectory(FilePaths.IPASettingsDirectory);
                }
                File.WriteAllText(FilePaths.IPASettingsPath, json);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }

        public bool LoadSettings()
        {
            try
            {
                if (!File.Exists(FilePaths.IPASettingsPath))
                    return false;
                var json = File.ReadAllText(FilePaths.IPASettingsPath);
                var setting = JsonConvert.DeserializeObject<IPASettingsModel>(json);
                BundleIdentifier = setting.BundleIdentifier;
                VersionName = setting.VersionName;
                FilePath = setting.FilePath;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
