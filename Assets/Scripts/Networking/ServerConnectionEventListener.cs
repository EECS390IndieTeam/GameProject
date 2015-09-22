using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class ServerConnectionEventListener : Bolt.GlobalEventListener {

    public static string ServerPassword = "dickbutt"; //this will be moved to a different class later
    //the password for literally everything is "dickbutt"!
    private string ServerUsername = "Im the server look at me!"; //this will also be moved later

    private LobbyState lobby;

    public override void BoltStartDone() {
        PlayerRegistry.CreatePlayer(null, ServerUsername);
        lobby = BoltNetwork.Instantiate(BoltPrefabs.LobbyList).GetComponent<LobbyState>();
        lobby.InitializeLobby();
    }

    public override void Connected(BoltConnection connection) {
        Bolt.IProtocolToken token = connection.ConnectToken;

        //if this is a development build or in the editor, user authorization is not required
        //this should allow for much faster debugging and testing
        if (Debug.isDebugBuild || Application.isEditor && token == null) {
            var newToken = new ConnectionRequestData();
            newToken.Password = ServerPassword;
            string baseusername = "debug player ";
            int suffix = 0;
            while (PlayerRegistry.UserConnected(baseusername + suffix)) suffix++;
            newToken.PlayerName = baseusername + suffix;
            token = newToken;
        }

        if (token != null && token is ConnectionRequestData) {
            ConnectionRequestData data = (ConnectionRequestData)token;
            Debug.Log("connection request with token of type " + token.GetType().Name);
            if (data.Password != ServerConnectionEventListener.ServerPassword) {
                //BoltNetwork.Refuse(endpoint, reason);
                connection.Disconnect(new DisconnectReason("Server Refused Connection", "Incorrect Password"));
            } else if (PlayerRegistry.UserConnected(data.PlayerName)) {
                //BoltNetwork.Refuse(endpoint, reason);
                connection.Disconnect(new DisconnectReason("Server Refused Connection", "A player with that name is already connected"));
            } else {
                PlayerRegistry.CreatePlayer(connection, data.PlayerName);
                lobby.AddPlayer(data.PlayerName);
                //player connected successfully!
                //BoltNetwork.Accept(endpoint);
            }
        } else {
            //BoltNetwork.Refuse(endpoint, reason);
            connection.Disconnect(new DisconnectReason("Server Refused Connection", "Invalid Connection Token"));
        }
    }

    public override void Disconnected(BoltConnection connection) {
        lobby.RemovePlayer(PlayerRegistry.GetUserNameFromConnection(connection));
        PlayerRegistry.Remove(connection);
    }

    public override void OnEvent(TeamChangeEvent evnt) {
        lobby.SetPlayerTeam(PlayerRegistry.GetUserNameFromConnection(evnt.RaisedBy), evnt.NewTeam);
    }

}
