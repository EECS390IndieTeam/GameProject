using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour]
public class BoltServerAndClientListener : Bolt.GlobalEventListener {
    public override void BoltStartBegin(){
        BoltNetwork.RegisterTokenClass<DisconnectReason>();
        BoltNetwork.RegisterTokenClass<ConnectionRequestData>();
    }
}
