using NoWingmen.Targets;
using NuclearOption.Networking;
using UnityEngine;

namespace NoWingmen.Marks;

internal sealed class MarkColorResolver
{
    private readonly Settings _settings;
    private readonly Wing _wing;
    private readonly TargetClaimIndex _targetClaimIndex;

    public MarkColorResolver(
        Settings settings,
        Wing wing,
        TargetClaimIndex targetClaimIndex
    )
    {
        _settings = settings;
        _wing = wing;
        _targetClaimIndex = targetClaimIndex;
    }

    public bool TryResolve(Unit unit, out Color color)
    {
        color = default;

        if (unit == null)
        {
            return false;
        }

        return TryResolveIdentity(unit, out color) || TryResolveClaim(unit, out color);
    }

    private bool TryResolveIdentity(Unit unit, out Color color)
    {
        color = default;

        var player = unit.GetPlayer();
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

        MarkCategory category;
        if (_settings.ShowWing.Value && _wing.Check(player))
        {
            category = MarkCategory.Wing;
        }
        else if (_settings.ShowTeammates.Value)
        {
            category = MarkCategory.Teammate;
        }
        else
        {
            return false;
        }

        color = GetColor(category);
        return true;
    }

    private bool TryResolveClaim(Unit unit, out Color color)
    {
        color = default;

        if (!_targetClaimIndex.IsClaimed(unit))
        {
            return false;
        }

        color = GetColor(MarkCategory.ClaimedTarget);
        return true;
    }

    public static Color GetColor(MarkCategory category)
    {
        return Plugin.Settings.Palette.Active.GetColor(category);
    }
}