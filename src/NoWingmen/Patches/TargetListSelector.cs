using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(TargetListSelector), nameof(TargetListSelector.CheckExclusions))]
internal static class TargetListSelector_CheckExclusions
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(Unit u, ref bool __result)
    {
        if (Plugin.MissionState == null)
        {
            return;
        }

        if (__result)
        {
            return;
        }

        if (!Plugin.Settings.LockPreventionEnabled.Value)
        {
            return;
        }

        if (!Plugin.MissionState.TargetClaimIndex.IsClaimed(u))
        {
            return;
        }

        __result = true;
    }
}