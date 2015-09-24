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
        AvailableWeapons = new List<IWeapon>();
        GrappleGun = GetComponent<GrappleGun>();
        Controller = GetComponent<FPSController>();
        Debug.Log("Player " + Username + " died a horrible, painful death");
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
