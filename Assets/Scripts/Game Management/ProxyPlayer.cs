using UnityEngine;
using System.Collections;

public class ProxyPlayer : AbstractPlayer {

    public override void Die() {
        if (!BoltNetwork.isServer) throw new System.NotImplementedException();
    }

    public override void RespawnAt(Transform location) {
        if (!BoltNetwork.isServer) throw new System.NotImplementedException();
    }

    public override void MoveTo(Transform location) {
        if(!BoltNetwork.isServer) throw new System.NotImplementedException();

    }

    public override void TakeDamage(float Damage, string OtherUser, Vector3 direction) {
        HurtEvent evnt = HurtEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.Amount = Damage;
        evnt.Player = OtherUser;
        evnt.Direction = direction;
        evnt.FromAttacker = true;
        evnt.Send();
    }
}
