using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using IPASideLoader.Constants;
using IPASideLoader.Model;
using IPASideLoader.Pages;
using IPASideLoader.Services;
using IPASideLoader.ViewModels.Interfaces;
using Microsoft.Win32;

namespace IPASideLoader.ViewModels
{
    public class MainViewModel : ViewModelBase, IWindowEventAwareViewModel
    {
        #region Fields

        private readonly IApplicationSettingsModel _applicationSettingsModel;
        private readonly ICertService _certService;
        private readonly IHttpService _httpService;
        private readonly IIPASettingsModel _ipaModel;
        private readonly IIpService _ipService;
        private readonly IIpSettingsModel _ipSettings;
        private readonly IUserDataModel _userDataModel;

        private string _bundleIdentifier;

        private string _downloadUrl;

        private string _ipaPath;

        private ImageSource _qrCode;

        private int _serverStartProgress;

        private string _versionName;

        #endregion

        #region  Properties

        public string BundleIdentifier
        {
            get => _bundleIdentifier;
            set
            {
                if (_bundleIdentifier == value)
                    return;
                _bundleIdentifier = value;
                _ipaModel.BundleIdentifier = value;
                RaisePropertyChanged();
            }
        }

        public string VersionName
        {
            get => _versionName;
            set
            {
                if (_versionName == value)
                    return;
                _versionName = value;
                _ipaModel.VersionName = value;
                RaisePropertyChanged();
            }
        }

        public string IpaPath
        {
            get => _ipaPath;
            set
            {
                if (_ipaPath == value)
                    return;
                _ipaPath = value;
                _ipaModel.FilePath = value;
                RaisePropertyChanged();
            }
        }

        public string DownloadUrl
        {
            get => _downloadUrl;
            set
            {
                if (_downloadUrl == value)
                    return;
                _downloadUrl = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource QrCode
        {
            get => _qrCode;
            set
            {
                if (_qrCode == value)
                    return;
                _qrCode = value;
                RaisePropertyChanged();
            }
        }

        public int ServerStartProgress
        {
            get => _serverStartProgress;
            set
            {
                if (_serverStartProgress == value)
                    return;
                _serverStartProgress = value;
                RaisePropertyChanged();
            }
        }

        public ICommand SelectIpaCommand { get; set; }
        public ICommand FixCertCommand { get; set; }
        public ICommand CreateCaCertCommand { get; set; }
        public ICommand CreateDownloadCommand { get; set; }
        public ICommand CertificateDownloadCommand { get; set; }


        #endregion

        #region Constructor

        public MainViewModel(IIPASettingsModel ipaModel, IIpService ipService, IHttpService httpService,
            ICertService certService, IIpSettingsModel ipSettings, IApplicationSettingsModel applicationSettingsModel,
            IUserDataModel userDataModel)
        {
            _ipaModel = ipaModel;
            _ipSettings = ipSettings;
            _ipService = ipService;
            _httpService = httpService;
            _certService = certService;
            _applicationSettingsModel = applicationSettingsModel;
            _userDataModel = userDataModel;
            BundleIdentifier = _ipaModel.BundleIdentifier;
            VersionName = _ipaModel.VersionName;
            IpaPath = _ipaModel.FilePath;

            SelectIpaCommand = new RelayCommand(SelectIpaExecute);
            FixCertCommand = new RelayCommand(FixCertExecute);
            CreateCaCertCommand = new RelayCommand(CreateCaCertExecute);
            CreateDownloadCommand = new RelayCommand(() => CreateDownloadExecute("https://" + _ipService.GetLocalIpAddress() + ":9696/web/index.html"));
            CertificateDownloadCommand = new RelayCommand(() => CreateDownloadExecute("http://" + _ipService.GetLocalIpAddress() + "/web/sideCA.cer"));
        }

        #endregion

        #region IWindowEventAwareViewModel Members

        public void WindowClosed()
        {
            _httpService.StopHttpServer();
            _ipaModel.SaveSettings();
            _ipSettings.SaveSettings();
        }

        public void WindowShowed()
        {
            if (LoadApplicationSettings() == false)
                Application.Current.MainWindow?.Close();
            if (!File.Exists(FilePaths.CertDirectory))
                Directory.CreateDirectory(FilePaths.CertDirectory);
            if (!File.Exists(FilePaths.CaDirectory))
                Directory.CreateDirectory(FilePaths.CaDirectory);
            if (!File.Exists(FilePaths.CertLocalDirectory))
                Directory.CreateDirectory(FilePaths.CertLocalDirectory);
        }

        #endregion

        #region Public Methods

        public void SelectIpaExecute()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                IpaPath = openFileDialog.FileName;
                File.Copy(IpaPath, FilePaths.IPAPath, true);
            }
        }

