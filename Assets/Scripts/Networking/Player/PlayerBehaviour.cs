using UnityEngine;
using System.Collections;

public class PlayerBehaviour : Bolt.EntityBehaviour<IPlayerState> {
    public float Health {
        get {
            return state.Health;
        }
    }

    public string Name {
        get {
            return state.Name;
        }
    }

    public int SelectedWeapon {
        get {
            return state.SelectedWeapon;
        }
        set {
            state.SelectedWeapon = value;
        }
    }

    public override void Attached() {
        state.Transform.SetTransforms(transform);
    }

    public void DoDamage(float amount, Vector3 source, BoltPlayer who) {
        state.Health -= amount;
        //TODO finish this
    }

    public override void SimulateOwner() {
        Vector3 pos = transform.position;
        pos.x += Input.GetAxis("Horizontal");
        pos.z += Input.GetAxis("Vertical");
        if (Input.GetButton("Fire3")) pos.y--;
        if (Input.GetButton("Jump")) pos.y++;
        transform.position = pos;
    }
}
