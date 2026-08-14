using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(UnitMapIcon), "GetColor")]
internal static class UnitMapIcon_GetColor
{
    private const float SelectedBrighten = 0.35f;

    private static readonly AccessTools.FieldRef<MapIcon, bool> IsSelectedRef =
        AccessTools.FieldRefAccess<MapIcon, bool>("isSelected");

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(UnitMapIcon __instance, ref Color __result)
    {
        var unit = __instance?.unit;
        if (unit == null)
        {
            return;
        }

        if (!Plugin.StateManager.MarkColorResolver.TryResolve(unit, out var color))
        {
            return;
        }

        if (IsSelectedRef(__instance))
        {
            var configuredAlpha = color.a;
            color = Color.Lerp(color, Color.white, SelectedBrighten);
            color.a = configuredAlpha;
        }

        color.a *= __result.a;
        __result = color;
    }
}