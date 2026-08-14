namespace NoWingmen.Targets;

internal sealed class TargetClaimIndex
{
    private readonly Wing _wing;

    private readonly List<TargetClaim> _index = new();
    private readonly HashSet<Unit> _lockPrevented = new();

    private Dictionary<Unit, Unit> _current = new();
    private Dictionary<Unit, Unit> _previous = new();

    public TargetClaimIndex(Wing wing)
    {
        _wing = wing;
    }

    public IReadOnlyCollection<TargetClaim> Index => _index;

    public bool IsClaimed(Unit target)
    {
        return _current.ContainsKey(target);
    }

    public bool IsLockPrevented(Unit target)
    {
        return _lockPrevented.Contains(target);
    }

    public bool Refresh()
    {
        _index.Clear();
        _lockPrevented.Clear();

        (_current, _previous) = (_previous, _current);
        _current.Clear();

        Collect();

        return IndexChanged();
    }

    public void Clear()
    {
        _index.Clear();
        _lockPrevented.Clear();

        _current.Clear();
        _previous.Clear();
    }

    private void Collect()
    {
        foreach (var claimer in UnitRegistry.allAircraft)
        {
            if (!IsClaimer(claimer))
            {
                continue;
            }

            var preventsLock = IsWingMember(claimer);

            foreach (var target in claimer.weaponManager.GetTargetList()
                .Where(target => !Identity.IsSameFaction(target)))
            {
                _index.Add(new TargetClaim(claimer, target));
                _current[target] = claimer;

                if (preventsLock)
                {
                    _lockPrevented.Add(target);
                }
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

        if (Config.ShowTeammatesTargetSelection.Value)
        {
            return true;
        }

        return Config.ShowWingTargetSelection.Value && IsWingMember(aircraft);
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