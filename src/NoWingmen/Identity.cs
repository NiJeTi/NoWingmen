using NuclearOption.Networking;
using Steamworks;

namespace NoWingmen;

internal static class Identity
{
    public static bool IsSameFaction(Unit unit)
    {
        var map = GetMap();

        var localHq = map.HQ;
        return localHq != null && localHq == unit.NetworkHQ;
    }

    public static bool IsSameFaction(Player player)
    {
        var map = GetMap();

        var localHq = map.HQ;
        return localHq != null && localHq == player.HQ;
    }

    public static bool IsLocalPlayer(Player player)
    {
        return GameManager.IsLocalPlayer(player);
    }

    public static string GetDisplayName(ulong id)
    {
        return Player.GetPlayerNameBySteamID((CSteamID)id).GetDisplayName(PlayerNameContext.Other);
    }

    private static DynamicMap GetMap()
    {
        return SceneSingleton<DynamicMap>.i ?? throw new InvalidOperationException($"{nameof(DynamicMap)} is null.");
    }
}