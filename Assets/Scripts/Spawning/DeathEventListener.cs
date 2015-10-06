using UnityEngine;
using System.Collections;

/// <summary>
/// This script listens for DeathEvents
/// </summary>
[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame*")]
public class DeathEventListener : Bolt.GlobalEventListener {
    public override void OnEvent(DeathEvent evnt) {
        //TODO record who died

        //because of the Owner/ProxyPlayer abstraction, this line will either just move the server's player, or
        //send an MovePlayerEvent to the propper player!
        GameManager.instance.gameMode.MovePlayerToSpawnPoint(PlayerRegistry.GetIPlayerForUserName(evnt.Player));
       
    }
}
