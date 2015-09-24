using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GrenadeState : Bolt.EntityBehaviour<IGrenadeState> {
    public override void Attached() {
        state.Transform.SetTransforms(this.transform);
        if (!entity.isOwner) {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public float DetonateTime {
        get {
            return state.DetonateTime;
        }
    }

    public string Thrower {
        get {
            return state.Thrower;
        }
    }


}
