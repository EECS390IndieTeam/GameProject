using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Setup (Host) game menu actions code behind.
/// </summary>
public class SetupGameMenuActions : MonoBehaviour
{
    /// <summary>
    /// Port input field control.
    /// </summary>
    public InputField portInputField;

    /// <summary>
    /// Lobby password input field control.
    /// </summary>
    public InputField lobbyPasswordInputField;

    /// <summary>
    /// Screen name input field control.
    /// </summary>
    public InputField screenNameInputField;

    /// <summary>
    /// Launch button input control.
    /// </summary>
    public Button launchButton;

    /// <summary>
    /// Initial setup.
    /// </summary>
    void Start()
    {
        if (this.portInputField == null)
        {
            Debug.LogError("SetupGameMenuActions.portInputField cannot be null.");
        }

        if (this.lobbyPasswordInputField == null)
        {
            Debug.LogError("SetupGameMenuActions.lobbyPasswordInputField cannot be null.");
        }

        if (this.screenNameInputField == null)
        {
            Debug.LogError("SetupGameMenuActions.screenNameInputField cannot be null.");
        }

        if (this.launchButton == null)
        {
            Debug.LogError("SetupGameMenuActions.launchButton cannot be null.");
        }

        // Setup input validation. Deactivate launch button if the input values are bad.
        var validateAction = new UnityEngine.Events.UnityAction<string>((newValue) =>
        {
            this.ValidateInputs();
        });
        this.portInputField.onValueChange.AddListener(validateAction);
        this.lobbyPasswordInputField.onValueChange.AddListener(validateAction);
        this.screenNameInputField.onValueChange.AddListener(validateAction);

        // Add click listener for the launch button.
        // Animations to waiting scene are done in the editor so we don't have to do lookups.
        // We could do this in the editor but we scripted everything else.
        // Might as well do it here too.
        this.launchButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
        {
            GameManager.instance.CurrentUserName = this.screenNameInputField.text;
            GameManager.instance.gameMode = new TeamDeathmatchMode();
            ServerConnectionEventListener.ServerPassword = this.lobbyPasswordInputField.text;
            Cursor.visible = false;
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                BoltLauncher.Shutdown();
            }

            // We validate this on edit, we shouldn't need to again.
            BoltLauncher.StartServer(int.Parse(this.portInputField.text));
        }));
    }

    /// <summary>
    /// Validates user inputs. If inputs are bad, disables the launch button.
    /// </summary>
    private void ValidateInputs()
    {
        int port;

        if (int.TryParse(this.portInputField.text, out port) &&
            this.lobbyPasswordInputField.text.Length >= 6 &&
            this.screenNameInputField.text.Length != 0)
        {
            this.launchButton.interactable = true;
        }
        else
        {
            this.launchButton.interactable = false;
        }
    }
}
