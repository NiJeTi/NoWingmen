using BepInEx.Configuration;
using NoWingmen.Marks;
using UnityEngine;

namespace NoWingmen;

internal static class Config
{
    private const string GeneralSection = "General";
    private const string VisualsSection = "Visuals";

    public static ConfigEntry<bool> ShowWing { get; private set; }
    public static ConfigEntry<bool> ShowWingTargetSelection { get; private set; }
    public static ConfigEntry<bool> ShowFriends { get; private set; }
    public static ConfigEntry<bool> ShowTeammates { get; private set; }
    public static ConfigEntry<bool> ShowTeammatesTargetSelection { get; private set; }

    public static ConfigEntry<Color> WingColor { get; private set; }
    public static ConfigEntry<Color> FriendsColor { get; private set; }
    public static ConfigEntry<Color> TeammatesColor { get; private set; }
    public static ConfigEntry<Color> SelectedTargetsColor { get; private set; }

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
        ShowTeammates = config.Bind(
            GeneralSection, "ShowTeammates", true,
            "Mark player aircraft in the same faction."
        );
        ShowTeammatesTargetSelection = config.Bind(
            GeneralSection, "ShowTeammatesTargetSelection", true,
            "Mark every target each teammate have selected."
        );

        WingColor = config.Bind(
            VisualsSection, "WingColor", new Color(1f, 0.75f, 0f),
            "Color of members of your wing."
        );
        FriendsColor = config.Bind(
            VisualsSection, "FriendsColor", new Color(0.03f, 0.85f, 0.66f),
            "Color of your friends in the same faction."
        );
        TeammatesColor = config.Bind(
            VisualsSection, "TeammatesColor", new Color(0.45f, 0.7f, 1f),
            "Color of player aircraft in the same faction."
        );
        SelectedTargetsColor = config.Bind(
            VisualsSection, "SelectedTargetsColor", new Color(0.8f, 0.3f, 1f),
            "Color of target enemy units."
        );

        ShowWing.SettingChanged += (_, _) => onVisualsChanged();
        ShowWingTargetSelection.SettingChanged += (_, _) => onVisualsChanged();
        ShowFriends.SettingChanged += (_, _) => onVisualsChanged();
        ShowTeammates.SettingChanged += (_, _) => onVisualsChanged();
        ShowTeammatesTargetSelection.SettingChanged += (_, _) => onVisualsChanged();

        WingColor.SettingChanged += (_, _) => onVisualsChanged();
        FriendsColor.SettingChanged += (_, _) => onVisualsChanged();
        TeammatesColor.SettingChanged += (_, _) => onVisualsChanged();
        SelectedTargetsColor.SettingChanged += (_, _) => onVisualsChanged();
    }

    public static bool IsCategoryVisible(MarkCategory markCategory)
    {
        return markCategory switch
        {
            MarkCategory.Wing => ShowWing.Value,
            MarkCategory.Friend => ShowFriends.Value,
            MarkCategory.Teammate => ShowTeammates.Value,
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
            MarkCategory.Teammate => TeammatesColor.Value,
            MarkCategory.ClaimedTarget => SelectedTargetsColor.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(markCategory), markCategory, "Invalid category.")
        };
    }
}