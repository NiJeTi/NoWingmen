using NuclearOption.Networking;

namespace NoWingmen;

internal sealed class Wing
{
    private readonly HashSet<ulong> _ids = [];

    public event Action<bool>? PlayerToggled;

    public IReadOnlyCollection<ulong> Ids => _ids;

    public static bool IsEligible(Player player)
    {
        return !Identity.IsLocalPlayer(player) && Identity.IsSameFaction(player);
    }

    public bool Check(Player player)
    {
        return _ids.Contains(player.SteamID);
    }

    public void Toggle(Player player)
    {
        var id = player.SteamID;

        var state = !_ids.Remove(id);
        if (state)
        {
            _ids.Add(id);
        }

        PlayerToggled?.Invoke(state);
    }

    public void Remove(ulong id)
    {
        if (!_ids.Remove(id))
        {
            return;
        }

        PlayerToggled?.Invoke(false);
    }
}