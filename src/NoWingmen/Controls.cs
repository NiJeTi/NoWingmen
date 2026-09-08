using InputFramework;
using Rewired;

namespace NoWingmen;

internal static class Controls
{
    public const string InputFrameworkGuid = "experimental.assassin1076.extrainputframework";

    private const string ActionCategory = "Gameplay";

    public const string ActionToggleWing = "NoWingmen: Toggle player in wing";
    public const string ActionToggleLockPrevention = "NoWingmen: Toggle lock prevention";

    public static void Register()
    {
        ExtraInputManager.LoadPendingActions();
        ExtraInputManager.RegisterAction(ActionToggleWing, InputActionType.Button, ActionCategory);
        ExtraInputManager.RegisterAction(ActionToggleLockPrevention, InputActionType.Button, ActionCategory);
    }

    public static bool Pressed(string action)
    {
        return GameManager.playerInput.GetButtonDown(action);
    }
}