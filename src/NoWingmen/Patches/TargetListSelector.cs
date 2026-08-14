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
        if (__result)
        {
            return;
        }

        if (!Config.PreventSelectedTargetsLock.Value)
        {
            return;
        }

        if (!Plugin.StateManager.TargetClaimIndex.IsLockPrevented(u))
        {
            return;
        }

        __result = true;
    }
}