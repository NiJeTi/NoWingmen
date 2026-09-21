using NuclearOption.UIStyleSystem;
using UnityEngine;

namespace NoWingmen.Theming;

internal static class PaletteFile
{
    private const string FileName = "NoWingmen.json";
    private const string ThemesFolder = "themes";

    [Serializable]
    private sealed class Dto
    {
        public string id = string.Empty;
        public string wing = string.Empty;
        public string teammate = string.Empty;
        public string targetedEnemy = string.Empty;
    }

    private static string PathFor(ThemeGroup group)
    {
        return Path.Combine(Application.persistentDataPath, ThemesFolder, group.name, FileName);
    }

    public static Palette? Read(ThemeGroup group)
    {
        var path = PathFor(group);

        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            var dto = JsonUtility.FromJson<Dto>(File.ReadAllText(path));
            if (dto == null)
            {
                Plugin.Logger.LogWarning($"Ignoring empty palette: {path}");
                return null;
            }

            if (!TryParse(dto.wing, out var wing) ||
                !TryParse(dto.teammate, out var teammate) ||
                !TryParse(dto.targetedEnemy, out var targetedEnemy))
            {
                Plugin.Logger.LogWarning($"Ignoring palette with invalid colors: {path}");
                return null;
            }

            return new Palette(wing, teammate, targetedEnemy);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Failed to read palette {path}: {e}");
            return null;
        }
    }

    public static void Write(ThemeGroup group, Palette palette)
    {
        var path = PathFor(group);

        try
        {
            var folder = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var dto = new Dto
            {
                id = group.Id,
                wing = ColorUtility.ToHtmlStringRGB(palette.Wing),
                teammate = ColorUtility.ToHtmlStringRGB(palette.Teammate),
                targetedEnemy = ColorUtility.ToHtmlStringRGB(palette.TargetedEnemy)
            };

            File.WriteAllText(path, JsonUtility.ToJson(dto, true));

            Plugin.Logger.LogDebug($"Saved palette: {path}");
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Failed to write palette {path}: {e}");
        }
    }

    private static bool TryParse(string hex, out Color color)
    {
        color = default;

        return !string.IsNullOrWhiteSpace(hex) &&
            ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : $"#{hex}", out color);
    }
}