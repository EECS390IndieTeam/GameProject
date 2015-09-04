using UnityEngine;
using System.Collections;

public class UITests : MonoBehaviour {

	PlayerHealth playerHealth;
	AmmoCounter ammoCounter;
	public GameObject player;

	// Use this for initialization
	void Start () {
		playerHealth = player.GetComponent <PlayerHealth> ();
		ammoCounter = player.GetComponent <AmmoCounter>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F)){
			playerHealth.TakeDamage(25);
		}
		if (Input.GetKeyDown(KeyCode.G)){
			ammoCounter.FireWeapon();
		}
		if (Input.GetKeyDown(KeyCode.R)){
			ammoCounter.Reload();
		}
	}
}
