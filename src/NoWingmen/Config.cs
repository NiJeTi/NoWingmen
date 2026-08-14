using BepInEx.Configuration;
using NoWingmen.Marks;
using UnityEngine;

namespace NoWingmen;

internal static class Config
{
    private const string GeneralSection = "General";
    private const string VisualsSection = "Visuals";
    private const string KeybindsSection = "Keybinds";

    public static ConfigEntry<bool> ShowWing { get; private set; }
    public static ConfigEntry<bool> ShowWingTargetSelection { get; private set; }
    public static ConfigEntry<bool> ShowFriends { get; private set; }
    public static ConfigEntry<bool> ShowTeammatesTargetSelection { get; private set; }
    public static ConfigEntry<bool> PreventSelectedTargetsLock { get; private set; }

    public static ConfigEntry<Color> WingColor { get; private set; }
    public static ConfigEntry<Color> FriendsColor { get; private set; }
    public static ConfigEntry<Color> SelectedTargetsColor { get; private set; }

    public static ConfigEntry<KeyboardShortcut> ToggleWingShortcut { get; private set; }
    public static ConfigEntry<KeyboardShortcut> ToggleLockPreventionShortcut { get; private set; }

    public static void Bind(ConfigFile config, Action onVisualsChanged)
    {
        ShowWing = config.Bind(
            GeneralSection, "ShowWing", true,
            "Mark members of your wing."
        );
        ShowWingTargetSelection = config.Bind(
            GeneralSection, "ShowWingTargetSelection", true,
            "Mark every target each wing member have selected."
        );
        ShowFriends = config.Bind(
            GeneralSection, "ShowFriends", true,
            "Mark your friends in the same faction."
        );
        ShowTeammatesTargetSelection = config.Bind(
            GeneralSection, "ShowTeammatesTargetSelection", false,
            "Mark every target each teammate have selected."
        );
        PreventSelectedTargetsLock = config.Bind(
            GeneralSection, "PreventSelectedTargetsLock", true,
            "Prevent locking enemy units your wing members have already selected."
        );

        WingColor = config.Bind(
            VisualsSection, "WingColor", new Color( 1f, 0.75f, 0f),
            "Color of members of your wing."
        );
        FriendsColor = config.Bind(
            VisualsSection, "FriendsColor", new Color(0.03f, 0.85f, 0.66f),
            "Color of your friends in the same faction."
        );
        SelectedTargetsColor = config.Bind(
            VisualsSection, "SelectedTargetsColor", new Color(0.8f, 0.3f, 1f),
            "Color of target enemy units."
        );

        ToggleWingShortcut = config.Bind(
            KeybindsSection, "ToggleWingShortcut", new KeyboardShortcut(KeyCode.P),
            "Add/remove the last selected unit to/from your wing."
        );
        ToggleLockPreventionShortcut = config.Bind(
            KeybindsSection, "ToggleLockPreventionShortcut", new KeyboardShortcut(KeyCode.O),
            "Toggle already targeted enemy units lock prevention shortcut."
        );

        ShowWing.SettingChanged += (_, _) => onVisualsChanged();
        ShowWingTargetSelection.SettingChanged += (_, _) => onVisualsChanged();
        ShowFriends.SettingChanged += (_, _) => onVisualsChanged();
        ShowTeammatesTargetSelection.SettingChanged += (_, _) => onVisualsChanged();

        WingColor.SettingChanged += (_, _) => onVisualsChanged();
        FriendsColor.SettingChanged += (_, _) => onVisualsChanged();
        SelectedTargetsColor.SettingChanged += (_, _) => onVisualsChanged();
    }

    public static bool IsCategoryVisible(MarkCategory markCategory)
    {
        return markCategory switch
        {
            MarkCategory.Wing => ShowWing.Value,
            MarkCategory.Friend => ShowFriends.Value,
            MarkCategory.ClaimedTarget => ShowWingTargetSelection.Value || ShowTeammatesTargetSelection.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(markCategory), markCategory, "Invalid category."),
        };
    }

    public static Color GetCategoryColor(MarkCategory markCategory)
    {
        return markCategory switch
        {
            MarkCategory.Wing => WingColor.Value,
            MarkCategory.Friend => FriendsColor.Value,
            MarkCategory.ClaimedTarget => SelectedTargetsColor.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(markCategory), markCategory, "Invalid category.")
        };
    }
}