using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class AbstractPlayer : MonoBehaviour, IPlayer {


    private PlayerState state;

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
	public int Team {//TODO bolt stuff here
		get;
		protected set;
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

    public abstract void Die();

    public abstract void RespawnAt(Transform location);

    public abstract void MoveTo(Transform location);

    public abstract void TakeDamage(float Damage, string OtherUser, Vector3 direction);
}
