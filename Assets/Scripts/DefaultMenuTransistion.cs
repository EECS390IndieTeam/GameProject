using UnityEngine;

/// <summary>
/// Default menu transistion. Add a transistion to a menu by adding a new IMenuTransition
/// script to each canvas. If you don't this one is automagically loaded.
/// </summary>
public class DefaultMenuTransistion : IMenuTransistion
{
    /// <summary>
    /// Loads from one menu to the next by disabling the current menu and
    /// enabling and moving the new one.
    /// </summary>
    /// <param name="from">Point A.</param>
    /// <param name="to">Point B.</param>
    public void Transistion(Canvas from, Canvas to)
    {
        // Hide first menu.
        from.enabled = false;

        var fromTransform = from.GetComponent<RectTransform>();
        var toTranform = to.GetComponent<RectTransform>();
        if (toTranform == null || fromTransform == null)
        {
            Debug.LogWarning("Default Menu Transistion menu has no RectTransform component.");
            return;
        }

        // Move and enable second.
        toTranform.position = fromTransform.position;
        toTranform.rotation = fromTransform.rotation;
        to.enabled = true;
    }
}

