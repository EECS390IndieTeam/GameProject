using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains menu actions for game lobby.
/// </summary>
public class LobbyGameMenuActions : Bolt.GlobalEventListener
{
    public GameObject scrollPanel;

    public GameObject listItemPrefab;

    public Button launchButton;

    public bool isClient = true;

    /// <summary>
    /// Stops the multiplayer client or server.
    /// </summary>
    public void StopBolt()
    {
        BoltLauncher.Shutdown();
    }

    /// <summary>
    /// Loads the scene and begins the multiplayer game.
    /// </summary>
    public void StartGame()
    {
        Cursor.visible = false;
        BoltNetwork.LoadScene("ingame");
    }

    /// <summary>
    /// State updates.
    /// </summary>
    void Update()
    {
        // TODO: fix this spelling mistake in the code whereever it originated.
        DebugHUD.setValue("IsServer", BoltNetwork.isServer);

        if (BoltNetwork.isClient)
        {
            //DebugHUD.setValue("ping", BoltNetwork.server.PingNetwork);
        }

        DebugHUD.setValue("username", GameManager.instance.CurrentUserName);
    }

    /// <summary>
    /// Disconnected from Bolt handler.
    /// </summary>
    /// <param name="connection">The connection to remote bolt client/server(s).</param>
    public override void Disconnected(BoltConnection connection)
    {
        Bolt.IProtocolToken token = connection.DisconnectToken;

        if (token != null && token is DisconnectReason)
        {
            DisconnectReason reason = (DisconnectReason)token;
            Debug.Log("Disconnected from server: " + reason.Reason + (reason.Message == "" ? "" : ": " + reason.Message));
        }
    }

    void Start()
    {
        Lobby.LobbyUpdatedEvent += (change) =>
        {
            switch (change)
            {
                case Lobby.LobbyChange.PLAYER_ADDED:
              //  case Lobby.LobbyChange.PLAYER_CHANGED:
               // case Lobby.LobbyChange.PLAYER_REMOVED:
                    ReloadPlayersList();
                    break;
            }
        };
    }

    private void ReloadPlayersList()
    {
        // Clear the list.
        foreach (var childTransform in this.scrollPanel.transform)
        {
            Destroy(((Transform)childTransform).gameObject);
        }

        // Populate list with items.
        int nextY = -15;
        foreach (var player in Lobby.AllPlayers)
        {
            var newItem = Instantiate(this.listItemPrefab);
            var textComponent = newItem.GetComponentInChildren<Text>();

            textComponent.text = player.Name;

            // For some reason Unity auto rotates the item. Set it to be straight up.
            newItem.transform.eulerAngles = Vector3.zero;

            // Set item's position in the box.
            newItem.transform.position = new Vector3(0, nextY, 0);
            nextY -= 30;

            // Add new item to the list box.
            newItem.transform.SetParent(this.scrollPanel.transform);
        }
    }
}
