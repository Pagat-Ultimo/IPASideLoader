using IPASideLoader.Constants;

namespace IPASideLoader.Model
{
    public interface IApplicationSettingsModel
    {
        string CAPassword { get; set; }
        bool IsLoaded { get; set; }

        bool SettingsExists { get; }

        bool SaveSettings(string password);
        LoadSettingsResult LoadSettings(string password);
    }
}
