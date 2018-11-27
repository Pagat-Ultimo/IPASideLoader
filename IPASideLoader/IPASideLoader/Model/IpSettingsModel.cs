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
    public class IpSettingsModel : IIpSettingsModel
    {
        public string LastIpAddress { get; set; } = "";

        public bool SaveSettings()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this);
                if (!Directory.Exists(FilePaths.IpSettingsDirectory))
                {
                    Directory.CreateDirectory(FilePaths.IpSettingsDirectory);
                }
                File.WriteAllText(FilePaths.IpSettingsPath, json);
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
                if (!File.Exists(FilePaths.IpSettingsPath))
                    return false;
                var json = File.ReadAllText(FilePaths.IpSettingsPath);
                var setting = JsonConvert.DeserializeObject<IpSettingsModel>(json);
                LastIpAddress = setting.LastIpAddress;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
