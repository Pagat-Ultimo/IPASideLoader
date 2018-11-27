using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using IPASideLoader.Model;
using IPASideLoader.Services;

namespace IPASideLoader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ViewModelLocator _locator;

        public static ViewModelLocator Locator => _locator ?? (_locator = new ViewModelLocator());

        public App()
        {
            RegisterServices();
            InitInstances();
        }

        private void RegisterServices()
        {
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();

            SimpleIoc.Default.Register<IIPASettingsModel, IPASettingsModel>();
            SimpleIoc.Default.Register<IUserDataModel, UserDataModel>();
            SimpleIoc.Default.Register<IIpSettingsModel, IpSettingsModel>();
            SimpleIoc.Default.Register<IApplicationSettingsModel, ApplicationSettingsModel>();

            SimpleIoc.Default.Register<IIpService, IpService>();
            SimpleIoc.Default.Register<IHttpService, HttpService>();
            SimpleIoc.Default.Register<ICertService, CertService>();

        }

        private void InitInstances()
        {
            var ipaModel = SimpleIoc.Default.GetInstance<IIPASettingsModel>();
            ipaModel.LoadSettings();
            var ipModel = SimpleIoc.Default.GetInstance<IIpSettingsModel>();
            ipModel.LoadSettings();

        }

    }
}
