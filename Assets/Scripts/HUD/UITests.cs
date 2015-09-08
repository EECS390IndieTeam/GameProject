using UnityEngine;
using System.Collections;

public class UITests : MonoBehaviour {

	PlayerHealth playerHealth;
	AmmoCounter ammoCounter;
	public GameObject player;
	public int ammoToPickup;
	public int damageToTake;

	// Use this for initialization
	void Start () {
		playerHealth = player.GetComponent <PlayerHealth> ();
		ammoCounter = player.GetComponent <AmmoCounter>();

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T)){
			playerHealth.TakeDamage(damageToTake);
		}
		if (Input.GetKeyDown(KeyCode.F)){
			ammoCounter.FireWeapon();
		}
		if (Input.GetKeyDown(KeyCode.R)){
			ammoCounter.Reload();
		}
		if (Input.GetKeyDown(KeyCode.P)){
			ammoCounter.PickupAmmo(ammoToPickup);
		}
	}
}
