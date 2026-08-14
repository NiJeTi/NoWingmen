using Steamworks;

namespace NoWingmen;

internal sealed class FriendList
{
    private readonly Dictionary<ulong, bool> _cache = [];

    public bool Check(ulong id)
    {
        if (id == 0)
        {
            return false;
        }

        if (!SteamManager.ClientInitialized)
        {
            return false;
        }

        if (_cache.TryGetValue(id, out var isFriend))
        {
            return isFriend;
        }

        isFriend = SteamFriends.GetFriendRelationship((CSteamID)id) == EFriendRelationship.k_EFriendRelationshipFriend;
        _cache[id] = isFriend;

        return isFriend;
    }
}