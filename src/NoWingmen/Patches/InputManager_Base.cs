using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Rewired;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(InputManager_Base), "Awake")]
internal static class InputManager_Base_Awake
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Prefix(InputManager_Base __instance)
    {
        Controls.Inject(__instance.userData);
    }
}
