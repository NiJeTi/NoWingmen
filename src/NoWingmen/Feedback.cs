using HarmonyLib;
using UnityEngine;

namespace NoWingmen;

internal static class Feedback
{
    private const float ReportSeconds = 5f;

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> SelectSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("selectSound");

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> DeselectSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("deselectSound");

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> WeaponSwitchSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("weaponSwitchSound");

    public static void OnWingPlayerToggled(bool state)
    {
        Play(state ? SelectSoundRef : DeselectSoundRef);
    }

    public static void OnLockPreventionToggle(bool enabled)
    {
        Play(WeaponSwitchSoundRef);
        Report($"Lock prevention <b>{(enabled ? "ENABLED" : "DISABLED")}</b>");
    }

    private static void Report(string message)
    {
        var report = SceneSingleton<AircraftActionsReport>.i ??
            throw new InvalidOperationException($"{nameof(AircraftActionsReport)} is null.");

        report.ReportText(message, ReportSeconds);
    }

    private static void Play(AccessTools.FieldRef<CombatHUD, AudioClip> clipRef)
    {
        var hud = SceneSingleton<CombatHUD>.i ??
            throw new InvalidOperationException($"{nameof(CombatHUD)} is null.");

        SoundManager.PlayInterfaceOneShot(clipRef(hud));
    }
}