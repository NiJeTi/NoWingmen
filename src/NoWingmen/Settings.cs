using BepInEx.Configuration;

namespace NoWingmen;

internal sealed class Settings
{
    private const string SectionGeneral = "General";

    public ConfigEntry<bool> ShowWing { get; private set; } = null!;
    public ConfigEntry<bool> ShowWingTargetSelection { get; private set; } = null!;
    public ConfigEntry<bool> ShowTeammates { get; private set; } = null!;
    public ConfigEntry<bool> ShowTeammatesTargetSelection { get; private set; } = null!;

    public event EventHandler SettingsChanged
    {
        add
        {
            ShowWing.SettingChanged += value;
            ShowWingTargetSelection.SettingChanged += value;
            ShowTeammates.SettingChanged += value;
            ShowTeammatesTargetSelection.SettingChanged += value;
        }
        remove
        {
            ShowWing.SettingChanged -= value;
            ShowWingTargetSelection.SettingChanged -= value;
            ShowTeammates.SettingChanged -= value;
            ShowTeammatesTargetSelection.SettingChanged -= value;
        }
    }

    private Settings()
    {
    }

    public static Settings Bind(ConfigFile config)
    {
        return new Settings
        {
            ShowWing = config.Bind(
                SectionGeneral, "ShowWing", true,
                "Mark members of your wing."
            ),
            ShowWingTargetSelection = config.Bind(
                SectionGeneral, "ShowWingTargetSelection", true,
                "Mark every target each wing member have selected."
            ),
            ShowTeammates = config.Bind(
                SectionGeneral, "ShowTeammates", true,
                "Mark player aircraft in the same faction."
            ),
            ShowTeammatesTargetSelection = config.Bind(
                SectionGeneral, "ShowTeammatesTargetSelection", true,
                "Mark every target each teammate have selected."
            )
        };
    }
}