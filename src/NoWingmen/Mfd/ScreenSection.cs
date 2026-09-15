using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NoWingmen.Mfd;

internal sealed class ScreenSection
{
    private const float SectionPadding = 24f;
    private const float RowSpacing = 4f;
    private const float ColumnSpacing = 8f;
    private const float GapHeight = 10f;
    private const int SidePadding = 8;

    private readonly RectTransform _root;
    private readonly RectTransform _container;
    private readonly TextMeshProUGUI _heading;

    private ScreenSection(RectTransform root, RectTransform container, TextMeshProUGUI heading)
    {
        _root = root;
        _container = container;
        _heading = heading;
    }

    public Transform Container => _container;

    public static TextMeshProUGUI FindHeading(Transform root)
    {
        foreach (Transform child in root)
        {
            var heading = child.GetComponent<TextMeshProUGUI>();
            if (heading != null)
            {
                return heading;
            }
        }

        throw new InvalidOperationException($"{root.name} has no heading.");
    }

    public static void StackFromTop(Transform panelContainer)
    {
        var sections = panelContainer.GetComponent<VerticalLayoutGroup>();
        if (sections != null)
        {
            sections.childAlignment = TextAnchor.UpperCenter;
        }
    }

    public static ScreenSection Claim(RectTransform container, TextMeshProUGUI heading, string title)
    {
        return new ScreenSection((RectTransform)container.parent, container, heading).Prepare(title);
    }

    public ScreenSection Clone(string title)
    {
        var root = Object.Instantiate(_root, _root.parent);
        var container = (RectTransform)root.Find(_container.name);

        return new ScreenSection(root, container, FindHeading(root)).Prepare(title);
    }

    public Transform AddRow()
    {
        var row = AddLine("Row", ScreenRow.Height);

        Stack<HorizontalLayoutGroup>(row.gameObject, ColumnSpacing);

        return row;
    }

    public Transform AddGap()
    {
        return AddLine("Gap", GapHeight);
    }

    public ScreenSection Fit()
    {
        var content = 0f;
        var lines = 0;

        foreach (RectTransform line in _container)
        {
            if (!line.gameObject.activeSelf)
            {
                continue;
            }

            content += line.sizeDelta.y;
            lines++;
        }

        if (lines > 1)
        {
            content += (lines - 1) * RowSpacing;
        }

        _container.sizeDelta = new Vector2(_container.sizeDelta.x, content);
        _root.sizeDelta = new Vector2(_root.sizeDelta.x, content + SectionPadding);

        LayoutRebuilder.MarkLayoutForRebuild((RectTransform)_root.parent);

        return this;
    }

    private ScreenSection Prepare(string title)
    {
        _root.name = $"NoWingmen.WingScreen.{title}";
        _heading.text = title;

        var existing = _container.GetComponent<LayoutGroup>();
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        Stack<VerticalLayoutGroup>(_container.gameObject, RowSpacing).padding =
            new RectOffset(SidePadding, SidePadding, 0, 0);

        return Fit();
    }

    private RectTransform AddLine(string name, float height)
    {
        var line = new GameObject($"NoWingmen.WingScreen.{name}", typeof(RectTransform));
        var rect = (RectTransform)line.transform;

        rect.SetParent(_container, false);
        rect.sizeDelta = new Vector2(0f, height);

        Fit();

        return rect;
    }

    private static T Stack<T>(GameObject target, float spacing) where T : HorizontalOrVerticalLayoutGroup
    {
        var layout = target.AddComponent<T>();

        layout.childAlignment = TextAnchor.UpperCenter;
        layout.spacing = spacing;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandHeight = false;

        return layout;
    }
}
