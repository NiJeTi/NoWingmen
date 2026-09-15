using NuclearOption.SavedMission;

namespace NoWingmen;

internal static class MissionTracker
{
    private static Mission? _mission;

    public static bool InMission => MissionManager.IsRunning && Current() != null;

    public static bool HasChanged()
    {
        var mission = Current();

        if (ReferenceEquals(mission, _mission))
        {
            return false;
        }

        _mission = mission;

        return true;
    }

    private static Mission? Current()
    {
        var mission = MissionManager.CurrentMission;

        return ReferenceEquals(mission, Mission.NullMission) ? null : mission;
    }
}