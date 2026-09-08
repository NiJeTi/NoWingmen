using NoWingmen.Lines;
using NoWingmen.Marks;
using NoWingmen.Targets;

namespace NoWingmen;

internal sealed class StateManager
{
    private const bool DefaultLockPreventionEnabled = true;
    
    public bool InGame =>
        GameManager.gameState == GameState.SinglePlayer ||
        GameManager.gameState == GameState.Multiplayer;

    public Unit CurrentSelection { get; private set; }
    public bool LockPreventionEnabled { get; set; } = DefaultLockPreventionEnabled;

    public FriendList FriendList { get; } = new();
    public Wing Wing { get; } = new();
    public MissionTracker MissionTracker { get; } = new();

    public TargetClaimIndex TargetClaimIndex { get; }
    public LineRenderer LineRenderer { get; }
    public MarkColorResolver MarkColorResolver { get; }

    public StateManager()
    {
        TargetClaimIndex = new TargetClaimIndex(Wing);
        LineRenderer = new LineRenderer(TargetClaimIndex);
        MarkColorResolver = new MarkColorResolver(FriendList, Wing, TargetClaimIndex);
    }

    public void Reset()
    {
        CurrentSelection = null;
        LockPreventionEnabled = DefaultLockPreventionEnabled;

        Wing.Clear();
        TargetClaimIndex.Clear();
        LineRenderer.Clear();
    }

    public void Select(Unit unit)
    {
        if (unit == null)
        {
            return;
        }

        CurrentSelection = unit;
    }

    public void Deselect(Unit unit)
    {
        if (CurrentSelection != unit)
        {
            return;
        }

        CurrentSelection = null;
    }
}