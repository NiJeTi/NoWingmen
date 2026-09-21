using NoWingmen.Lines;
using NoWingmen.Marks;
using NoWingmen.Mfd;
using NoWingmen.Targets;
using NuclearOption.MissionEditorScripts;

namespace NoWingmen;

internal sealed class State : IDisposable
{
    private const bool DefaultLockPreventionEnabled = true;

    private readonly Controls _controls;
    private readonly LineRenderer _lineRenderer;

    private WingScreen? _wingScreen;

    public static Stack<Action<State>> OnCreateActions { get; } = new();

    public Wing Wing { get; }
    public TargetClaimIndex TargetClaimIndex { get; }
    public MarkColorResolver MarkColorResolver { get; }

    public bool LockPreventionEnabled { get; private set; } = DefaultLockPreventionEnabled;

    private State(
        Controls controls,
        Wing wing,
        TargetClaimIndex targetClaimIndex,
        MarkColorResolver markColorResolver,
        LineRenderer lineRenderer
    )
    {
        Plugin.Settings.VisualsChanged += OnVisualsChanged;
        Plugin.Settings.ThemeChanged += OnThemeChanged;

        _controls = controls;

        Wing = wing;
        Wing.PlayerToggled += OnWingPlayerToggled;

        TargetClaimIndex = targetClaimIndex;
        MarkColorResolver = markColorResolver;

        _lineRenderer = lineRenderer;
    }

    public static State Create(Settings settings, Controls controls)
    {
        var wing = new Wing();

        var targetClaimIndex = new TargetClaimIndex(settings, wing);
        var markColorResolver = new MarkColorResolver(settings, wing, targetClaimIndex);

        var lineRenderer = new LineRenderer(targetClaimIndex);

        var state = new State(
            controls,
            wing,
            targetClaimIndex,
            markColorResolver,
            lineRenderer
        );

        while (OnCreateActions.TryPop(out var action))
        {
            action(state);
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
        if (!InputFieldChecker.InsideInputField && _controls.IsToggleLockPreventionDown())
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
        _wingScreen?.Dispose();
        _wingScreen = WingScreen.Create(Wing, mfd);
    }

    private void ToggleLockPrevention()
    {
        LockPreventionEnabled = !LockPreventionEnabled;
        Feedback.OnLockPreventionToggle(LockPreventionEnabled);

        Plugin.Logger.LogDebug($"Lock prevention {(LockPreventionEnabled ? "enabled" : "disabled")}");
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