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
    public static MissionState? MissionState { get; private set; }

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
        MissionState?.Dispose();
        MissionState = null;

        _harmony.UnpatchSelf();
    }

    private void Update()
    {
        MissionState?.Tick();
    }

    private void LateUpdate()
    {
        UpdateState();

        MissionState?.LateTick();
    }

    private void UpdateState()
    {
        if (MissionTracker.HasChanged())
        {
            MissionState?.Dispose();
            MissionState = null;
        }

        if (MissionState != null)
        {
            return;
        }

        if (!MissionTracker.InMission)
        {
            return;
        }

        MissionState = MissionState.Create(Settings, _controls);

        Logger.LogDebug("Mission state created");
    }
}