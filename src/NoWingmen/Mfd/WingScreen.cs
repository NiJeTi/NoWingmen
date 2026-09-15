using BepInEx.Configuration;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NoWingmen.Mfd;

internal sealed class WingScreen : IDisposable
{
    private class Layout(
        VirtualMFD mfd,
        MFDScreen screen,
        ScreenSection team,
        ScreenSection wing,
        MapOptions_ToggleButton rowTemplate
    )
    {
        public VirtualMFD Mfd { get; } = mfd;
        public MFDScreen Screen { get; } = screen;

        public ScreenSection Team { get; } = team;
        public ScreenSection Wing { get; } = wing;

        public MapOptions_ToggleButton RowTemplate { get; } = rowTemplate;
    }

    private const string ShortName = "WNG";

    private const string ScreenTitle = "WING";

    private const string TeamTitle = "TEAM";
    private const string WingTitle = "WING";

    private const string MarksLabel = "MARKS";
    private const string TargetsLabel = "TARGETS";

    private static readonly AccessTools.FieldRef<VirtualMFD, List<Button>> LeftButtonsRef =
        AccessTools.FieldRefAccess<VirtualMFD, List<Button>>("leftButtons");

    private static readonly AccessTools.FieldRef<VirtualMFD, List<MFDScreen?>> LeftScreensRef =
        AccessTools.FieldRefAccess<VirtualMFD, List<MFDScreen?>>("leftScreens");

    private readonly Wing _wing;

    private readonly Layout _layout;
    private readonly int _screenIndex;

    private readonly List<ScreenRow> _toggleRows = [];
    private readonly List<ScreenRow> _rosterRows = [];

    private Transform _rosterGap = null!;

    private bool _rosterUpdateRequested;
    private bool _toggleUpdateRequested;

    private WingScreen(Wing wing, Layout layout, int screenIndex)
    {
        _wing = wing;
        _layout = layout;
        _screenIndex = screenIndex;
    }

    public static WingScreen? Create(Settings settings, Wing wing, VirtualMFD mfd)
    {
        var layout = BuildLayout(mfd);

        if (!TryClaimScreenSlot(layout, out var slotIndex))
        {
            Object.Destroy(layout.Screen.gameObject);

            return null;
        }

        var wingScreen = new WingScreen(wing, layout, slotIndex);

        var teamOptions = layout.Team.AddRow();
        wingScreen.AddToggle(teamOptions, MarksLabel, settings.ShowTeammates);
        wingScreen.AddToggle(teamOptions, TargetsLabel, settings.ShowTeammatesTargetSelection);

        var wingOptions = layout.Wing.AddRow();
        wingScreen.AddToggle(wingOptions, MarksLabel, settings.ShowWing);
        wingScreen.AddToggle(wingOptions, TargetsLabel, settings.ShowWingTargetSelection);

        wingScreen._rosterGap = layout.Wing.AddGap();
        wingScreen.RebuildRoster();

        layout.Screen.gameObject.SetActive(true);
        mfd.HideAllLeftScreens();

        return wingScreen;
    }

    public void Dispose()
    {
        DisposeRows();
        ReleaseScreenSlot();
        Object.Destroy(_layout.Screen.gameObject);
    }

    public void Tick()
    {
        if (_toggleUpdateRequested)
        {
            _toggleUpdateRequested = false;

            foreach (var row in _toggleRows)
            {
                row.UpdateState();
            }
        }

        if (!_rosterUpdateRequested)
        {
            return;
        }

        _rosterUpdateRequested = false;
        RebuildRoster();
    }

    public void OnWingChanged()
    {
        _rosterUpdateRequested = true;
    }

    public void OnSettingsChanged()
    {
        _toggleUpdateRequested = true;
    }

    private void AddToggle(Transform container, string label, ConfigEntry<bool> entry)
    {
        _toggleRows.Add(
            ScreenRow.CreateToggle(
                container,
                _layout.RowTemplate,
                label,
                () => entry.Value,
                () => entry.Value = !entry.Value
            )
        );
    }

    private void RebuildRoster()
    {
        foreach (var row in _rosterRows)
        {
            row.Dispose();
        }

        _rosterRows.Clear();

        foreach (var id in _wing.Ids)
        {
            _rosterRows.Add(
                ScreenRow.CreateRosterRow(
                    _layout.Wing.Container,
                    _layout.RowTemplate,
                    Identity.GetDisplayName(id),
                    () => _wing.Remove(id)
                )
            );
        }

        _rosterGap.gameObject.SetActive(_rosterRows.Count > 0);

        _layout.Wing.Fit();
    }

    private void DisposeRows()
    {
        foreach (var row in _toggleRows)
        {
            row.Dispose();
        }

        _toggleRows.Clear();

        foreach (var row in _rosterRows)
        {
            row.Dispose();
        }

        _rosterRows.Clear();
    }

