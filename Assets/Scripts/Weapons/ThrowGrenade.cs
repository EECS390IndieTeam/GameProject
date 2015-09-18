using UnityEngine;
using System.Collections;

public class ThrowGrenade : MonoBehaviour {
	//Variables for Grenade
	public GameObject grenadePrefab;
	public int grenadeAmmo;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 * --------------------------CREATE PROJECTILE---------------------------------------------------- 
		*/
		if (Input.GetKeyDown(KeyCode.G) && grenadeAmmo > 0) {
			Vector3 grenadePosition = this.transform.position;
			Quaternion grenadeRotation = this.transform.rotation;
			Instantiate(grenadePrefab, grenadePosition, grenadeRotation);
			grenadeAmmo--;
		}

		//TODO: Implement "cooking" of grenade by holding G key.
		//Generally know how to do this - just set a boolean when G is pressed, reset it when G is released.
		//Keep track of how long G is held, and reduce the Grenade's duration by that amount when thrown (G released).
		//Just need to look up how to set variables in a spawned object, or find some similar way to reduce the duration of the new Grenade.
	}
}
