using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GrappleGun),typeof(FPSController))]
public class Player : MonoBehaviour, IPlayer {

	private GrappleGun GrappleGun;
	private FPSController Controller;

	// Use this for initialization
	void Start () {
		AvailableWeapons = new List<IWeapon> ();
		GrappleGun = GetComponent<GrappleGun> ();
		Controller = GetComponent<FPSController> ();
	}

	#region IPlayer implementation
	public void Die ()
	{
		throw new System.NotImplementedException ();
	}
	public void RespawnAt (Transform location)
	{
		throw new System.NotImplementedException ();
	}
	public void MoveTo (Transform location)
	{
		throw new System.NotImplementedException ();
	}
	public void TakeDamage (float Damage, string OtherUser)
	{
		Health -= Damage;
		if (Health <= 0) {
			Die ();

			//TODO implement the respawn based off the current game mode and other context things.
		}
	}
	public string Username {
		get;
		set;
	}
	public int TeamID {
		get;
		set;
	}
	public System.Collections.Generic.List<IWeapon> AvailableWeapons {
		get;
		private set;
	}
	public IWeapon CurrentWeapon {
		get;
		private set;
	}
	public float Health {
		get;
		private set;
	}

	public float MaxHealth {
		get;
		private set;
	}

	#endregion
}
