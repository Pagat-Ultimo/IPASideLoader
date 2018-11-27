namespace IPASideLoader.Model
{
    public interface IIpSettingsModel
    {
        string LastIpAddress { get; set; }
        bool SaveSettings();
        bool LoadSettings();
    }
}