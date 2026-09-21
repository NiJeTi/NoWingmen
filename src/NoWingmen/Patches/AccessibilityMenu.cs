using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using NoWingmen.Theming;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(AccessibilityMenu), "Awake")]
internal static class AccessibilityMenu_Awake
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(AccessibilityMenu __instance)
    {
        PaletteMenu.Build(__instance);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(AccessibilityMenu), "LoadColorPickers")]
internal static class AccessibilityMenu_LoadColorPickers
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(AccessibilityMenu __instance)
    {
        PaletteMenu.Refresh(__instance);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(AccessibilityMenu), "OnThemeGroupSave")]
internal static class AccessibilityMenu_OnThemeGroupSave
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Prefix()
    {
        PaletteMenu.Commit();
    }
}