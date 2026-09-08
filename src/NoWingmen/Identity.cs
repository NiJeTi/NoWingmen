using NuclearOption.Networking;

namespace NoWingmen;

internal static class Identity
{
    public static bool TryGetId(Unit unit, out ulong id)
    {
        id = 0;

        if (unit == null)
        {
            return false;
        }

        var player = unit.GetPlayer();
        if (player == null)
        {
            return false;
        }

        id = player.SteamID;
        return id != 0;
    }

    public static string GetDisplayName(Unit unit)
    {
        if (unit == null)
        {
            return string.Empty;
        }

        var player = unit.GetPlayer();

        return player != null ? player.GetDisplayName(PlayerNameContext.Other) : unit.unitName;
    }

    public static bool TryCompareFaction(Unit unit, out bool same)
    {
        same = false;

        var localHq = SceneSingleton<DynamicMap>.i?.HQ;
        if (localHq == null)
        {
            return false;
        }

        var unitHq = unit?.NetworkHQ;
        if (unitHq == null)
        {
            return false;
        }

        same = localHq == unitHq;
        return true;
    }

    public static bool IsSameFaction(Unit unit)
    {
        return TryCompareFaction(unit, out var same) && same;
    }
}