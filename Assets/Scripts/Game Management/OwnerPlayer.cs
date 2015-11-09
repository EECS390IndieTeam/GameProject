using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(FPSController))]
public class OwnerPlayer : AbstractPlayer {
    private FPSController fpsController;

    public bool ControlEnabled {
        get {
            return fpsController.enabled;
        }
        set {
            fpsController.enabled = value;
        }
    }

    void Awake() {
        this.fpsController = GetComponent<FPSController>();
        GameManager.instance.SetCurrentPlayer(this);
    }

	
	public Transform HandPoint;

    void Start()
    {
        this.Health = 100;
        this.MaxHealth = 100;
    }

    public override void Die(string killer, int weaponID) {
		//TODO check comments on Death
		//Do we need to reset the available weapon list here like this? Probably not...
		//Actually, do we need to reset the references on any of these components?
		//I thought death was a simple death animation combined with being moved around to your new spawn location.
        AvailableWeapons = new List<IWeapon>();
        Debug.Log("Player " + Username + " was killed by "+killer+" using weapon #"+weaponID);
        //we need to tell the server that we died so that it can respawn us
        DeathEvent evnt = DeathEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.Killer = killer;
        evnt.Player = Username;
        evnt.WeaponID = weaponID;
        evnt.Send();
        this.Health = MaxHealth;
    }

    public override void RespawnAt(Vector3 position, Quaternion rotation) {
        Health = MaxHealth;
        MoveTo(position, rotation);
    }

    public override void MoveTo(Vector3 position, Quaternion rotation) {
        this.transform.position = position;
        this.transform.rotation = rotation;
    }

    public override void TakeDamage(float Damage, string OtherUser, Vector3 direction, int weaponID) {
        Health -= Damage;
        if (Health <= 0) {
            Debug.Log(OtherUser + " killed " + Username);
            Die(OtherUser, weaponID);
        }
    }
}
