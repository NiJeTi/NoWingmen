using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(HUDUnitMarker), "UpdateColor")]
internal static class HUDUnitMarker_UpdateColor
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(HUDUnitMarker __instance)
    {
        if (Plugin.MissionState == null)
        {
            return;
        }

        if (__instance.selected)
        {
            return;
        }

        if (!Plugin.MissionState.MarkColorResolver.TryResolve(__instance.unit, out var color))
        {
            return;
        }

        color.a *= __instance.image.color.a;
        __instance.image.color = color;
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(HUDUnitMarker), "SetFactionColor")]
internal static class HUDUnitMarker_SetFactionColor
{
    private static readonly AccessTools.FieldRef<HUDUnitMarker, Color> ColorRef =
        AccessTools.FieldRefAccess<HUDUnitMarker, Color>("color");

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(HUDUnitMarker __instance)
    {
        if (Plugin.MissionState == null)
        {
            return;
        }

        if (__instance.selected)
        {
            return;
        }

        if (!Plugin.MissionState.MarkColorResolver.TryResolve(__instance.unit, out var color))
        {
            return;
        }

        color.a *= ColorRef(__instance).a;
        ColorRef(__instance) = color;
    }
}