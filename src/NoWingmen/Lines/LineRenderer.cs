using NoWingmen.Targets;
using UnityEngine;

namespace NoWingmen.Lines;

internal sealed class LineRenderer
{
    private const float Thickness = 2f;
    private const float Alpha = 0.6f;

    private readonly TargetClaimIndex _targetClaimIndex;
    private readonly Stack<Line> _pool = [];

    public LineRenderer(TargetClaimIndex targetClaimIndex)
    {
        _targetClaimIndex = targetClaimIndex;
    }

    public void Update()
    {
        var map = SceneSingleton<DynamicMap>.i;
        if (map == null)
        {
            return;
        }

        if (!map.gameObject.activeInHierarchy)
        {
            return;
        }

        var mapScale = map.mapImage.transform.localScale.x;
        if (mapScale <= 0f)
        {
            return;
        }

        var thickness = Thickness / mapScale;

        var drawn = new List<Line>();

        foreach (var claim in _targetClaimIndex.Index)
        {
            if (!DynamicMap.TryGetMapIcon(claim.Claimer, out var claimerIcon))
            {
                continue;
            }

            if (!DynamicMap.TryGetMapIcon(claim.Target, out var targetIcon))
            {
                continue;
            }

            var color = claimerIcon.iconImage.color;
            color.a *= Alpha;

            var line = Draw(map, claimerIcon, targetIcon, color, thickness);
            drawn.Add(line);
        }

        foreach (var line in _pool)
        {
            line.Hide();
        }

        foreach (var line in drawn)
        {
            _pool.Push(line);
        }
    }

    public void Clear()
    {
        foreach (var line in _pool)
        {
            line.Destroy();
        }

        _pool.Clear();
    }

    private Line Draw(DynamicMap map, UnitMapIcon source, UnitMapIcon target, Color color, float thickness)
    {
        if (!_pool.TryPop(out var line))
        {
            var instance = UnityEngine.Object.Instantiate(map.mapWaypointVector, map.iconLayer.transform);
            instance.transform.SetAsFirstSibling();

            line = new Line(instance);
        }

        line.Draw(
            source.transform.localPosition,
            target.transform.localPosition,
            color, thickness
        );

        return line;
    }
}