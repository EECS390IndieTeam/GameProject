using UnityEngine;

/// <summary>
/// Contains common logic required by all game menus. To use this script simply add as
/// a component to the EventSystem in your menu's scene. Call public methods by referencing
/// EventSystem.MenuActions from the menu item's click handler.
/// </summary>
public class MenuActions : MonoBehaviour
{
    /// <summary>
    /// Terminates Unity and returns control to the operating system.
    /// </summary>
    public void ExitApplication()
    {
        // Application.Quit() function does not function in editor.
        if (Application.isEditor)
        {
            Debug.LogWarning("Cannot exit game while in editor. Deploy to test this feature.");
        }

        Application.Quit();
    }
}
