using System.Linq;
using System.Collections.Generic;
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
    /// Button used to launch the game.
    /// </summary>
    public Button launchButton;

    /// <summary>
    /// Next map button.
    /// </summary>
    public Button nextMapButton;

    /// <summary>
    /// Prev. map button.
    /// </summary>
    public Button prevMapButton;

    /// <summary>
    /// Next mode button.
    /// </summary>
    public Button nextModeButton;

    /// <summary>
    /// Prev. mode button.
    /// </summary>
    public Button prevModeButton;

    /// <summary>
    /// The game mode label.
    /// </summary>
    public Text gameModelLabel;

    /// <summary>
    /// The map label.
    /// </summary>
    public Text mapLabel;

    /// <summary>
    /// Current player current team label.
    /// </summary>
    public Text teamLabel;

    /// <summary>
    /// Team UI panel.
    /// </summary>
    public Image teamPanel;

    /// <summary>
    /// The height of the list item prefab. There is probably a way to get it in code
    /// but Unity sucks and I can't find it. AGH!
    /// </summary>
    public int listItemHeight = 30;

    /// <summary>
    /// Current Game mode index in the buttons on the host.
    /// Not used by client since client has no buttons but is purely reactive
    /// to Bolt changes.
    /// </summary>
    private int currentHostGameMode = 0;
    
    /// <summary>
    /// Current map index for the host. Not used on client.
    /// </summary>
    private int currentHostMap = 0;

    /// <summary>
    /// Sets the default settings for the menu under this instance of bolt.
    /// </summary>
    public void PrepareMenu()
    {
        if (!BoltNetwork.isClient)
        {
            this.currentHostGameMode = 0;

            this.SetGameMode();

            //this.gameModelLabel.text = "Game Mode";
            //this.mapLabel.text = "Map";

            this.launchButton.interactable = true;

            if (GameModeManager.GameModes.Length > 1) {
                prevModeButton.interactable = true;
                nextModeButton.interactable = true;
            } else {
                prevModeButton.interactable = false;
                nextModeButton.interactable = false;
            }
        }
        else
        {
            this.launchButton.GetComponent<Text>().enabled = false;

            this.prevModeButton.GetComponent<Text>().enabled = false;
            this.nextMapButton.GetComponent<Text>().enabled = false;
            this.prevMapButton.GetComponent<Text>().enabled = false;
            this.nextModeButton.GetComponent<Text>().enabled = false;
        }

        GameManager.instance.ChangeGameState(GameManager.GameState.LOBBY);
    }

    /// <summary>
    /// Switches to Next GameMode in list if there is one or loops around.
    /// </summary>
    public void NextGameMode()
    {
        if (BoltNetwork.isClient)
        {
            return;
        }

        this.currentHostGameMode++;

        if (this.currentHostGameMode >= GameModeManager.GameModes.Length)
        {
            this.currentHostGameMode = 0;
        }

        this.SetGameMode();
    }

    /// <summary>
    /// Switches to previous GameMode or loops around if none.
    /// </summary>
    public void PrevGameMode()
    {
        if (BoltNetwork.isClient)
        {
            return;
        }

        this.currentHostGameMode--;

        if (this.currentHostGameMode < 0)
        {
            this.currentHostGameMode = GameModeManager.GameModes.Length - 1;
        }

        this.SetGameMode();
    }

    /// <summary>
    /// Switches to Next Map in list if there is one or loops around.
    /// </summary>
    public void NextMap()
    {
        if (BoltNetwork.isClient) {
            return;
        }

        this.currentHostMap++;

        if (this.currentHostMap >= GameModeManager.GetSupportedMapsForGameMode(GameModeManager.GameModeNames[this.currentHostGameMode]).Count)
        {
            this.currentHostMap = 0;
        }

        this.SetMap();
    }

    /// <summary>
    /// Switches to previous Map or loops around if none.
    /// </summary>
    public void PrevMap()
    {
        if (BoltNetwork.isClient)
        {
            return;
        }

        this.currentHostMap--;

        if (this.currentHostMap < 0)
        {
            this.currentHostMap = GameModeManager.GetSupportedMapsForGameMode(GameModeManager.GameModeNames[this.currentHostGameMode]).Count - 1;
        }

        this.SetMap();
    }

    /// <summary>
    /// Switches to the previous team in the teams enumerables.
    /// </summary>
    public void PrevTeam()
    {
        var currentLobbyPlayer = GetCurrentLobbyPlayer();

        // LogErrors printed in GetCurrentLobbyPlayer().
        if (currentLobbyPlayer == null)
        {
            return;
        }

        var newTeam = currentLobbyPlayer.Team - 1;

        // YOLO SWAG THUG-LYFE 5-EVA Fo shizzle ma nizzle.
        if (GameManager.instance.GameMode.UsesTeams) {
            if (newTeam < 1) {
                newTeam = GameManager.instance.GameMode.MaxTeams;
            }
        } else {
            if (newTeam < 0) {
                newTeam = 7;
            }
        }

        Lobby.SetPlayerTeam(currentLobbyPlayer.Name, newTeam);
    }

    /// <summary>
    /// Switches to the previous team in the teams enumerables.
    /// </summary>
    public void NextTeam()
    {
        var currentLobbyPlayer = GetCurrentLobbyPlayer();

        // LogErrors printed in GetCurrentLobbyPlayer().
        if (currentLobbyPlayer == null)
        {
            return;
        }

        int newTeam = currentLobbyPlayer.Team + 1;

        // YOLO SWAG THUG-LYFE 5-EVA Fo shizzle ma nizzle.
        if (GameManager.instance.GameMode.UsesTeams) {
            if (newTeam >= (GameManager.instance.GameMode.MaxTeams + 1 )) {
                Debug.Log("Wrapped around from team " + newTeam + "to team " + (GameManager.instance.GameMode.UsesTeams ? 1 : 0));
                newTeam = 1;
            }
        } else {
            if (newTeam >= 8) {
                newTeam = 0;
            }
        }

        Lobby.SetPlayerTeam(currentLobbyPlayer.Name, newTeam);
    }

    /// <summary>
    /// Updates the Map in Bolt.
    /// </summary>
    private void SetMap()
    {
        if (this.currentHostMap >= 0 && this.currentHostMap < GameModeManager.GetSupportedMapsForGameMode(Lobby.GameModeName).Count)
        {
            Lobby.MapName = GameModeManager.GetSupportedMapsForGameMode(Lobby.GameModeName)[this.currentHostMap];
        }
        else
        {
            Lobby.MapName = null;
        }
    }

    /// <summary>
    /// Updates the GameMode in Bolt.
    /// </summary>
    private void SetGameMode()
    {
        Debug.Log("Setting GameMode to " + this.currentHostGameMode + " (" + GameModeManager.GameModes[this.currentHostGameMode].GetType().Name + ")");
        if (this.currentHostGameMode >= 0 && this.currentHostGameMode < GameModeManager.GameModes.Length)
        {
            GameManager.instance.GameMode = GameModeManager.GameModes[this.currentHostGameMode];

            var maps = GameModeManager.GetSupportedMapsForGameMode(Lobby.GameModeName);
            if (maps.Count > 0) {
                currentHostMap = 0;
                nextMapButton.interactable = maps.Count > 1;
                prevMapButton.interactable = maps.Count > 1;
            } else {
                currentHostMap = -1;
                nextMapButton.interactable = false;
                prevMapButton.interactable = false;
            }
            SetMap();

        }
        else
        {
            Lobby.GameModeName = null;
        }
    }

    /// <summary>
    /// Stops the multiplayer client or server.
    /// </summary>
    public void StopBolt()
    {
        if (BoltNetwork.isRunning)
        {
            var isServer = BoltNetwork.isServer;

            BoltLauncher.Shutdown();

            if (isServer)
            {
                Lobby.RemoveAllDisconnectedPlayers();
            }
        }

        this.launchButton.GetComponent<Text>().enabled = true;

        this.prevModeButton.GetComponent<Text>().enabled = true;
        this.nextMapButton.GetComponent<Text>().enabled = true;
        this.prevMapButton.GetComponent<Text>().enabled = true;
        this.nextModeButton.GetComponent<Text>().enabled = true;

        GameManager.instance.ChangeGameState(GameManager.GameState.MENU);
    }

    /// <summary>
    /// Loads the scene and begins the multiplayer game.
    /// </summary>
    public void StartGame()
    {
        Cursor.visible = false;

        // Uhhhh...hmmmmmm.
        // You may refer to the code in this class by it's official name "Thomas the Jank-Engine"
        BoltNetwork.LoadScene(
            string.Format("ingame_{0}_{1}",
            GameModeManager.GetSupportedMapsForGameMode(Lobby.GameModeName)[this.currentHostMap],
            GameManager.instance.GameMode.GetType().Name));
    }

    /// <summary>
    /// State updates.
    /// </summary>
    void Update()
    {
        DebugHUD.setValue("IsServer", BoltNetwork.isServer);
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

        if (this.gameModelLabel == null)
        {
            Debug.LogError("LobbyGameMenuActions expects a non-null gameModeLabel.");
            return;
        }

        if (this.teamLabel == null)
        {
            Debug.LogError("LobbyGameMenuActions expects a non-null teamLabel.");
            return;
        }

        if (this.mapLabel == null)
        {
            Debug.LogError("LobbyGameMenuActions expects a non-null mapLabel.");
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

        if (this.teamPanel == null)
        {
            Debug.LogError("LobbyGameMenuActions expects teamPanel to be non-null.");
            return;
        }

        // Subscribe to lobby events.
        Lobby.LobbyUpdatedEvent += Lobby_LobbyUpdatedEvent;
    }

    /// <summary>
    /// Lobby events handler.
    /// </summary>
    /// <param name="change">What change happened.</param>
    private void Lobby_LobbyUpdatedEvent(Lobby.LobbyChange change)
    {
        switch (change)
        {
            case Lobby.LobbyChange.ALL:
                this.ReloadPlayersList();
                this.UpdateModeLabel();
                this.UpdateMapLabel();
                break;
            case Lobby.LobbyChange.PLAYER_ADDED:
            case Lobby.LobbyChange.PLAYER_CHANGED:
            case Lobby.LobbyChange.PLAYER_REMOVED:
                this.ReloadPlayersList();
                this.UpdateTeamLabel();
                break;
            case Lobby.LobbyChange.MAP_CHANGED:
                this.UpdateMapLabel();
                break;
            case Lobby.LobbyChange.MODE_CHANGED:
                this.UpdateModeLabel();
                break;
        }
        VerifyGameCanStart();
    }

    /// <summary>
    /// Intercept scene change event and unregister event handler so we aren't causing
    /// problems across scene. For more info talk to Stephen, he seems to understand Bolt.
    /// </summary>
    /// <param name="map">The new scene.</param>
    public override void SceneLoadLocalBegin(string map)
    {
        // Deregister event listener at scene exit.
        Lobby.LobbyUpdatedEvent -= Lobby_LobbyUpdatedEvent;
    }

    /// <summary>
    /// Updates the game mode UI on change.
    /// </summary>
    private void UpdateModeLabel()
    {
        IGameMode mode = GameManager.instance.GameMode;
        this.gameModelLabel.text = mode.GameModeName;
        
        var player = this.GetCurrentLobbyPlayer();
        if (mode.UsesTeams) {
            if (player.Team < 1) {
                Lobby.SetPlayerTeam(player.Name, 1);
                UpdateTeamLabel();
                ReloadPlayersList();
            } else if (player.Team >= mode.MaxTeams + 1) {
                Lobby.SetPlayerTeam(player.Name, mode.MaxTeams);
                UpdateTeamLabel();
                ReloadPlayersList();
            }
            teamLabel.enabled = true;
        } else {
            teamLabel.enabled = false;
        }
    }

    /// <summary>
    /// Updates the current user's team label in the GUI from the
    /// LobbyPlayer's current team.
    /// </summary>
    private void UpdateTeamLabel()
    {
        var currentLobbyPlayer = GetCurrentLobbyPlayer();

        // GetCurrentLobbyPlayer() prints to log, just return.
        if (currentLobbyPlayer == null)
        {
            return;
        }

        if (!VerifyValidTeam(currentLobbyPlayer.Team))
        {
            //Debug.LogWarning("Current user has invalid team number.");
            return;
        }

        this.teamLabel.text = Teams.Names[currentLobbyPlayer.Team];
        this.teamPanel.color = ColorFromTeam(currentLobbyPlayer.Team);
    }

    private static bool VerifyValidTeam(int team) {
        return team >= (GameManager.instance.GameMode.UsesTeams ? 1 : 0) && team < (GameManager.instance.GameMode.UsesTeams ? (GameManager.instance.GameMode.MaxTeams + 1) : 8);
    }

    /// <summary>
    /// Gets the Lobby.LobbyPlayer for the current user. Log messages are written on error.
    /// </summary>
    /// <returns>The lobby player or null if an error occurs.</returns>
    private Lobby.LobbyPlayer GetCurrentLobbyPlayer()
    {
        var currentUsername = GameManager.instance.CurrentUserName;

        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogError("Cannot update team label, current username is null or empty.");
            return null;
        }

        var currentLobbyPlayer = Lobby.GetPlayer(currentUsername);

        if (currentLobbyPlayer == null)
        {
            //Debug.LogError("Current user is not a member of the current game.");
            return null;
        }

        return currentLobbyPlayer;
    }

    /// <summary>
    /// Updates the map UI on change.
    /// </summary>
    private void UpdateMapLabel()
    {
        if (Lobby.MapName != null)
        {
            this.mapLabel.text = GameModeManager.GetHumanReadableNameForMap(Lobby.MapName);

            if (!BoltNetwork.isClient)
            {
                this.launchButton.interactable = true;
            }
        }
        else
        {
            this.mapLabel.text = "No Compatible Maps";
            this.launchButton.interactable = false;
        }
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

        // Populate list with connected players.
        int nextY = -(this.listItemHeight / 2);
        foreach (var player in Lobby.AllPlayers)
        {
            var newItem = Instantiate(this.listItemPrefab);
            var textComponent = newItem.GetComponentInChildren<Text>();
            var imageComponent = newItem.GetComponentInChildren<Image>();

            // Team colors.
            imageComponent.color = ColorFromTeam(player.Team);
			
			// Add new item to the list box.
			newItem.transform.SetParent(this.scrollPanel.transform);
            textComponent.text = player.Name;

            // Set it to be straight up since we're rotating the menus.
			newItem.transform.localRotation = Quaternion.identity;

            // Set item's position in the box.
            // TODO: non-jank-ass coordinates.
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
        var color = Color.gray;

        // Not exhaustive error checking, just superficial for helping with merge.
        // Will break if Teams.Colors is length 0.
        if (team >= 0 || team < Teams.Colors.Length)
        {
            color = Teams.MenuColors[team];
        }
        else
        {
            Debug.LogError("Unknown team color.");
        }

        return color;
    }

    private bool GameCanStart() {
        if (!BoltNetwork.isServer) return false;
        foreach (var p in Lobby.AllPlayers) {
            if (!VerifyValidTeam(p.Team)) return false;
        }
        if (Lobby.GameModeName == null) return false;
        if (Lobby.MapName == null) return false;
        return true;
    }

    private void VerifyGameCanStart() {
        launchButton.interactable = GameCanStart();
    }
}
