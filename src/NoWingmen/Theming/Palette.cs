using NoWingmen.Marks;
using UnityEngine;

namespace NoWingmen.Theming;

internal sealed class Palette
{
    public static readonly Palette Default = new(
        new Color(1f, 0.75f, 0f),
        new Color(0.03f, 0.85f, 0.66f),
        new Color(0.8f, 0.3f, 1f)
    );

    public Color Wing { get; }
    public Color Teammate { get; }
    public Color TargetedEnemy { get; }

    public Palette(Color wing, Color teammate, Color targetedEnemy)
    {
        Wing = wing;
        Teammate = teammate;
        TargetedEnemy = targetedEnemy;
    }

    public Color GetColor(MarkCategory category)
    {
        return category switch
        {
            MarkCategory.Wing => Wing,
            MarkCategory.Teammate => Teammate,
            MarkCategory.ClaimedTarget => TargetedEnemy,
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, "Invalid category."),
        };
    }
}