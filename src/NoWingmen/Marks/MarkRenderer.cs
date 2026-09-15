using HarmonyLib;

namespace NoWingmen.Marks;

internal static class MarkRenderer
{
    private static readonly AccessTools.FieldRef<CombatHUD, List<HUDUnitMarker>> MarkersRef =
        AccessTools.FieldRefAccess<CombatHUD, List<HUDUnitMarker>>("markers");

    private static readonly Action<HUDUnitMarker> UpdateColorRef = AccessTools.MethodDelegate<Action<HUDUnitMarker>>(
        AccessTools.Method(typeof(HUDUnitMarker), "UpdateColor")
    );

    public static void Update()
    {
        UpdateMap();
        UpdateHud();
    }

    private static void UpdateMap()
    {
        var map = SceneSingleton<DynamicMap>.i ??
            throw new InvalidOperationException($"{nameof(DynamicMap)} is null.");

        var exceptionCount = 0;
        Exception? lastException = null;
        foreach (var icon in map.mapIcons)
        {
            try
            {
                icon.UpdateColor();
            }
            catch (Exception e)
            {
                exceptionCount++;
                lastException = e;
            }
        }

        if (exceptionCount > 0)
        {
            Plugin.Logger.LogError($"Failed to update {exceptionCount} map icons: {lastException}");
        }
    }

    private static void UpdateHud()
    {
        var hud = SceneSingleton<CombatHUD>.i ??
            throw new InvalidOperationException($"{nameof(CombatHUD)} is null.");

        var exceptionCount = 0;
        Exception? lastException = null;
        foreach (var marker in MarkersRef(hud))
        {
            try
            {
                UpdateColorRef(marker);
            }
            catch (Exception e)
            {
                exceptionCount++;
                lastException = e;
            }
        }

        if (exceptionCount > 0)
        {
            Plugin.Logger.LogError($"Failed to update {exceptionCount} HUD markers: {lastException}");
        }
    }
}