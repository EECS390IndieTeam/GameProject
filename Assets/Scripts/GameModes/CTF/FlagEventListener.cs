using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame*")]
public class FlagEventListener : Bolt.GlobalEventListener {

    void Awake() {
        if (GameManager.instance.GameMode.Mode != GameModes.CAPTURE_THE_FLAG) {
            BoltNetwork.RemoveGlobalEventListener(this);
            Destroy(this);
        }
    }

    public override void OnEvent(FlagDroppedEvent evnt) {
        AbstractPlayer player = PlayerRegistry.GetIPlayerForUserName(evnt.Carrier) as AbstractPlayer;
        Flag.SpawnFlag(evnt.FlagTeam, player.transform.position, player.transform.rotation);
    }
}
