using BepInEx.Configuration;
using NoWingmen.Theming;
using NuclearOption.UIStyleSystem;

namespace NoWingmen;

internal sealed class Settings
{
    private const string SectionGeneral = "General";

    public ConfigEntry<bool> ShowWing { get; }
    public ConfigEntry<bool> ShowWingTargetSelection { get; }
    public ConfigEntry<bool> ShowTeammates { get; }
    public ConfigEntry<bool> ShowTeammatesTargetSelection { get; }

    public PaletteStore Palette { get; }

    public event EventHandler VisualsChanged
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

    public event Action ThemeChanged
    {
        add => ThemeManager.ThemeGroupChanged += value;
        remove => ThemeManager.ThemeGroupChanged -= value;
    }

    private Settings(
        ConfigEntry<bool> showWing,
        ConfigEntry<bool> showWingTargetSelection,
        ConfigEntry<bool> showTeammates,
        ConfigEntry<bool> showTeammatesTargetSelection,
        PaletteStore palette
    )
    {
        ShowWing = showWing;
        ShowWingTargetSelection = showWingTargetSelection;
        ShowTeammates = showTeammates;
        ShowTeammatesTargetSelection = showTeammatesTargetSelection;
        Palette = palette;
    }

    public static Settings Init(ConfigFile config)
    {
        var showWing = config.Bind(
            SectionGeneral, "ShowWing", true,
            "Mark members of your wing."
        );
        var showWingTargetSelection = config.Bind(
            SectionGeneral, "ShowWingTargetSelection", true,
            "Mark every target each wing member have selected."
        );
        var showTeammates = config.Bind(
            SectionGeneral, "ShowTeammates", true,
            "Mark player aircraft in the same faction."
        );
        var showTeammatesTargetSelection = config.Bind(
            SectionGeneral, "ShowTeammatesTargetSelection", true,
            "Mark every target each teammate have selected."
        );

        var paletteStore = new PaletteStore();

        return new Settings(
            showWing,
            showWingTargetSelection,
            showTeammates,
            showTeammatesTargetSelection,
            paletteStore
        );
    }
}