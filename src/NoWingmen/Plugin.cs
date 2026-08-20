using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NoWingmen.Marks;
using NuclearOption.MissionEditorScripts;
using UnityEngine;

namespace NoWingmen;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal sealed class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }
    public static StateManager StateManager { get; private set; }

    private Harmony _harmony;

    private void Awake()
    {
        Logger = base.Logger;

        NoWingmen.Config.Bind(
            Config, () =>
            {
                MarkRenderer.Update();
                StateManager?.TargetClaimIndex.Clear();
                StateManager?.LineRenderer.Clear();
            }
        );

        StateManager = new StateManager();

        try
        {
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
        }
        catch (Exception e)
        {
            enabled = false;
            Logger.LogError($"Failed to patch: {e}");
            return;
        }

        Logger.LogInfo("Patch successful");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();

        StateManager.TargetClaimIndex.Clear();
        StateManager.LineRenderer.Clear();
        StateManager = null;
    }

    private void Update()
    {
        if (StateManager.IsChatOpen)
        {
            return;
        }

        if (InputFieldChecker.InsideInputField)
        {
            return;
        }

        if (IsPressed(NoWingmen.Config.ToggleWingShortcut.Value))
        {
            ToggleWing();
        }
        else if (IsPressed(NoWingmen.Config.ToggleLockPreventionShortcut.Value))
        {
            ToggleLockPrevention();
        }
    }

    private void LateUpdate()
    {
        if (StateManager.TargetClaimIndex.Refresh())
        {
            MarkRenderer.Update();
        }

        StateManager.LineRenderer.Update();
    }

    private static void ToggleWing()
    {
        var unit = StateManager.CurrentSelection;

        if (unit == null)
        {
            Logger.LogDebug("Shortcut is pressed on an empty selection");
            Feedback.OnWingReject();
            return;
        }

        if (!Identity.TryGetId(unit, out var id))
        {
            Logger.LogDebug($"'{unit.unitName}' is not a player");
            Feedback.OnWingReject();
            return;
        }

        if (!Identity.IsSameFaction(unit) && !StateManager.Wing.Check(id))
        {
            Logger.LogDebug($"'{unit.unitName}' is in another faction");
            Feedback.OnWingReject();
            return;
        }

        var status = StateManager.Wing.Toggle(id);
        if (status)
        {
            Logger.LogInfo($"'{unit.unitName}' was added to the wing");
            Feedback.OnWingAdd();
        }
        else
        {
            Logger.LogInfo($"'{unit.unitName}' was removed from the wing");
            Feedback.OnWingRemove();
        }

        StateManager.TargetClaimIndex.Clear();
        MarkRenderer.Update();
    }

    private static void ToggleLockPrevention()
    {
        var newState = !NoWingmen.Config.PreventSelectedTargetsLock.Value;
        NoWingmen.Config.PreventSelectedTargetsLock.Value = newState;

        if (newState)
        {
            Logger.LogDebug("Lock prevention enabled");
        }
        else
        {
            Logger.LogInfo("Lock prevention disabled");
        }

        Feedback.OnLockPreventionToggle();
    }

    private static bool IsPressed(KeyboardShortcut shortcut)
    {
        return Input.GetKeyDown(shortcut.MainKey) && shortcut.Modifiers.All(Input.GetKey);
    }
}