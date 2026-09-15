using NuclearOption.Networking;

namespace NoWingmen.Targets;

internal sealed class TargetClaimIndex
{
    private readonly Settings _settings;
    private readonly Wing _wing;

    private readonly List<TargetClaim> _index = [];

    private Dictionary<Unit, Unit> _current = new();
    private Dictionary<Unit, Unit> _previous = new();

    private bool _cleared;

    public IReadOnlyCollection<TargetClaim> Index => _index;

    public TargetClaimIndex(Settings settings, Wing wing)
    {
        _settings = settings;
        _wing = wing;
    }

    public bool IsClaimed(Unit target)
    {
        return _current.ContainsKey(target);
    }

    public bool Refresh()
    {
        _index.Clear();

        (_current, _previous) = (_previous, _current);
        _current.Clear();

        Collect();

        var changed = _cleared || IndexChanged();
        _cleared = false;

        return changed;
    }

    public void Clear()
    {
        _index.Clear();

        _current.Clear();
        _previous.Clear();

        _cleared = true;
    }

    private void Collect()
    {
        foreach (var claimer in UnitRegistry.allAircraft.Where(IsClaimer))
        {
            foreach (var target in claimer.weaponManager.GetTargetList().Where(x => !Identity.IsSameFaction(x)))
            {
                _index.Add(new TargetClaim(claimer, target));
                _current[target] = claimer;
            }
        }
    }

    private bool IsClaimer(Aircraft aircraft)
    {
        if (aircraft.disabled)
        {
            return false;
        }

        var player = aircraft.GetPlayer();
        if (player == null)
        {
            return false;
        }

        if (Identity.IsLocalPlayer(player))
        {
            return false;
        }

        if (!Identity.IsSameFaction(player))
        {
            return false;
        }

        if (_settings.ShowTeammatesTargetSelection.Value)
        {
            return true;
        }

        if (_settings.ShowWingTargetSelection.Value && _wing.Check(player))
        {
            return true;
        }

        return false;
    }

    private bool IndexChanged()
    {
        if (_current.Count != _previous.Count)
        {
            return true;
        }

        foreach (var claim in _current)
        {
            if (!_previous.TryGetValue(claim.Key, out var claimer) || claimer != claim.Value)
            {
                return true;
            }
        }

        return false;
    }
}