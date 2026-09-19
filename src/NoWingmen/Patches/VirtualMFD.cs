using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(VirtualMFD), "Start")]
internal static class VirtualMFD_Start
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(VirtualMFD __instance)
    {
        State.OnCreateActions.Push(state => state.AttachWingScreen(__instance));
    }
}