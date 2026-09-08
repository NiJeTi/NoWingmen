using HarmonyLib;
using NuclearOption.UIStyleSystem;
using UnityEngine;

namespace NoWingmen;

internal static class Feedback
{
    private const float ReportSeconds = 5f;

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> SelectSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("selectSound");

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> DeselectSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("deselectSound");

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> DeselectAllSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("deselectAllSound");

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> WeaponSwitchSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("weaponSwitchSound");

    public static void OnWingAdd(Unit unit)
    {
        Play(SelectSoundRef);
        Report($"{Name(unit)} added to wing");
    }

    public static void OnWingRemove(Unit unit)
    {
        Play(DeselectSoundRef);
        Report($"{Name(unit)} removed from wing");
    }

    public static void OnWingReject(string reason)
    {
        Play(DeselectAllSoundRef);
        Report(reason.AddColor(ThemeManager.Active.ColorTheme.Alert));
    }

    public static void OnLockPreventionToggle(bool enabled)
    {
        Play(WeaponSwitchSoundRef);
        Report($"Lock prevention <b>{(enabled ? "ENABLED" : "DISABLED")}</b>");
    }

    private static string Name(Unit unit)
    {
        return $"<b>{Identity.GetDisplayName(unit).AddColor(Config.WingColor.Value)}</b>";
    }

    private static void Report(string message)
    {
        var report = SceneSingleton<AircraftActionsReport>.i;
        if (report != null)
        {
            report.ReportText(message, ReportSeconds);
        }
    }

    private static void Play(AccessTools.FieldRef<CombatHUD, AudioClip> clipRef)
    {
        var hud = SceneSingleton<CombatHUD>.i;
        if (hud == null)
        {
            return;
        }

        var clip = clipRef(hud);
        if (clip != null)
        {
            SoundManager.PlayInterfaceOneShot(clip);
        }
    }
}
