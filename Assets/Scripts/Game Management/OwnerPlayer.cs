using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(FPSController))]
public class OwnerPlayer : AbstractPlayer {
    private FPSController fpsController;
    private Collider coll;
    private Rigidbody rb;

    public MeshRenderer[] materials;

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
        this.rb = GetComponent<Rigidbody>();
        this.coll = GetComponent<Collider>();
    }

    void Start() {
        this.Health = 100;
        this.MaxHealth = 100;
        SetColor(Teams.Colors[this.Team]);
    }

    public override void Die(string killer, int weaponID) {
        //TODO check comments on Death
        //I thought death was a simple death animation combined with being moved around to your new spawn location.
        Debug.Log("Player " + Username + " was killed by " + killer + " using weapon #" + weaponID);
        //we need to tell the server that we died so that it can respawn us
        DeathEvent evnt = DeathEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
		if (Username != killer) {
			SpawningBlind.instance.Text = "Killed by " + killer + "\nRespawing...";
		} else {
			SpawningBlind.instance.Text = "You killed yourself.\nRespawing...";
		}
		SpawningBlind.instance.FadeIn (3.5f);
        evnt.Killer = killer;
        evnt.Player = Username;
        evnt.WeaponID = weaponID;
        evnt.Send();
        this.Health = MaxHealth;
        this.IsDead = true;
        ControlEnabled = false;
        coll.enabled = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }

	public override void RespawnAt(Vector3 position, Quaternion rotation) {
        MoveTo(position, rotation);
        Health = MaxHealth;
        fpsController.grenade.RefillGrenades();
		fpsController.grappleGun.detach();
		fpsController.isAttachedToSurface = false;
		fpsController.EnableGun();
        IsDead = false;
        ControlEnabled = true;
        this.coll.enabled = true;
		rb.isKinematic = false;
		SpawningBlind.instance.Hide ();
    }

    public override void MoveTo(Vector3 position, Quaternion rotation) {
        this.transform.position = position;
        this.transform.rotation = rotation;
    }

    public override void TakeDamage(float Damage, string OtherUser, Vector3 direction, int weaponID) {
        if (!IsDead)
        {
            Health -= Damage;
            if (Health <= 0)
            {
                Debug.Log(OtherUser + " killed " + Username);
                Die(OtherUser, weaponID);
            }
        }
    }

    public override void SetColor(Color color) {
        foreach (MeshRenderer renderer in materials) {
            renderer.material.SetColor("_EmissionColor", color);
        }
    }
}
