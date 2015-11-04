using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class ServerConnectionEventListener : Bolt.GlobalEventListener {
    private LobbyState lobby;
    public static PlayerIndexMap IndexMap;

    public override void BoltStartDone() {
        IndexMap = new PlayerIndexMap();
        if (GameManager.instance.CurrentUserName == "") GameManager.instance.CurrentUserName = "Server Player";
        PlayerRegistry.CreatePlayer(null, GameManager.instance.CurrentUserName);
        IndexMap.AddPlayer(GameManager.instance.CurrentUserName);
        lobby = BoltNetwork.Instantiate(BoltPrefabs.LobbyList).GetComponent<LobbyState>();
        lobby.InitializeLobby();
        Lobby.AddPlayer(GameManager.instance.CurrentUserName, 0);
        Lobby.SetPlayerIsHost(GameManager.instance.CurrentUserName, true);
    }

    public override void Connected(BoltConnection connection) {
        Bolt.IProtocolToken token = connection.ConnectToken;

        //if this is a development build or in the editor, user authorization is not required
        //this should allow for much faster debugging and testing
        if ((Debug.isDebugBuild || Application.isEditor) && token == null) {
            var newToken = new ConnectionRequestData();
            newToken.Password = ServerSideData.Password;
            string baseusername = "debug player ";
            int suffix = 0;
            while (PlayerRegistry.UserConnected(baseusername + suffix)) suffix++;
            string name = baseusername + suffix;
            newToken.PlayerName = name;
            token = newToken;
            DebugNameEvent evnt = DebugNameEvent.Create(connection, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.NewName = name;
            evnt.Send();
        }

        if (token != null && token is ConnectionRequestData) {
            ConnectionRequestData data = (ConnectionRequestData)token;
            Debug.Log("connection request with token of type " + token.GetType().Name);
            if (data.Password != ServerSideData.Password) {
                connection.Disconnect(new DisconnectReason("Server Refused Connection", "Incorrect Password"));
            } else if (PlayerRegistry.UserConnected(data.PlayerName)) {
                connection.Disconnect(new DisconnectReason("Server Refused Connection", "A player with that name is already connected"));
            } else if(GameManager.instance.CurrentGameState == GameManager.GameState.IN_GAME || GameManager.instance.CurrentGameState == GameManager.GameState.POST_GAME){
                if (!IndexMap.ContainsPlayer(data.PlayerName)) {
                    connection.Disconnect(new DisconnectReason("Server Refused Connection", "Game already in progress"));
                } else {
                    //Lobby 2.0
                    Lobby.SetPlayerConnected(data.PlayerName, true);
                }
            }else if(data.PlayerName.StartsWith(Lobby.PP_PREFIX)){
                connection.Disconnect(new DisconnectReason("Server Refused Connection", "Invalid Username; Usernames may not begin with "+Lobby.PP_PREFIX));
            } else{
                PlayerRegistry.CreatePlayer(connection, data.PlayerName);
                //Lobby 1.0
                lobby.AddPlayer(data.PlayerName);
                lobby.SetPlayerStatIndex(data.PlayerName, IndexMap.AddPlayer(data.PlayerName));

                //Lobby 2.0
                Lobby.AddPlayer(data.PlayerName, 0);
                Lobby.SendFullDataToClient(connection);
                //player connected successfully!
            }
        } else {
            connection.Disconnect(new DisconnectReason("Server Refused Connection", "Invalid Connection Token"));
        }
    }

    public override void Disconnected(BoltConnection connection) {
        //Lobby 1.0
        lobby.RemovePlayer(PlayerRegistry.GetUserNameFromConnection(connection));

        //Lobby 2.0
        if (GameManager.instance.CurrentGameState == GameManager.GameState.IN_GAME || GameManager.instance.CurrentGameState == GameManager.GameState.POST_GAME) {
            Lobby.SetPlayerConnected(PlayerRegistry.GetUserNameFromConnection(connection), false);
        } else {
            Lobby.RemovePlayer(PlayerRegistry.GetUserNameFromConnection(connection));
        }

        PlayerRegistry.Remove(connection);
    }

    public override void OnEvent(TeamChangeEvent evnt) {
        //Lobby 1.0
        lobby.SetPlayerTeam(PlayerRegistry.GetUserNameFromConnection(evnt.RaisedBy), evnt.NewTeam);

        //Lobby 2.0
        Lobby.SetPlayerTeam(PlayerRegistry.GetUserNameFromConnection(evnt.RaisedBy), evnt.NewTeam);

    }

}
