using NuclearOption.SavedMission;

namespace NoWingmen;

internal sealed class MissionTracker
{
    private Mission _mission;

    public bool Changed()
    {
        var mission = MissionManager.CurrentMission;
        if (ReferenceEquals(mission, _mission))
        {
            return false;
        }

        _mission = mission;
        return true;
    }
}
