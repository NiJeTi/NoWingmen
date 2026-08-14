using HarmonyLib;
using UnityEngine;

namespace NoWingmen;

internal static class Feedback
{
    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> SelectSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("selectSound");

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> DeselectSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("deselectSound");

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> DeselectAllSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("deselectAllSound");

    private static readonly AccessTools.FieldRef<CombatHUD, AudioClip> WeaponSwitchSoundRef =
        AccessTools.FieldRefAccess<CombatHUD, AudioClip>("weaponSwitchSound");

    public static void OnWingAdd()
    {
        Play(SelectSoundRef);
    }

    public static void OnWingRemove()
    {
        Play(DeselectSoundRef);
    }

    public static void OnWingReject()
    {
        Play(DeselectAllSoundRef);
    }

    public static void OnLockPreventionToggle()
    {
        Play(WeaponSwitchSoundRef);
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