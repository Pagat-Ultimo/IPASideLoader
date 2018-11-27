namespace IPASideLoader.Services
{
    public interface ICertService
    {
        bool IsCaExisting { get; }
        bool CreateCertificates();
        bool CreateCaCert(string password);
        bool NewCertificateNeeded();
    }
}
