using UnityEngine;

/// <summary>
/// Contains menu actions for game lobby.
/// </summary>
public class LobbyGameMenuActions : Bolt.GlobalEventListener
{
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
}
