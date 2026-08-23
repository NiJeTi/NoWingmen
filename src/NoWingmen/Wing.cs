namespace NoWingmen;

internal sealed class Wing
{
    private readonly HashSet<ulong> _ids = [];

    public bool Check(ulong id)
    {
        return _ids.Contains(id);
    }

    public void Clear()
    {
        _ids.Clear();
    }

    public bool Toggle(ulong steamId)
    {
        if (_ids.Remove(steamId))
        {
            return false;
        }

        _ids.Add(steamId);
        return true;
    }
}