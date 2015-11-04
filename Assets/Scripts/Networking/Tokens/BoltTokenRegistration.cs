using UnityEngine;
using System.Collections;

/// <summary>
/// This class just registers all the different tokens that we use
/// </summary>
[BoltGlobalBehaviour]
public class BoltTokenRegistration : Bolt.GlobalEventListener {
    public override void BoltStartBegin(){
        BoltNetwork.RegisterTokenClass<DisconnectReason>();
        BoltNetwork.RegisterTokenClass<ConnectionRequestData>();
        BoltNetwork.RegisterTokenClass<ServerInfoToken>();
        BoltNetwork.RegisterTokenClass<FullLobbyDataToken>();
        BoltNetwork.RegisterTokenClass<LobbyUpdateToken>();
        BoltNetwork.RegisterTokenClass<StatListToken>();
        BoltNetwork.RegisterTokenClass<StatUpdateToken>();
    }
}
