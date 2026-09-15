using InputFramework;
using Rewired;

namespace NoWingmen;

internal sealed class Controls
{
    public const string GuidInputFramework = "experimental.assassin1076.extrainputframework";

    private const string ActionCategory = "Gameplay";
    private const string ActionToggleLockPrevention = "NoWingmen: Toggle lock prevention";

    private Controls()
    {
    }

    public static Controls Init()
    {
        ExtraInputManager.LoadPendingActions();
        ExtraInputManager.RegisterAction(ActionToggleLockPrevention, InputActionType.Button, ActionCategory);

        return new Controls();
    }

    public bool IsToggleLockPreventionDown()
    {
        var player = GameManager.playerInput;

        return player != null && player.GetButtonDown(ActionToggleLockPrevention);
    }
}