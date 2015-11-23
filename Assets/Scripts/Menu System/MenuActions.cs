using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    /// Gets the current instance for the scene.
    /// </summary>
    public static MenuActions Instance { get; private set; }

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
    /// Terminates Unity and returns control to the operating system.
    /// </summary>
    public void ExitApplication()
    {
#if UNITY_EDITOR
        // Application.Quit() function does not function in editor.
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        } else {
            Debug.LogWarning("Error; somehow code was compiled for the editor, but we are not in the editor.  This is impossible and/or unimportant");
        }
#else
        Application.Quit();

#endif
    }

    /// <summary>
    /// Performs menu system setup.
    /// </summary>
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple MenuActions present in the scene.");
            return;
        }

        if (initialCanvas == null)
        {
            Debug.LogError("Must set MenuActions.initialCanvas for menu system to function.");
        }

        this.currentCanvas = this.initialCanvas;
    }
}
