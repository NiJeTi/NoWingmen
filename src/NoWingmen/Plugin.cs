using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using NoWingmen.Marks;
using NuclearOption.MissionEditorScripts;

namespace NoWingmen;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Controls.InputFrameworkGuid)]
internal sealed class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }
    public static StateManager StateManager { get; } = new();

    private Harmony _harmony;

    private void Awake()
    {
        Logger = base.Logger;

        NoWingmen.Config.Bind(
            Config, () =>
            {
                StateManager?.TargetClaimIndex.Clear();
                StateManager?.LineRenderer.Clear();
                MarkRenderer.Update();
            }
        );

        Controls.Register();

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
    }

    private void Update()
    {
        if (!StateManager.InGame)
        {
            return;
        }

        if (InputFieldChecker.InsideInputField)
        {
            return;
        }

        if (Controls.Pressed(Controls.ActionToggleWing))
        {
            ToggleWing();
        }
        else if (Controls.Pressed(Controls.ActionToggleLockPrevention))
        {
            ToggleLockPrevention();
        }
    }

    private void LateUpdate()
    {
        if (StateManager.MissionTracker.Changed())
        {
            Logger.LogDebug("State reset on mission change");
            StateManager.Reset();
            MarkRenderer.Update();
        }

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
            Reject("No unit selected");
            return;
        }

        if (!Identity.IsSameFaction(unit))
        {
            Reject($"<b>{Identity.GetDisplayName(unit)}</b> is in another faction");
            return;
        }

        if (!Identity.TryGetId(unit, out var id))
        {
            Reject($"<b>{unit.unitName}</b> is not a player");
            return;
        }

        var status = StateManager.Wing.Toggle(id);
        if (status)
        {
            Logger.LogInfo($"'{unit.unitName}' was added to the wing");
            Feedback.OnWingAdd(unit);
        }
        else
        {
            Logger.LogInfo($"'{unit.unitName}' was removed from the wing");
            Feedback.OnWingRemove(unit);
        }

        StateManager.TargetClaimIndex.Clear();
        MarkRenderer.Update();
    }

    private static void Reject(string reason)
    {
        Logger.LogDebug(reason);
        Feedback.OnWingReject(reason);
    }

    private static void ToggleLockPrevention()
    {
        var newState = !StateManager.LockPreventionEnabled;
        StateManager.LockPreventionEnabled = newState;

        Logger.LogDebug($"Lock prevention {(newState ? "enabled" : "disabled")}");

        Feedback.OnLockPreventionToggle(newState);
    }
}