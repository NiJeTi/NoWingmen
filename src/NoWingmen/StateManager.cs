using NoWingmen.Lines;
using NoWingmen.Marks;
using NoWingmen.Targets;

namespace NoWingmen;

internal sealed class StateManager
{
    public Unit CurrentSelection { get; private set; }
    public bool IsChatOpen { get; set; }

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