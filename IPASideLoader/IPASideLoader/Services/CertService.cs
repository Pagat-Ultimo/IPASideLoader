using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPASideLoader.Constants;
using IPASideLoader.Model;

namespace IPASideLoader.Services
{
    public class CertService : ICertService
    {
        private readonly IIpService _ipService;
        private readonly IIpSettingsModel _ipSettings;
        private readonly IApplicationSettingsModel _applicationSettingsModel;

        public CertService(IIpService ipService, IIpSettingsModel ipSettings, IApplicationSettingsModel applicationSettingsModel)
        {
            _ipService = ipService;
            _ipSettings = ipSettings;
            _applicationSettingsModel = applicationSettingsModel;
        }


        public bool IsCaExisting => File.Exists(FilePaths.CaCert);

        public bool CreateCertificates()
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = _ipService.GetLocalIpAddress() + " " + _applicationSettingsModel.CAPassword;
                startInfo.FileName = "createcert.bat";
                startInfo.WorkingDirectory = @"openssl\scripts";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                _ipSettings.LastIpAddress = _ipService.GetLocalIpAddress();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public bool CreateCaCert(string password)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = password;
                startInfo.FileName = @"createca.bat";
                startInfo.WorkingDirectory = @"openssl\scripts";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                File.Copy(FilePaths.CaCert, FilePaths.CaCertWebPath, true);
                return true;

            }
            catch (Exception e)
            {
                return false;
            }

        }

        public bool NewCertificateNeeded()
        {
            if (!File.Exists(FilePaths.SSLCert))
                return true;

            if (_ipService.GetLocalIpAddress() != _ipSettings.LastIpAddress)
                return true;

            return false;
        }
    }
}