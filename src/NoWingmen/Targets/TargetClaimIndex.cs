namespace NoWingmen.Targets;

internal sealed class TargetClaimIndex
{
    private readonly Wing _wing;

    private readonly List<TargetClaim> _index = new();

    private Dictionary<Unit, Unit> _current = new();
    private Dictionary<Unit, Unit> _previous = new();

    private bool _cleared;

    public TargetClaimIndex(Wing wing)
    {
        _wing = wing;
    }

    public IReadOnlyCollection<TargetClaim> Index => _index;

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

        if (GameManager.IsLocalAircraft(aircraft))
        {
            return false;
        }

        if (!Identity.IsSameFaction(aircraft))
        {
            return false;
        }

        if (!Identity.IsPlayer(aircraft))
        {
            return false;
        }

        if (Config.ShowTeammatesTargetSelection.Value)
        {
            return true;
        }

        if (Config.ShowWingTargetSelection.Value && IsWingMember(aircraft))
        {
            return true;
        }

        return false;
    }

    private bool IsWingMember(Aircraft aircraft)
    {
        return Identity.TryGetId(aircraft, out var id) && _wing.Check(id);
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