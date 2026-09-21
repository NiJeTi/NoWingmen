using HarmonyLib;
using NuclearOption.UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NoWingmen.Theming;

internal sealed class PaletteSection : IDisposable
{
    private const string Title = "NoWingmen Marks";

    private const string WingLabel = "Wing";
    private const string TeammateLabel = "Teammate";
    private const string TargetedEnemyLabel = "Targeted Enemy";

    private const string WingTooltip =
        "Color of aircraft flown by members of your wing, on the HUD, the map and the leaderboard.";

    private const string TeammateTooltip =
        "Color of aircraft flown by other players in your faction.";

    private const string TargetedEnemyTooltip =
        "Color of enemies already selected by your wing or your teammates, and of the lines drawn to them on the map.";

    private static readonly AccessTools.FieldRef<AccessibilityMenu, ColorPicker> ColorPickerPrefabRef =
        AccessTools.FieldRefAccess<AccessibilityMenu, ColorPicker>("colorPickerPrefab");

    private static readonly AccessTools.FieldRef<AccessibilityMenu, CategoryTitle> CategoryTitlePrefabRef =
        AccessTools.FieldRefAccess<AccessibilityMenu, CategoryTitle>("categoryTitlePrefab");

    private static readonly AccessTools.FieldRef<AccessibilityMenu, Transform> MenuThemeTransformRef =
        AccessTools.FieldRefAccess<AccessibilityMenu, Transform>("menuThemeTransform");

    private static readonly AccessTools.FieldRef<AccessibilityMenu, SliderToggle> SliderToggleRef =
        AccessTools.FieldRefAccess<AccessibilityMenu, SliderToggle>("themeGroupSliderToggle");

    private static readonly AccessTools.FieldRef<AccessibilityMenu, bool> IsThemeEditedRef =
        AccessTools.FieldRefAccess<AccessibilityMenu, bool>("isThemeEdited");

    private readonly AccessibilityMenu _menu;
    private readonly GameObject _container;
    private readonly ColorPicker[] _pickers;

    private readonly Action _refreshThemeName;
    private readonly Action _refreshButtons;
    private readonly Action<string> _hoverIn;
    private readonly Action<string> _hoverOut;

    private PaletteSection(
        AccessibilityMenu menu,
        GameObject container,
        ColorPicker[] pickers
    )
    {
        _menu = menu;
        _container = container;
        _pickers = pickers;

        _refreshThemeName = AccessTools.MethodDelegate<Action>(
            AccessTools.Method(typeof(AccessibilityMenu), "RefreshThemeName"), menu
        );
        _refreshButtons = AccessTools.MethodDelegate<Action>(
            AccessTools.Method(typeof(AccessibilityMenu), "RefreshButtons"), menu
        );
        _hoverIn = AccessTools.MethodDelegate<Action<string>>(
            AccessTools.Method(typeof(AccessibilityMenu), "OnColorNameHoverIn"), menu
        );
        _hoverOut = AccessTools.MethodDelegate<Action<string>>(
            AccessTools.Method(typeof(AccessibilityMenu), "OnColorNameHoveredOut"), menu
        );

        foreach (var picker in _pickers)
        {
            picker.OnColorChanged += OnColorChanged;
            picker.OnColorNameHoverIn += _hoverIn;
            picker.OnColorNameHoverOut += _hoverOut;
        }
    }

    public void Dispose()
    {
        foreach (var picker in _pickers)
        {
            picker.OnColorChanged -= OnColorChanged;
            picker.OnColorNameHoverIn -= _hoverIn;
            picker.OnColorNameHoverOut -= _hoverOut;
        }

        if (_container != null)
        {
            Object.Destroy(_container);
        }
    }

    public bool Owns(AccessibilityMenu menu)
    {
        return _menu == menu;
    }

    public static PaletteSection Create(AccessibilityMenu menu)
    {
        var prefab = ColorPickerPrefabRef(menu);

        var container = CreateContainer(menu);

        SetTitle(menu, container);

        var pickers = new[]
        {
            Object.Instantiate(prefab, container.transform),
            Object.Instantiate(prefab, container.transform),
            Object.Instantiate(prefab, container.transform)
        };

        var section = new PaletteSection(menu, container, pickers);

        Plugin.Logger.LogInfo("Palette section created.");

        return section;
    }

    public void Refresh()
    {
        var palette = Plugin.Settings.Palette.Active;

        _pickers[0].SetValues(WingLabel, palette.Wing, WingTooltip);
        _pickers[1].SetValues(TeammateLabel, palette.Teammate, TeammateTooltip);
        _pickers[2].SetValues(TargetedEnemyLabel, palette.TargetedEnemy, TargetedEnemyTooltip);

        var toggle = SliderToggleRef(_menu);
        _container.SetActive(toggle != null && toggle.isOn);

        if (_container.transform.parent is RectTransform parent)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        }
    }

    public Palette Commit()
    {
        return new Palette(
            _pickers[0].Color.WithAlpha(1f),
            _pickers[1].Color.WithAlpha(1f),
            _pickers[2].Color.WithAlpha(1f)
        );
    }

    private static GameObject CreateContainer(AccessibilityMenu menu)
    {
        var template = MenuThemeTransformRef(menu);

        var wasActive = template.gameObject.activeSelf;
        template.gameObject.SetActive(false);

        var container = Object.Instantiate(template.gameObject, template.parent);
        template.gameObject.SetActive(wasActive);

        container.name = "NoWingmen.PaletteSection";
        container.transform.SetSiblingIndex(template.GetSiblingIndex() + 1);

        for (var i = container.transform.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(container.transform.GetChild(i).gameObject);
        }

        container.SetActive(true);

        return container;
    }

    private static void SetTitle(AccessibilityMenu menu, GameObject container)
    {
        var prefab = CategoryTitlePrefabRef(menu);

        var title = Object.Instantiate(prefab, container.transform);
        title.name = "NoWingmen.PaletteSection.Title";
        title.Text = Title;
        title.transform.SetSiblingIndex(0);
    }

    private void OnColorChanged(Color color)
    {
        IsThemeEditedRef(_menu) = true;

        _refreshThemeName();
        _refreshButtons();
    }
}