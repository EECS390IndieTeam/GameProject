using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains menu actions for game lobby.
/// </summary>
public class LobbyGameMenuActions : Bolt.GlobalEventListener
{
    /// <summary>
    /// The content panel of the connected players ScrollView.
    /// </summary>
    public GameObject scrollPanel;

    /// <summary>
    /// Prefab used to populate the list.
    /// </summary>
    public GameObject listItemPrefab;

    /// <summary>
    /// The height of the list item prefab. There is probably a way to get it in code
    /// but Unity sucks and I can't find it. AGH!
    /// </summary>
    public int listItemHeight = 30;

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

    /// <summary>
    /// Runs once at scene load.
    /// </summary>
    void Start()
    {
        if (this.scrollPanel == null)
        {
            Debug.LogError("LobbyGameMenuActions expects a non-null scrollPanel.");
            return;
        }

        if (this.listItemPrefab == null)
        {
            Debug.LogError("LobbyGameMenuActions expects a non-null listItemPrefab.");
            return;
        }

        if (this.listItemHeight < 1)
        {
            Debug.LogError("LobbyGameMenuActions expects a positive listItemHeight.");
            return;
        }

        Lobby.LobbyUpdatedEvent += (change) =>
        {
            switch (change)
            {
                case Lobby.LobbyChange.PLAYER_ADDED:
                case Lobby.LobbyChange.PLAYER_CHANGED:
                case Lobby.LobbyChange.PLAYER_REMOVED:
                    ReloadPlayersList();
                    break;
            }
        };
    }

    /// <summary>
    /// Repopulates the list of connected players.
    /// </summary>
    private void ReloadPlayersList()
    {
        // Clear the list.I gott
        foreach (var childTransform in this.scrollPanel.transform)
        {
            Destroy(((Transform)childTransform).gameObject);
        }

        // Populate list with connected players,
        // group players by teams sorted alphabetically by name.
        int nextY = -(this.listItemHeight / 2);
        foreach (var player in Lobby.AllPlayers
            .Where(x => x.Connected)
            .OrderBy(y => y.Team)
            .ThenBy(z => z.Name))
        {
            var newItem = Instantiate(this.listItemPrefab);
            var textComponent = newItem.GetComponentInChildren<Text>();
            var imageComponent = newItem.GetComponentInChildren<Image>();

            // Team colors.
            var teamColor = ColorFromTeam(player.Team);
            teamColor.a = 0.5f;
            imageComponent.color = teamColor;
			
			// Add new item to the list box.
			newItem.transform.SetParent(this.scrollPanel.transform);

            textComponent.text = player.Name;

            // For some reason Unity auto rotates the item. Set it to be straight up.
			newItem.transform.localRotation = Quaternion.identity;

            // Set item's position in the box.
            newItem.transform.localPosition = new Vector3(91.5f, nextY, 0);
            nextY -= this.listItemHeight;
        }
    }

    /// <summary>
    /// Maps team number to a color.
    /// </summary>
    /// <param name="team">The number of the team.</param>
    /// <returns>A color for the team.</returns>
    private static Color ColorFromTeam(int team)
    {
        switch (team)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.blue;
            case 2:
                return Color.green;
            case 3:
                return Color.yellow;
            case 4:
                return Color.cyan;
            case 5:
                return Color.magenta;
            case 6:
                return Color.white;
            default:
                Debug.LogWarning(string.Format(
                    "LobbyGameMenuActions does not have a definition for colors for team {0}",
                    team));
                return Color.grey;
        }
    }
}
