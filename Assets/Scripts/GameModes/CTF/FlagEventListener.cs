using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame*")]
public class FlagEventListener : Bolt.GlobalEventListener {

    private bool ignore = false;

    void Awake() {
        if (GameManager.instance.GameMode.Mode != GameModes.CAPTURE_THE_FLAG) {
            ignore = true;
        }
    }

    public override void OnEvent(FlagDroppedEvent evnt) {
        if (ignore) return;
        AbstractPlayer player = PlayerRegistry.GetIPlayerForUserName(evnt.Carrier) as AbstractPlayer;
        Flag.SpawnFlag(evnt.FlagTeam, player.transform.position, player.transform.rotation);
    }
}
