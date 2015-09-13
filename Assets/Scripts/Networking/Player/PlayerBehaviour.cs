using UnityEngine;
using System.Collections;

public class PlayerBehaviour : Bolt.EntityBehaviour<IPlayerState> {
    public float Health {
        get {
            return state.Health;
        }
    }
    public override void Attached() {
        state.Transform.SetTransforms(transform);
    }

    public override void SimulateController() {
    }

    public void DoDamage(float amount, Vector3 source, BoltPlayer who) {
        state.Health -= amount;
        
    }
}
