using HarmonyLib;
using NuclearOption.UIStyleSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NoWingmen.Mfd;

internal sealed class ScreenRow : IDisposable
{
    public const float Height = 24f;

    private const float LabelInset = 8f;

    private const float CheckSize = 24f;

    private const float CheckSpacing = 8f;

    private const string CheckLabel = "X";

    private static readonly AccessTools.FieldRef<MapOptions_ToggleButton, TextMeshProUGUI> LabelRef =
        AccessTools.FieldRefAccess<MapOptions_ToggleButton, TextMeshProUGUI>("label");

    private static readonly AccessTools.FieldRef<MapOptions_ToggleButton, MonoBehaviour?> ScriptRef =
        AccessTools.FieldRefAccess<MapOptions_ToggleButton, MonoBehaviour?>("script");

    private static readonly AccessTools.FieldRef<MapOptions_ToggleButton, string> VariableNameRef =
        AccessTools.FieldRefAccess<MapOptions_ToggleButton, string>("variableName");

    private static readonly AccessTools.FieldRef<MapOptions_ToggleButton, List<MapOptions_ToggleButton>>
        OtherButtonsRef =
            AccessTools.FieldRefAccess<MapOptions_ToggleButton, List<MapOptions_ToggleButton>>("otherButtons");

    private readonly GameObject _root;
    private readonly MapOptions_ToggleButton _button;
    private readonly Func<bool>? _getState;

    private ScreenRow(GameObject root, MapOptions_ToggleButton button, Func<bool>? getState = null)
    {
        _root = root;
        _button = button;
        _getState = getState;
    }

    public static ScreenRow CreateToggle(
        Transform container,
        MapOptions_ToggleButton template,
        string label,
        Func<bool> getState,
        Action onToggle
    )
    {
        var button = Clone(container, template, $"Toggle: {label}");

        Style(LabelRef(button), label, TextAlignmentOptions.Center, LabelInset);
        Bind(button, getState(), onToggle);

        return new ScreenRow(button.gameObject, button, getState);
    }

    public static ScreenRow CreateRosterRow(
        Transform container,
        MapOptions_ToggleButton template,
        string playerName,
        Action onClick
    )
    {
        var line = CreateLine(container, $"Roster: {playerName}");

        var check = Clone(line.transform, template, $"Remove: {playerName}");

        Style(LabelRef(check), CheckLabel, TextAlignmentOptions.Center, 0f);
        Stretch(check.gameObject, CheckSize, 0f);
        Bind(check, true, onClick);

        var name = Object.Instantiate(LabelRef(template), line.transform);
        name.gameObject.name = $"NoWingmen.WingScreen.Name: {playerName}";
        name.color = ThemeManager.Active.ColorTheme.AllClear;

        var nameRect = (RectTransform)name.transform;
        nameRect.sizeDelta = new Vector2(nameRect.sizeDelta.x, Height);

        Style(name, playerName, TextAlignmentOptions.Left, 0f);
        Stretch(name.gameObject, 0f, 1f);

        return new ScreenRow(line, check);
    }

    public void Dispose()
    {
        Object.DestroyImmediate(_root);
    }

    public void UpdateState()
    {
        if (_getState != null)
        {
            _button.Set(_getState());
        }
    }

    private static GameObject CreateLine(Transform container, string name)
    {
        var line = new GameObject($"NoWingmen.WingScreen.{name}", typeof(RectTransform));
        var rect = (RectTransform)line.transform;

        rect.SetParent(container, false);
        rect.sizeDelta = new Vector2(0f, Height);

        var columns = line.AddComponent<HorizontalLayoutGroup>();

        columns.childAlignment = TextAnchor.MiddleLeft;
        columns.spacing = CheckSpacing;
        columns.childControlWidth = true;
        columns.childForceExpandWidth = false;
        columns.childControlHeight = false;
        columns.childForceExpandHeight = false;

        return line;
    }

    private static MapOptions_ToggleButton Clone(Transform container, MapOptions_ToggleButton template, string name)
    {
        var button = Object.Instantiate(template, container);
        button.gameObject.name = $"NoWingmen.WingScreen.{name}";

        ScriptRef(button) = null;
        VariableNameRef(button) = string.Empty;
        OtherButtonsRef(button) = [];

        return button;
    }

    private static void Style(TextMeshProUGUI text, string content, TextAlignmentOptions alignment, float inset)
    {
        text.text = content;
        text.alignment = alignment;
        text.margin = new Vector4(inset, 0f, inset, 0f);
    }

    private static void Stretch(GameObject target, float width, float flexible)
    {
        var element = target.AddComponent<LayoutElement>();

        element.minWidth = width;
        element.preferredWidth = width;
        element.flexibleWidth = flexible;
    }

    private static void Bind(MapOptions_ToggleButton button, bool state, Action onToggle)
    {
        button.Set(state);

        button.OnToggleMethod = new MapOptions_ToggleButton.CallBackEvent();
        button.OnToggleMethod.AddListener(() => onToggle());

        button.gameObject.SetActive(true);
    }
}
