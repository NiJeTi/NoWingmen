using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(MapIcon), nameof(MapIcon.SelectIcon))]
internal static class MapIcon_SelectIcon
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(MapIcon __instance)
    {
        Plugin.StateManager.Select((__instance as UnitMapIcon)?.unit);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(MapIcon), nameof(MapIcon.DeselectIcon))]
internal static class MapIcon_DeselectIcon
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(MapIcon __instance)
    {
        Plugin.StateManager.Deselect((__instance as UnitMapIcon)?.unit);
    }
}
