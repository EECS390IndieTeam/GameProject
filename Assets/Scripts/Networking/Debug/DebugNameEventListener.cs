using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class DebugNameEventListener : Bolt.GlobalEventListener {
    public override void OnEvent(DebugNameEvent evnt) {
        GameManager.instance.CurrentUserName = evnt.NewName;
    }
}
