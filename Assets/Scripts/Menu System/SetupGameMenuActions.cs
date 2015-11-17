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
    /// Default host port number.
    /// </summary>
    public int defaultPort = 54321;

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

        // Set default port number in input box.
        this.portInputField.text = this.defaultPort.ToString();

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
            GameManager.instance.GameMode = new TeamDeathmatchMode();
            ServerSideData.Password = this.lobbyPasswordInputField.text;

            // IMPORTANT: If you are planning on hiding the mouse cursor here, DON'T.
            // There is another UI page after this one.

            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                BoltLauncher.Shutdown();
            }

            // We validate this on edit, we shouldn't need to again.
            BoltLauncher.StartServer(int.Parse(this.portInputField.text));
            GameObject.Find("LobbyPanel").GetComponent<LobbyGameMenuActions>().PrepareMenu();
        }));
    }

    /// <summary>
    /// Validates user inputs. If inputs are bad, disables the launch button.
    /// </summary>
    private void ValidateInputs()
    {
        int port;

        this.launchButton.interactable
            = (int.TryParse(this.portInputField.text, out port) &&
            this.screenNameInputField.text.Length != 0);
    }
}
