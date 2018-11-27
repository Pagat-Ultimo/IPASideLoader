namespace IPASideLoader.Services
{
    public interface IHttpService
    {
        bool IsRunning { get; }
        void StartHttpServer();
        void StopHttpServer();
    }
}