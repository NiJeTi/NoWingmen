using Rewired;
using Rewired.Data;

namespace NoWingmen;

internal static class Controls
{
    private const string ActionCategory = "Gameplay";
    
    private const string ActionToggleLockPrevention = "NoWingmen: Toggle lock prevention";
    private const int ActionToggleLockPreventionId = 9121999_01;

    public static void Inject(UserData userData)
    {
        if (userData.ContainsAction(ActionToggleLockPreventionId))
        {
            return;
        }

        var category = userData.actionCategories.FirstOrDefault(c => c.name == ActionCategory);
        if (category == null)
        {
            return;
        }

        userData.actions.Add(new InputAction
        {
            id = ActionToggleLockPreventionId,
            name = ActionToggleLockPrevention,
            descriptiveName = ActionToggleLockPrevention,
            type = InputActionType.Button,
            categoryId = category.id,
            userAssignable = true,
        });
        userData.actionCategoryMap.AddAction(category.id, ActionToggleLockPreventionId);
    }

    public static bool IsToggleLockPreventionDown()
    {
        var player = GameManager.playerInput;

        return player != null && player.GetButtonDown(ActionToggleLockPreventionId);
    }
}
