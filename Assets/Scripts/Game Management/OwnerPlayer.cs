using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(FPSController))]
public class OwnerPlayer : AbstractPlayer {
    private FPSController fpsController;

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
        evnt.Killer = killer;
        evnt.Player = Username;
        evnt.WeaponID = weaponID;
        evnt.Send();
        this.Health = MaxHealth;
        this.IsDead = true;
        ControlEnabled = false;
        GetComponent<Collider>().enabled = false;
    }

    public override void RespawnAt(Vector3 position, Quaternion rotation) {
        MoveTo(position, rotation);
        Health = MaxHealth;
        IsDead = false;
        ControlEnabled = true;
        GetComponent<Collider>().enabled = true;
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
