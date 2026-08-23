using NoWingmen.Targets;
using UnityEngine;

namespace NoWingmen.Marks;

internal sealed class MarkColorResolver
{
    private readonly FriendList _friendList;
    private readonly Wing _wing;
    private readonly TargetClaimIndex _targetClaimIndex;

    public MarkColorResolver(
        FriendList friendList,
        Wing wing,
        TargetClaimIndex targetClaimIndex
    )
    {
        _friendList = friendList;
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

    private bool TryResolveClaim(Unit unit, out Color color)
    {
        color = default;

        if (!_targetClaimIndex.IsClaimed(unit))
        {
            return false;
        }

        color = Config.GetCategoryColor(MarkCategory.ClaimedTarget);
        return true;
    }

    private bool TryResolveIdentity(Unit unit, out Color color)
    {
        color = default;

        if (!Identity.TryGetId(unit, out var steamId))
        {
            return false;
        }

        if (!Identity.IsSameFaction(unit))
        {
            return false;
        }

        var isFriend = Config.IsCategoryVisible(MarkCategory.Friend) && _friendList.Check(steamId);
        var isWing = Config.IsCategoryVisible(MarkCategory.Wing) && _wing.Check(steamId);

        var category = isWing ? MarkCategory.Wing : isFriend ? MarkCategory.Friend : MarkCategory.None;
        if (category == MarkCategory.None)
        {
            return false;
        }

        color = Config.GetCategoryColor(category);
        return true;
    }
}