using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPASideLoader.Model;

namespace IPASideLoader.Constants
{
    public class FilePaths
    {
        public static string IPASettingsDirectory => @"Settings";
        public static string IPASettingsFileName => @"IPASettings.sl";
        public static string IPASettingsPath => Path.Combine(IPASettingsDirectory, IPASettingsFileName);

        public static string IpSettingsDirectory => @"Settings";
        public static string IpSettingsFileName => @"IpSettings.sl";
        public static string IpSettingsPath => Path.Combine(IpSettingsDirectory, IpSettingsFileName);

        public static string ApplicationSettingsDirectory => @"Settings";
        public static string ApplicationSettingsFileName => @"ApplicationSettings.sl";
        public static string ApplicationSettingsPath => Path.Combine(ApplicationSettingsDirectory, ApplicationSettingsFileName);

        public static string IPAPath => @"web/ipa.ipa";
        public static string SSLCert => @"openssl/certificates/local/side.pfx";
        public static string CaCert => @"openssl/certificates/ca/sideCA.cer";
        public static string CaCertWebPath => @"web/sideCA.cer";

        public static string CertDirectory => @"openssl/certificates";
        public static string CaDirectory => @"openssl/certificates/ca";
        public static string CertLocalDirectory => @"openssl/certificates/local";

    }
}
