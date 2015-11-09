using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Join game menu actions code behind.
/// </summary>
public class JoinGameMenuActions : MonoBehaviour
{
    /// <summary>
    /// IP and port input field control.
    /// </summary>
    public InputField ipInputField;

    /// <summary>
    /// Lobby password input field control.
    /// </summary>
    public InputField lobbyPasswordInputField;

    /// <summary>
    /// Screen name input field control.
    /// </summary>
    public InputField screenNameInputField;

    /// <summary>
    /// Lobby button input control.
    /// </summary>
    public Button lobbyButton;

    /// <summary>
    /// Set this to true to begin connecting to the server
    /// </summary>
    private bool connect = false;

    /// <summary>
    /// Initial setup.
    /// </summary>
    void Start()
    {
        if (this.ipInputField == null)
        {
            Debug.LogError("SetupGameMenuActions.ipInputField cannot be null.");
        }

        if (this.lobbyPasswordInputField == null)
        {
            Debug.LogError("SetupGameMenuActions.lobbyPasswordInputField cannot be null.");
        }

        if (this.screenNameInputField == null)
        {
            Debug.LogError("SetupGameMenuActions.screenNameInputField cannot be null.");
        }

        if (this.lobbyButton == null)
        {
            Debug.LogError("SetupGameMenuActions.launchButton cannot be null.");
        }

        // Setup input validation. Deactivate launch button if the input values are bad.
        var validateAction = new UnityEngine.Events.UnityAction<string>((newValue) =>
        {
            this.ValidateInputs();
        });
        this.ipInputField.onValueChange.AddListener(validateAction);
        this.lobbyPasswordInputField.onValueChange.AddListener(validateAction);
        this.screenNameInputField.onValueChange.AddListener(validateAction);

        // Add click listener for the launch button.
        // Animations to waiting scene are done in the editor so we don't have to do lookups.
        // We could do this in the editor but we scripted everything else.
        // Might as well do it here too.
        this.lobbyButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
        {
            GameManager.instance.CurrentUserName = this.screenNameInputField.text;

            // We validate this on edit, we shouldn't need to again.
            var addressComponents = this.ipInputField.text.Split(':');
            var port = int.Parse(addressComponents[1]);

            if (BoltNetwork.isRunning && BoltNetwork.isServer)
            {
                BoltLauncher.Shutdown();
            }

            connect = true;
        }));
    }

    void Update() {
        if (connect) {
            if (!BoltNetwork.isRunning) {
                BoltLauncher.StartClient();
            } else {
                BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(this.ipInputField.text),
                    new ConnectionRequestData(GameManager.instance.CurrentUserName, this.lobbyPasswordInputField.text));
                connect = false;
            }
        }
    }

    /// <summary>
    /// Validates user inputs. If inputs are bad, disables the lobby button.
    /// </summary>
    private void ValidateInputs()
    {
        if (this.IsIPAndPortValid() &&
            this.screenNameInputField.text.Length != 0)
        {
            this.lobbyButton.interactable = true;
        }
        else
        {
            this.lobbyButton.interactable = false;
        }
    }

    /// <summary>
    /// Superficially verifies an IP address by checking for 4 IP integer
    /// segments connected by 3 periods. More intense checking is done
    /// by Bolt. This is "good 'nuf"!
    /// </summary>
    /// <returns>True if the IP address is superficially valid.</returns>
    private bool IsIPAndPortValid()
    {
        string[] addressComponents = this.ipInputField.text.Split(':');

        // Check for both an IP address and a port separated by a colon.
        if (addressComponents.Length != 2)
        {
            return false;
        }

        // Check for 4 segments.
        int seg;
        var ipComponets = addressComponents[0].Split('.');
        if (ipComponets.Length != 4 ||
            !int.TryParse(ipComponets[0], out seg) ||
            !int.TryParse(ipComponets[1], out seg) ||
            !int.TryParse(ipComponets[2], out seg) ||
            !int.TryParse(ipComponets[3], out seg) ||
            !int.TryParse(addressComponents[1], out seg))
        {
            return false;
        }

        return true;
    }
}