        public void CreateDownloadExecute(string downloadUrl)
        {
            try
            {
                ServerStartProgress = 0;
                if (!_httpService.IsRunning)
                {
                    if (_applicationSettingsModel.IsLoaded == false)
                    {
                        MessageBox.Show("Error",
                            "Application Settings are not loaded. Please restart the application.");
                        Application.Current.MainWindow?.Close();
                    }

                    ServerStartProgress = 5;
                    if (!_certService.IsCaExisting)
                    {
                        var caQuestionResult = MessageBox.Show("Error",
                            "CA certificate missing? Would you like to create one now?", MessageBoxButton.YesNo);
                        if (caQuestionResult == MessageBoxResult.Yes)
                        {
                            CreateCaCertExecute();
                        }
                        else
                        {
                            return;
                        }
                    }

                    ServerStartProgress = 10;
                    if (_certService.NewCertificateNeeded())
                        _certService.CreateCertificates();
                    ServerStartProgress = 20;
                    if (!File.Exists(FilePaths.IPAPath))
                    {
                        MessageBox.Show("Error", "No .ipa selected. Please select the .ipa you want to install!");
                        return;
                    }

                    ServerStartProgress = 30;
                    var plist = File.ReadAllText("web/download2.plist");
                    plist = plist.Replace("{appidentifier}", BundleIdentifier);
                    plist = plist.Replace("{appversion}", VersionName);
                    plist = plist.Replace("{ipapath}", @"https://" + _ipService.GetLocalIpAddress() + ":9696/web/ipa.ipa");
                    File.WriteAllText(@"web/download2.plist", plist);
                    ServerStartProgress = 50;
                    var index = File.ReadAllText("web/index.html");
                    index = index.Replace("{ip}", _ipService.GetLocalIpAddress());
                    File.WriteAllText(@"web/index.html", index);
                    ServerStartProgress = 70;
                    _httpService.StartHttpServer();
                }

                var encoder = new QrEncoder(ErrorCorrectionLevel.M);
                QrCode qrCode;
                encoder.TryEncode(downloadUrl, out qrCode);
                var wRenderer = new WriteableBitmapRenderer(new FixedModuleSize(3, QuietZoneModules.Two), Colors.Black,
                    Colors.White);
                var size = 100;
                var wBitmap = new WriteableBitmap(size, size, size - 1, size - 1, PixelFormats.Gray8, null);
                wRenderer.Draw(wBitmap, qrCode.Matrix);

                QrCode = wBitmap;
                DownloadUrl = downloadUrl;
             
                ServerStartProgress = 100;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"ERROR " + ex.Message);
            }
        }

        public void FixCertExecute()
        {
            _certService.CreateCertificates();
        }

        public void CreateCaCertExecute()
        {
            var dialog = new InputDialog();
            if (dialog.ShowDialog() == true)
            {
                if (_certService.CreateCaCert(dialog.ResponseText))
                {
                    _applicationSettingsModel.CAPassword = dialog.ResponseText;
                    _applicationSettingsModel.SaveSettings(_userDataModel.Password);
                }
                else
                {
                    MessageBox.Show("Error creating CA Certificate");
                }
            }

            //_certService.CreateCertificates();
        }


        #endregion

        #region Private Methods

        private bool LoadApplicationSettings()
        {
            var dialog = new InputDialog();
            if (_applicationSettingsModel.SettingsExists == false)
                dialog.DialogText = "Please set a password to secure your settings";
            else
                dialog.DialogText = "Please input your password to load your secure settings";

            if (dialog.ShowDialog() == true)
            {
                var result = _applicationSettingsModel.LoadSettings(dialog.ResponseText);
                switch (result)
                {
                    case LoadSettingsResult.Success:
                        _userDataModel.Password = dialog.ResponseText;
                        return true;
                    case LoadSettingsResult.Failed:
                        return false;
                    case LoadSettingsResult.FileNotExisting:
                        _applicationSettingsModel.SaveSettings(dialog.ResponseText);
                        break;
                    case LoadSettingsResult.WrongPassword:
                        var resetPasswordResult = MessageBox.Show("Error",
                            "The given password was wrong would you like to reset your password (YES) your settings and certificates will be lost or try again (NO)",
                            MessageBoxButton.YesNo);
                        if (resetPasswordResult == MessageBoxResult.Yes)
                            _applicationSettingsModel.SaveSettings(dialog.ResponseText);
                        else
                        {
                            return LoadApplicationSettings();
                        }

                        break;
                    default:
                        return false;
                }

                if (_applicationSettingsModel.LoadSettings(dialog.ResponseText) != LoadSettingsResult.Success)
                    return false;

                _userDataModel.Password = dialog.ResponseText;
                return true;
            }

            return false;
        }

        #endregion
    }
}