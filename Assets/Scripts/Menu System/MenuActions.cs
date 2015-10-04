using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains common logic required by all game menus. To use this script simply add as
/// a component to the EventSystem in your menu's scene. Call public methods by referencing
/// EventSystem.MenuActions from the menu item's click handler.
/// </summary>
public class MenuActions : MonoBehaviour
{
    /// <summary>
    /// Default menu transition simply moves and scales the given menu into camera view
    /// instantanously. Add an IMenuTransition component to a canvas to animate it.
    /// </summary>
    private static DefaultMenuTransistion defaultTransistion = new DefaultMenuTransistion();

    /// <summary>
    /// The Editor assignable canvas.
    /// </summary>
    public Canvas initialCanvas;

    /// <summary>
    /// The stack of canvas's that we have navigated to. Pop to go back.
    /// </summary>
    private Stack<Canvas> navigationStack = new Stack<Canvas>();

    /// <summary>
    /// The current menu canvas.
    /// </summary>
    private Canvas currentCanvas;

    /// <summary>
    /// Navigates to a new canvas and pushes the previous one to the navigation stack.
    /// </summary>
    /// <param name="canvas">The canvas to navigate to.</param>
    public void NavigateAndPushCanvas(Canvas canvas)
    {
        if (canvas == null)
        {
            Debug.LogWarning("Pushed null canvas to navigation stack.");
            return;
        }
        
        var transition = this.currentCanvas.GetComponent<IMenuTransistion>() ?? defaultTransistion;
        transition.Transistion(this.currentCanvas, canvas);

        this.navigationStack.Push(this.currentCanvas);
        this.currentCanvas = canvas;
    }

    /// <summary>
    /// Pops the last menu we were at and navigates to it.
    /// </summary>
    public void NavigateAndPopCanvas()
    {
        if (this.navigationStack.Count == 0)
        {
            Debug.LogWarning("Popped empty navigation stack.");
            return;
        }

        var previousCanvas = this.navigationStack.Pop();
        var transition = this.currentCanvas.GetComponent<IMenuTransistion>() ?? defaultTransistion;

        transition.Transistion(this.currentCanvas, previousCanvas);
        this.currentCanvas = previousCanvas;
    }

    /// <summary>
    /// Clears all current items from the navigation stack.
    /// NOTE: be careful, this can break the state of the menu system if not immediately
    /// followed by a Navigate and push because we are clearing ALL menus and leaving them
    /// in between states.
    /// </summary>
    /// <param name="canvas">The canvas to reset the stack to.</param>
    public void Reset()
    {
        this.navigationStack.Clear();
    }

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

    /// <summary>
    /// Performs menu system setup.
    /// </summary>
    void Start()
    {
        if (initialCanvas == null)
        {
            Debug.LogError("Must set MenuActions.initialCanvas for menu system to function.");
        }

        this.currentCanvas = this.initialCanvas;
    }
}
