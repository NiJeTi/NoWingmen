using NuclearOption.UIStyleSystem;

namespace NoWingmen.Theming;

internal static class PaletteMenu
{
    private static PaletteSection? _section;

    public static void Build(AccessibilityMenu menu)
    {
        _section?.Dispose();

        _section = PaletteSection.Create(menu);
        _section.Refresh();
    }

    public static void Refresh(AccessibilityMenu menu)
    {
        if (_section == null)
        {
            return;
        }

        if (!_section.Owns(menu))
        {
            return;
        }

        _section.Refresh();
    }

    public static void Commit()
    {
        if (_section == null)
        {
            return;
        }

        var group = ThemeManager.Active;
        if (group == null)
        {
            return;
        }

        Plugin.Settings.Palette.Set(group.Id, _section.Commit());
    }
}