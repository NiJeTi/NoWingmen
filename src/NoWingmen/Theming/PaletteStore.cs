using NuclearOption.UIStyleSystem;

namespace NoWingmen.Theming;

internal sealed class PaletteStore
{
    private readonly Dictionary<string, Palette> _palettes = new();

    public Palette Active => Get(ThemeManager.Active);

    public void Set(string id, Palette palette)
    {
        _palettes[id] = palette;
    }

    public void Save(ThemeGroup group)
    {
        if (group.Origin == ThemeGroup.ThemeOrigin.Scriptable_Object)
        {
            return;
        }

        PaletteFile.Write(group, Get(group));
    }

    public void Copy(ThemeGroup from, string toId)
    {
        _palettes[toId] = Get(from);
    }

    public void Drop(string id)
    {
        _palettes.Remove(id);
    }

    private Palette Get(ThemeGroup? group)
    {
        if (group == null)
        {
            return Palette.Default;
        }

        if (_palettes.TryGetValue(group.Id, out var cached))
        {
            return cached;
        }

        var palette = PaletteFile.Read(group) ?? Palette.Default;
        _palettes[group.Id] = palette;

        return palette;
    }
}