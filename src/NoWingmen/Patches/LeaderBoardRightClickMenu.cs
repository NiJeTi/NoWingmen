using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using NuclearOption.Networking;
using NuclearOption.UI;
using TMPro;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NoWingmen.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(LeaderBoardRightClickMenu), "OnShowPanel")]
internal static class LeaderBoardRightClickMenu_OnShowPanel
{
    private const string ButtonName = "NoWingmen.WingButton";

    private const string ToggleLabel = "Toggle in wing";

    private static readonly AccessTools.FieldRef<LeaderBoardRightClickMenu, Button> MuteButtonRef =
        AccessTools.FieldRefAccess<LeaderBoardRightClickMenu, Button>("muteButton");

    private static readonly AccessTools.FieldRef<LeaderBoardRightClickMenu, Button> BlockButtonRef =
        AccessTools.FieldRefAccess<LeaderBoardRightClickMenu, Button>("blockButton");

    private delegate bool TryGetPlayer(LeaderBoardRightClickMenu menu, out Player player);

    private static readonly TryGetPlayer TryGetPlayerRef = AccessTools.MethodDelegate<TryGetPlayer>(
        AccessTools.Method(typeof(LeaderBoardRightClickMenu), "TryGetPlayer")
    );

    private delegate UniTaskVoid HideMenuAsync(LeaderBoardRightClickMenu menu);

    private static readonly HideMenuAsync HideMenuAsyncRef = AccessTools.MethodDelegate<HideMenuAsync>(
        AccessTools.Method(typeof(LeaderBoardRightClickMenu), "HideMenuAsync")
    );

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(LeaderBoardRightClickMenu __instance)
    {
        if (Plugin.MissionState == null)
        {
            return;
        }

        var template = MuteButtonRef(__instance);

        var existing = template.transform.parent.Find(ButtonName);

        if (!TryGetTarget(__instance, out var player))
        {
            if (existing != null)
            {
                existing.gameObject.SetActive(false);
            }

            return;
        }

        var button = existing != null ? existing.GetComponent<Button>() : CreateButton(__instance, template);

        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick(__instance, player));

        button.GetComponentInChildren<TextMeshProUGUI>(true).text = ToggleLabel;

        button.gameObject.SetActive(true);
    }

    private static bool TryGetTarget(LeaderBoardRightClickMenu menu, [NotNullWhen(true)] out Player? player)
    {
        player = null;

        return TryGetPlayerRef(menu, out player) && Wing.IsEligible(player);
    }

    private static Button CreateButton(LeaderBoardRightClickMenu menu, Button template)
    {
        var button = Object.Instantiate(template, template.transform.parent);
        button.name = ButtonName;

        button.transform.SetSiblingIndex(BlockButtonRef(menu).transform.GetSiblingIndex() + 1);

        return button;
    }

    private static void OnClick(LeaderBoardRightClickMenu menu, Player player)
    {
        Plugin.MissionState?.Wing.Toggle(player);
        HideMenuAsyncRef(menu);
    }
}