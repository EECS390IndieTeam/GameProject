using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An abstract implemtation of IPlayer.  This class contains all the code common to 
/// both OwnerPlayers and ProxyPlayers.  This class handles any IPlayer fields that need to
/// be synchronized by simply forwarding the gets and sets to the corresponding fields in the
/// PlayerState.  
/// </summary>
public abstract class AbstractPlayer : MonoBehaviour, IPlayer {

    /// <summary>
    /// A reference to the attached PlayerState
    /// </summary>
    private PlayerState state;

    /// <summary>
    /// Sets the PlayerState to use.  We can't simply use GetComponent because when the prefab
    /// containing the implementation of this script, it initially has no parent transform, so we don't
    /// have a good way to find the PlayerState.  instead, the PlayerState itself calls SetState(this) 
    /// after it is done with all its instantiation.  
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(PlayerState newState) {
        state = newState;
        MaxHealth = Health;
    }


	public string Username {
		get{
            return state.Name;
        }
		protected set{
            state.Name = value;
        }
	}

    public int CurrentWeaponID {
        get {
            return state.SelectedWeapon;
        }
        protected set {
            state.SelectedWeapon = value;
        }
    }
	public int Team {
        get {
            return state.Team;
        }
        protected set {
            state.Team = value;
        }
	}
	public System.Collections.Generic.List<IWeapon> AvailableWeapons {
		get;
		protected set;
	}
	public IWeapon CurrentWeapon {
		get;
		protected set;
	}
	public float Health {
        get {
            return state.Health;
        }
        protected set {
            state.Health = value;
        }
	}

	public float MaxHealth {
		get;
		protected set;
	}

    public Vector3 GrappleEndpoint {
        get {
            return state.GrappleEndpoint;
        }
        set {
            state.GrappleEndpoint = value;
        }
    }

    public bool GrappleVisible {
        get {
            return state.GrappleVisible;
        }
        set {
            state.GrappleVisible = value;
        }
    }

    public abstract void Die(string killer, int weaponID);

    public void RespawnAt(Transform location) {
        RespawnAt(location.position, location.rotation);
    }

    public abstract void RespawnAt(Vector3 position, Quaternion rotation);

    public abstract void MoveTo(Vector3 position, Quaternion rotation);

    public void MoveTo(Transform location) {
        MoveTo(location.position, location.rotation);
    }

    public abstract void TakeDamage(float Damage, string DamageSource, Vector3 direction, int weaponID);
}
