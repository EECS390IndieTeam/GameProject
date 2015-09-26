using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GrappleGun), typeof(FPSController))]
public class OwnerPlayer : AbstractPlayer {
    private GrappleGun GrappleGun;
    private FPSController Controller;

    void Awake() {
        GameManager.instance.SetCurrentPlayer(this);
    }

    public override void Die() {
		//TODO check comments on Death
		//Do we need to reset the available weapon list here like this? Probably not...
		//Actually, do we need to reset the references on any of these components?
		//I thought death was a simple death animation combined with being moved around to your new spawn location.
        AvailableWeapons = new List<IWeapon>();
        GrappleGun = GetComponent<GrappleGun>();
        Controller = GetComponent<FPSController>();
        Debug.Log("Player " + Username + " died a horrible, painful death");
		//TODO add in movement back to a spawn location
    }

    public override void RespawnAt(Transform location) {
        Health = MaxHealth;
        MoveTo(location);
    }

    public override void MoveTo(Transform location) {
        this.transform.position = location.position;
        this.transform.rotation = location.rotation;
    }

    public override void TakeDamage(float Damage, string OtherUser, Vector3 direction) {
        Health -= Damage;
        if (Health <= 0) {
            Debug.Log(OtherUser + " killed " + Username);
            Die();
        }
    }
}