    private void ReleaseScreenSlot()
    {
        if (_screenIndex < 0)
        {
            return;
        }

        var screens = LeftScreensRef(_layout.Mfd);
        if (_screenIndex < screens.Count && ReferenceEquals(screens[_screenIndex], _layout.Screen))
        {
            screens[_screenIndex] = null;
        }

        var buttons = LeftButtonsRef(_layout.Mfd);
        if (_screenIndex < buttons.Count)
        {
            buttons[_screenIndex].onClick = new Button.ButtonClickedEvent();
        }

        _layout.Mfd.SetupButtons();
    }

    private static Layout BuildLayout(VirtualMFD mfd)
    {
        var mapOptions = SceneSingleton<MapOptions>.i ??
            throw new InvalidOperationException($"{nameof(MapOptions)} is null.");

        var mapScreen = mapOptions.screen;

        var holder = new GameObject("NoWingmen.WingScreen.Holder");
        holder.transform.SetParent(mapScreen.transform.parent, false);
        holder.SetActive(false);

        var screen = Object.Instantiate(mapScreen, holder.transform);
        screen.name = "NoWingmen.WingScreen";

        Object.DestroyImmediate(screen.GetComponentInChildren<MapOptions>(true));

        screen.transform.SetParent(mapScreen.transform.parent, false);
        holder.transform.SetParent(screen.transform, false);

        screen.aircraftOnly = false;

        var panel = screen.displayPanel.transform;

        var rowTemplate = panel.GetComponentInChildren<MapOptions_ToggleButton>(true);
        var rowContainer = (RectTransform)rowTemplate.transform.parent;
        var sectionRoot = rowContainer.parent;

        var panelHeading = ScreenSection.FindHeading(panel);
        var sectionHeading = ScreenSection.FindHeading(sectionRoot);

        TakeRowTemplate(rowTemplate, holder.transform);
        StripPanel(panel, rowContainer, panelHeading.transform, sectionHeading.transform);

        panelHeading.text = ScreenTitle;
        ScreenSection.StackFromTop(sectionRoot.parent);

        var team = ScreenSection.Claim(rowContainer, sectionHeading, TeamTitle);
        var wing = team.Clone(WingTitle);

        return new Layout(mfd, screen, team, wing, rowTemplate);
    }

    private static void TakeRowTemplate(MapOptions_ToggleButton template, Transform holder)
    {
        var rect = (RectTransform)template.transform;

        template.transform.SetParent(holder, false);
        template.gameObject.SetActive(false);

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, ScreenRow.Height);
    }

    private static void StripPanel(Transform panel, params Transform[] keepLeaves)
    {
        var keep = new HashSet<Transform> { panel };

        foreach (var leaf in keepLeaves)
        {
            for (var t = leaf; t != null && t != panel; t = t.parent)
            {
                keep.Add(t);
            }
        }

        DestroyChildrenExcept(panel, keep);
    }

    private static void DestroyChildrenExcept(Transform parent, HashSet<Transform> keep)
    {
        var children = parent.Cast<Transform>().ToList();

        foreach (var child in children)
        {
            if (keep.Contains(child))
            {
                DestroyChildrenExcept(child, keep);
                continue;
            }

            Object.DestroyImmediate(child.gameObject);
        }
    }

    private static bool TryClaimScreenSlot(Layout layout, out int index)
    {
        var buttons = LeftButtonsRef(layout.Mfd);
        var screens = LeftScreensRef(layout.Mfd);

        index = FindFreeSlotIndex(buttons, screens);
        if (index < 0)
        {
            Plugin.Logger.LogError("No free MFD slot for the wing screen");

            return false;
        }

        var button = buttons[index];

        if (!TryBindSlotVisuals(layout.Screen, button))
        {
            Plugin.Logger.LogError($"MFD button {index} has no label or highlight to bind");
            index = -1;

            return false;
        }

        screens[index] = layout.Screen;

        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => layout.Mfd.PressLeftButton(button));
        button.enabled = true;

        layout.Screen.Setup(layout.Mfd, ShortName);
        layout.Mfd.SetupButtons();

        return true;
    }

    private static bool TryBindSlotVisuals(MFDScreen screen, Button button)
    {
        var label = FindByName<TextMeshProUGUI>(button.transform, screen.label);
        var highlight = FindByName<Image>(button.transform, screen.highlight);

        if (label == null || highlight == null)
        {
            return false;
        }

        screen.label = label;
        screen.highlight = highlight;

        return true;
    }

    private static T? FindByName<T>(Transform parent, Component? source) where T : Component
    {
        return source == null ? null : parent.Find(source.name)?.GetComponent<T>();
    }

    private static int FindFreeSlotIndex(List<Button> buttons, List<MFDScreen?> screens)
    {
        for (var i = 0; i < buttons.Count; i++)
        {
            if (i >= screens.Count || screens[i] == null)
            {
                return i;
            }
        }

        return -1;
    }
}