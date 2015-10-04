using UnityEngine;
using System.Collections;


/// <summary>
/// This script recieves all HurtEvents.  On clients, this script simply damages the current player by the amount needed.
/// It works a bit different for the server; if the damage event is intended to hurt the server's player, it works like a client.  
/// Otherwise, the server will create a new HurtEvent to send to the player who actually took the damage.  This is because
/// clients cannot send events directly to each other, instead the server needs to forward it to the correct player
/// </summary>
[BoltGlobalBehaviour("ingame*")]
public class DamageEventListener : Bolt.GlobalEventListener {
    public override void OnEvent(HurtEvent evnt) {
        if (BoltNetwork.isServer) {
            //TODO damage stats can be recorded here if needed, all HurtEvents pass through here exactly once

            //Direct damage events, either by consuming them here (if they were meant for the server player)
            //Or by redirecting them to the right player

            //if this HurtEvent is targeted at the server itself, apply it directly without forwarding it first
            if (evnt.Target == GameManager.instance.CurrentUserName) {
                GameManager.instance.CurrentPlayer.TakeDamage(evnt.Amount, evnt.Source, evnt.Direction, evnt.WeaponID);
            } else {//this event is not directed at the server player, it needs to be forwarded to the correct player
                BoltConnection destination = PlayerRegistry.GetConnectionFromUserName(evnt.Target);
                if (destination == null) {//quick error check
                    Debug.LogError("The destination connection for a HurtEvent could not be found! "+evnt);
                } else {
                    //forward the event to the player that was actually damaged
                    HurtEvent redir = HurtEvent.Create(destination, Bolt.ReliabilityModes.ReliableOrdered);
                    redir.Amount = evnt.Amount;
                    redir.Target = evnt.Target;
                    redir.Source = evnt.Source;
                    redir.WeaponID = evnt.WeaponID;
                    redir.Direction = evnt.Direction;
                    redir.Send();
                }
            }
        } else { //this is a client
            if (evnt.Target == GameManager.instance.CurrentUserName) { //make sure that this hurtEvent is intended for this client
                GameManager.instance.CurrentPlayer.TakeDamage(evnt.Amount, evnt.Source, evnt.Direction, evnt.WeaponID);
            } else {
                Debug.LogError("Recieved HurtEvent that is not directed to this player! " + evnt.ToString());
            }
        }
    }
}
