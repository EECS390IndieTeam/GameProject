using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(FPSController))]
public class OwnerPlayer : AbstractPlayer {
    private FPSController fpsController;

    private bool _IsDead = false;
    /// <summary>
    /// True of the player is currently dead
    /// </summary>
    public bool IsDead {
        get {
            return _IsDead;
        }
        private set {
            _IsDead = value;
            foreach (var o in ObjectsToDisableWhileDead) o.SetActive(!value);
        }
    }

    /// <summary>
    /// These objects will be disabled when the player dies and re-enabled when the player respawns
    /// </summary>
    public GameObject[] ObjectsToDisableWhileDead;

    /// <summary>
    /// Set to false to disable the player's controls
    /// </summary>
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

    void Start()
    {
        this.Health = 100;
        this.MaxHealth = 100;
    }

    public override void Die(string killer, int weaponID) {
		//TODO check comments on Death
		//I thought death was a simple death animation combined with being moved around to your new spawn location.
        Debug.Log("Player " + Username + " was killed by "+killer+" using weapon #"+weaponID);
        //we need to tell the server that we died so that it can respawn us
        DeathEvent evnt = DeathEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.Killer = killer;
        evnt.Player = Username;
        evnt.WeaponID = weaponID;
        evnt.Send();
        this.Health = MaxHealth;
        this.IsDead = true;
        ControlEnabled = false;
    }

    public override void RespawnAt(Vector3 position, Quaternion rotation) {
        MoveTo(position, rotation);
        Health = MaxHealth;
        IsDead = false;
        ControlEnabled = true;
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
