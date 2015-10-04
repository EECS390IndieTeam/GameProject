using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Contains menu actions for game lobby.
/// </summary>
public class LobbyGameMenuActions : Bolt.GlobalEventListener
{
    /// <summary>
    /// An editor provided list of buttons, one for each team.
    /// </summary>
    public List<Button> teamButtonsList;

    /// <summary>
    /// Fatal error occurred.
    /// </summary>
    private bool dead;

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
        BoltNetwork.LoadScene("ingame");
    }

    /// <summary>
    /// Setup.
    /// </summary>
    void Start()
    {
        if (this.teamButtonsList != null || this.teamButtonsList.Count == 0)
        {
            Debug.LogError("Lobby Game Menu Actions Team buttons list cannot be empty.");
            this.dead = true;
            return;
        }

        // Create a click listener for each of the 7 buttons in ascending order
        // assigning them to teams 0 through 7.
        var i = 0;
        foreach (var button in this.teamButtonsList)
        {
            button.onClick.AddListener(() =>
            {
                TeamChangeEvent evnt = TeamChangeEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
                evnt.NewTeam = i++;
                evnt.Send();
            });
        }
    }

    /// <summary>
    /// State updates.
    /// </summary>
    void Update()
    {
        if (this.dead)
        {
            return;
        }

        if (!BoltNetwork.isRunning)
        {
            Application.LoadLevel(1);
            return;
        }

        // TODO: fix this spelling mistake in the code whereever it originated.
        DebugHUD.setValue("IsSever", BoltNetwork.isServer);

        if (BoltNetwork.isClient)
        {
            DebugHUD.setValue("ping", BoltNetwork.server.PingNetwork);
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
}
