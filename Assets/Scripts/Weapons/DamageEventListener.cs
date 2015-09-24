using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour("ingame*")]
public class DamageEventListener : Bolt.GlobalEventListener {
    //TODO these FindObjectOfTypes will need to be changed later
    public override void OnEvent(HurtEvent evnt) {
        if (BoltNetwork.isServer) {
            if (evnt.FromAttacker) {
                BoltConnection destConnection = PlayerRegistry.GetConnectionFromUserName(evnt.Player);
                if (destConnection == null) {
                    GameManager.instance.CurrentPlayer.TakeDamage(evnt.Amount, PlayerRegistry.GetUserNameFromConnection(evnt.RaisedBy), evnt.Direction);
                } else {
                    HurtEvent redir = HurtEvent.Create(destConnection, Bolt.ReliabilityModes.ReliableOrdered);
                    redir.Player = PlayerRegistry.GetUserNameFromConnection(evnt.RaisedBy);
                    redir.Amount = evnt.Amount;
                    redir.Direction = evnt.Direction;
                    redir.FromAttacker = false;
                    redir.Send();
                }
            } else {
                GameManager.instance.CurrentPlayer.TakeDamage(evnt.Amount, PlayerRegistry.GetUserNameFromConnection(evnt.RaisedBy), evnt.Direction);
            }
        } else {
            if (!evnt.FromAttacker) {
                GameManager.instance.CurrentPlayer.TakeDamage(evnt.Amount, evnt.Player, evnt.Direction);
            }
        }
    }
}
