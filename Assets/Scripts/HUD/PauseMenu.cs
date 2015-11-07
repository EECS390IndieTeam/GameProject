using UnityEngine;

/// <summary>
/// Pause menu script.
/// </summary>
public class PauseMenu : MonoBehaviour {

    /// <summary>
    /// The canvas of the pause menu.
    /// </summary>
    public Canvas pauseCanvas;

    /// <summary>
    /// Is the game paused.
    /// </summary>
    private bool paused = false;

    /// <summary>
    /// Fatal error encountered.
    /// </summary>
    private bool dead = false;

    /// <summary>
    /// Gets or sets the pause menu state and cursor visibility for the game.
    /// </summary>
    public bool IsPaused
    {
        set
        {
            if (!this.dead)
            {
                pauseCanvas.enabled = value;
                Cursor.visible = value;
                Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
                this.paused = value;
            }
        }

        get
        {
            return this.paused;
        }
    }

    /// <summary>
    /// Quits 
    /// </summary>
	public void Quit ()
    {
        // Feel free to change this if it breaks like I think it will.
        GameManager.instance.QuitToMainMenu();
    }


    /// <summary>
    /// Run once per frame.
    /// </summary>
    void Update()
    {
        if (!this.dead && Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle paused.
            this.IsPaused = !this.IsPaused;
        }
    }

    /// <summary>
    /// Check inputs before running.
    /// </summary>
    void Start()
    {
        if (this.pauseCanvas == null)
        {
            Debug.LogError("PauseMenu requires a PauseCanvas reference and it must be disabled initially.");
            this.dead = true;
        }
    }
}
