using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using IPASideLoader.Constants;
using IPASideLoader.Services;
using Newtonsoft.Json;

namespace IPASideLoader.Model
{
    public class ApplicationSettingsModel : IApplicationSettingsModel
    {
        private readonly IEncryptionService _encryptionService;

        
        public string CAPassword { get; set; }
        public bool IsLoaded { get; set; }

        public ApplicationSettingsModel(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public bool SettingsExists => File.Exists(FilePaths.ApplicationSettingsPath);

        public bool SaveSettings(string password)
        {
            try
            {
                var json = JsonConvert.SerializeObject(this);
                json = _encryptionService.Encrypt(json, password);
                if (!Directory.Exists(FilePaths.ApplicationSettingsDirectory))
                {
                    Directory.CreateDirectory(FilePaths.ApplicationSettingsDirectory);
                }
                File.WriteAllText(FilePaths.ApplicationSettingsPath, json);
                IsLoaded = true;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public LoadSettingsResult LoadSettings(string password)
        {
            try
            {
                if (!File.Exists(FilePaths.ApplicationSettingsPath))
                    return LoadSettingsResult.FileNotExisting;
                var json = File.ReadAllText(FilePaths.ApplicationSettingsPath);
                json = _encryptionService.Decrypt(json, password);
                var setting = JsonConvert.DeserializeObject<ApplicationSettingsModel>(json);
                CAPassword = setting.CAPassword;
                IsLoaded = true;
                return LoadSettingsResult.Success;
            }
            catch (Exception e)
            {
                return LoadSettingsResult.WrongPassword;
            }
        }
    }
}
