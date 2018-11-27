using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using IPASideLoader.Constants;
using IPASideLoader.Model;
using uhttpsharp;
using uhttpsharp.Listeners;
using uhttpsharp.RequestProviders;

namespace IPASideLoader.Services
{
    public class HttpService : IHttpService
    {
        private readonly IIpService _ipService;
        private HttpServer _httpServer;

        public HttpService(IIpService ipService)
        {
            _ipService = ipService;
        }

        public bool IsRunning => _httpServer != null;

        public void StartHttpServer()
        {
            if (_httpServer == null)
            {
                _httpServer = new HttpServer(new HttpRequestProvider());
                _httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Parse(_ipService.GetLocalIpAddress()), 80)));

                // Ssl Support :
                var serverCertificate = new X509Certificate2(FilePaths.SSLCert, "4IPA_only!",
                    X509KeyStorageFlags.MachineKeySet);
                _httpServer.Use(new ListenerSslDecorator(new TcpListenerAdapter(new TcpListener(IPAddress.Parse(_ipService.GetLocalIpAddress()), 9696)), serverCertificate));
                
                // Request handling : 
                _httpServer.Use((context, next) => {
                    Console.WriteLine("Got Request!");
                    return next();
                });
                _httpServer.Use(new CustomFileHandler());

                _httpServer.Start();
            }
        }

        public void StopHttpServer()
        {
            _httpServer?.Dispose();
            _httpServer = null;
        }
    }
}
