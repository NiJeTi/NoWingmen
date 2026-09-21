using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using NoWingmen.Marks;
using TMPro;
using UnityEngine;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(LeaderboardPlayerEntry), "UpdateScore")]
internal static class LeaderboardPlayerEntry_UpdateScore
{
    private static readonly AccessTools.FieldRef<LeaderboardPlayerEntry, TextMeshProUGUI> NameTextRef =
        AccessTools.FieldRefAccess<LeaderboardPlayerEntry, TextMeshProUGUI>("textName");

    private static readonly AccessTools.FieldRef<LeaderboardPlayerEntry, Color> DefaultNameColorRef =
        AccessTools.FieldRefAccess<LeaderboardPlayerEntry, Color>("defaultNameColor");

    private static readonly AccessTools.FieldRef<LeaderboardPlayerEntry, bool> IsVoteKickedRef =
        AccessTools.FieldRefAccess<LeaderboardPlayerEntry, bool>("previousIsVoteKicked");

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(LeaderboardPlayerEntry __instance)
    {
        if (Plugin.State == null)
        {
            return;
        }

        var player = __instance.Player;

        if (IsVoteKickedRef(__instance))
        {
            return;
        }

        NameTextRef(__instance).color = Plugin.State.Wing.Check(player)
            ? MarkColorResolver.GetColor(MarkCategory.Wing)
            : DefaultNameColorRef(__instance);
    }
}