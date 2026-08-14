using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(ChatBox), "OnEnable")]
internal static class ChatBox_OnEnable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(ChatBox __instance)
    {
        Plugin.StateManager.IsChatOpen = __instance.gameObject.activeSelf;
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(ChatBox), "OnDisable")]
internal static class ChatBox_OnDisable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix()
    {
        Plugin.StateManager.IsChatOpen = false;
    }
}