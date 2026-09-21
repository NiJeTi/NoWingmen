using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using NuclearOption.UIStyleSystem;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(ThemeManager), nameof(ThemeManager.SaveActiveThemeGroup))]
internal static class ThemeManager_SaveActiveThemeGroup
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix()
    {
        Plugin.Settings.Palette.Save(ThemeManager.Active);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(ThemeManager), nameof(ThemeManager.CopyActiveThemeGroupWithNewId))]
internal static class ThemeManager_CopyActiveThemeGroupWithNewId
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Prefix(out ThemeGroup? __state)
    {
        __state = ThemeManager.Active;
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(string __result, ThemeGroup? __state)
    {
        if (__state == null)
        {
            return;
        }

        Plugin.Settings.Palette.Copy(__state, __result);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(ThemeManager), nameof(ThemeManager.DeleteActiveThemeGroup))]
internal static class ThemeManager_DeleteActiveThemeGroup
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Prefix(out string? __state)
    {
        __state = ThemeManager.Active?.Id;
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(string? __state)
    {
        if (string.IsNullOrEmpty(__state))
        {
            return;
        }

        Plugin.Settings.Palette.Drop(__state);
    }
}