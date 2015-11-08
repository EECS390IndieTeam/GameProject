using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour]
public class BoltTokenRegistration : Bolt.GlobalEventListener {
    public override void BoltStartBegin(){
        BoltNetwork.RegisterTokenClass<DisconnectReason>();
        BoltNetwork.RegisterTokenClass<ConnectionRequestData>();
        BoltNetwork.RegisterTokenClass<ServerInfoToken>();
    }
}
