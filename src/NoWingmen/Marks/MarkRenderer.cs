using HarmonyLib;

namespace NoWingmen.Marks;

internal static class MarkRenderer
{
    private static readonly AccessTools.FieldRef<CombatHUD, List<HUDUnitMarker>> MarkersRef =
        AccessTools.FieldRefAccess<CombatHUD, List<HUDUnitMarker>>("markers");

    private static readonly Action<HUDUnitMarker> UpdateColorRef =
        AccessTools.MethodDelegate<Action<HUDUnitMarker>>(
            AccessTools.Method(typeof(HUDUnitMarker), "UpdateColor")
        );

    public static void Update()
    {
        UpdateMap();
        UpdateHud();
    }

    private static void UpdateMap()
    {
        var map = SceneSingleton<DynamicMap>.i;
        if (map == null || map.mapIcons == null)
        {
            return;
        }

        var exceptionCount = 0;
        Exception lastException = null;
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
            Plugin.Logger.LogError($"Failed to refresh {exceptionCount} map icons: {lastException}");
        }
    }

    private static void UpdateHud()
    {
        var hud = SceneSingleton<CombatHUD>.i;
        if (hud == null)
        {
            return;
        }

        var markers = MarkersRef(hud);
        if (markers == null)
        {
            return;
        }

        var exceptionCount = 0;
        Exception lastException = null;
        foreach (var marker in markers)
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
            Plugin.Logger.LogError($"Failed to refresh {exceptionCount} HUD markers: {lastException}");
        }
    }
}