using UnityEngine;
using System.Collections;

/// <summary>
/// This script allows clients to listen for PlayerMoveEvents and move their players accordingly.  
/// The server doesn't need this because only the server can send PlayerMoveEvents, and if the
/// server needs to move its own player, it does so directly instead of by sending an event to itself.
/// </summary>
[BoltGlobalBehaviour(BoltNetworkModes.Client, "ingame*")]
public class PlayerMoveEventListener : Bolt.GlobalEventListener {
    public override void OnEvent(PlayerMoveEvent evnt) {
        IPlayer player = GameManager.instance.CurrentPlayer;
        if (evnt.Respawn) {
            player.RespawnAt(evnt.NewPosition, evnt.NewRotation);
        } else {
            player.MoveTo(evnt.NewPosition, evnt.NewRotation);
        }
    }
}
