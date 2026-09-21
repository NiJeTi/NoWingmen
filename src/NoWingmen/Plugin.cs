using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace NoWingmen;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Controls.GuidInputFramework)]
internal sealed class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; } = null!;
    public static Settings Settings { get; private set; } = null!;
    public static State? State { get; private set; }

    private Harmony _harmony = null!;

    private Controls _controls = null!;

    private void Awake()
    {
        Logger = base.Logger;

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

        Settings = Settings.Init(Config);
        _controls = Controls.Init();

        Logger.LogInfo("Patch successful");
    }

    private void OnDestroy()
    {
        State?.Dispose();
        State = null;

        _harmony.UnpatchSelf();
    }

    private void Update()
    {
        State?.Tick();
    }

    private void LateUpdate()
    {
        UpdateState();

        State?.LateTick();
    }

    private void UpdateState()
    {
        if (MissionTracker.HasChanged())
        {
            State?.Dispose();
            State = null;
        }

        if (State != null)
        {
            return;
        }

        if (!MissionTracker.InMission)
        {
            return;
        }

        State = State.Create(Settings, _controls);

        Logger.LogDebug("Mission state created");
    }
}