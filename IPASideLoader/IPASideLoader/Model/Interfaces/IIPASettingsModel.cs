namespace IPASideLoader.Model
{
    public interface IIPASettingsModel
    {
        string BundleIdentifier { get; set; }
        string VersionName { get; set; }
        string FilePath { get; set; }

        bool SaveSettings();
        bool LoadSettings();
    }
}