using NoWingmen.Lines;
using NoWingmen.Marks;
using NoWingmen.Mfd;
using NoWingmen.Targets;
using NuclearOption.MissionEditorScripts;

namespace NoWingmen;

internal sealed class MissionState : IDisposable
{
    private readonly LineRenderer _lineRenderer;

    private WingScreen? _wingScreen;

    public static Stack<Action<MissionState>> OnCreateActions { get; } = new();

    public Wing Wing { get; }
    public TargetClaimIndex TargetClaimIndex { get; }
    public MarkColorResolver MarkColorResolver { get; }

    private MissionState(
        Wing wing,
        TargetClaimIndex targetClaimIndex,
        MarkColorResolver markColorResolver,
        LineRenderer lineRenderer
    )
    {
        Plugin.Settings.VisualsChanged += OnVisualsChanged;
        Plugin.Settings.ThemeChanged += OnThemeChanged;

        Wing = wing;
        Wing.PlayerToggled += OnWingPlayerToggled;

        TargetClaimIndex = targetClaimIndex;
        MarkColorResolver = markColorResolver;

        _lineRenderer = lineRenderer;
    }

    public static MissionState Create(Settings settings)
    {
        var wing = new Wing();

        var targetClaimIndex = new TargetClaimIndex(settings, wing);
        var markColorResolver = new MarkColorResolver(settings, wing, targetClaimIndex);

        var lineRenderer = new LineRenderer(targetClaimIndex);

        var state = new MissionState(
            wing,
            targetClaimIndex,
            markColorResolver,
            lineRenderer
        );

        while (OnCreateActions.TryPop(out var action))
        {
            try
            {
                action(state);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Mission state create action failed: {e}");
            }
        }

        return state;
    }

    public void Dispose()
    {
        Plugin.Settings.VisualsChanged -= OnVisualsChanged;
        Plugin.Settings.ThemeChanged -= OnThemeChanged;

        Wing.PlayerToggled -= OnWingPlayerToggled;

        _wingScreen?.Dispose();

        _lineRenderer.Clear();
    }

    public void Tick()
    {
        if (!InputFieldChecker.InsideInputField && Controls.IsToggleLockPreventionDown())
        {
            ToggleLockPrevention();
        }
    }

    public void LateTick()
    {
        if (TargetClaimIndex.Refresh())
        {
            MarkRenderer.Update();
        }

        _lineRenderer.Tick();

        _wingScreen?.Tick();
    }

    public void AttachWingScreen(VirtualMFD mfd)
    {
        if (mfd == null)
        {
            return;
        }

        _wingScreen?.Dispose();
        _wingScreen = WingScreen.Create(Wing, mfd);
    }

    private static void ToggleLockPrevention()
    {
        var newValue = !Plugin.Settings.LockPreventionEnabled.Value;

        Plugin.Settings.LockPreventionEnabled.Value = newValue;

        Feedback.OnLockPreventionToggle(newValue);

        Plugin.Logger.LogDebug($"Lock prevention {(newValue ? "enabled" : "disabled")}");
    }

    private void OnWingPlayerToggled(bool state)
    {
        TargetClaimIndex.Clear();
        MarkRenderer.Update();
        _wingScreen?.OnWingChanged();
        Feedback.OnWingPlayerToggled(state);
    }

    private void OnVisualsChanged(object sender, EventArgs eventArgs)
    {
        TargetClaimIndex.Clear();
        _lineRenderer.Clear();
        MarkRenderer.Update();
        _wingScreen?.OnSettingsChanged();
    }

    private static void OnThemeChanged()
    {
        MarkRenderer.Update();
    }
}