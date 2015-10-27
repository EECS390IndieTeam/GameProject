using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame*")]
public class FlagEventListener : Bolt.GlobalEventListener {

    public override void OnEvent(FlagDroppedEvent evnt) {
        evnt.Flag.GetComponent<Flag>().DropFlag();
    }
}
