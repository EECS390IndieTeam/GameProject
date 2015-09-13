using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class ServerGlobalEventListener : Bolt.GlobalEventListener {

    public static string ServerPassword = "dickbutt"; //this will be moved to a different class later
    //the password for literally everything is "dickbutt"!
    private string ServerUsername = "Im the server look at me!"; //this will also be moved later

    public override void BoltStartDone() {
        var p = PlayerRegistry.CreatePlayer(null);
        p.Name = ServerUsername;

        BoltPlayer.LocalPlayer = p;
    }

    public override void Connected(BoltConnection connection) {
        Bolt.IProtocolToken token = connection.ConnectToken;
        if (token != null && token is ConnectionRequestData) {
            ConnectionRequestData data = (ConnectionRequestData)token;
            Debug.Log("connection request with token of type " + token.GetType().Name);
            if (data.Password != ServerGlobalEventListener.ServerPassword) {
                DisconnectReason reason = new DisconnectReason();
                reason.Reason = "Server Refused Connection";
                reason.Message = "Incorrect Password";
                //BoltNetwork.Refuse(endpoint, reason);
                connection.Disconnect(reason);
            } else if (PlayerRegistry.usernameConnected(data.PlayerName)) {
                DisconnectReason reason = new DisconnectReason();
                reason.Reason = "Server Refused Connection";
                reason.Message = "A player with that name is already connected";
                //BoltNetwork.Refuse(endpoint, reason);
                connection.Disconnect(reason);
            } else {
                var p = PlayerRegistry.CreatePlayer(connection);
                p.Name = data.PlayerName;
                //player connected successfully!
                //BoltNetwork.Accept(endpoint);
            }
        } else {
            DisconnectReason reason = new DisconnectReason();
            reason.Reason = "Server Refused Connection";
            reason.Message = "Invalid Connection Token";
            //BoltNetwork.Refuse(endpoint, reason);
            connection.Disconnect(reason);
        }
    }

    public override void Disconnected(BoltConnection connection) {
        PlayerRegistry.Remove(connection);
    }

}
