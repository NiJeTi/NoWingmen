namespace NoWingmen.Targets;

public readonly struct TargetClaim(Unit claimer, Unit target)
{
    public Unit Claimer { get; } = claimer;
    public Unit Target { get; } = target;
}