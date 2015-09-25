using UnityEngine;
using System.Collections;

public class ThrowGrenade : MonoBehaviour {
	//Variables for Grenade
	public GameObject grenadePrefab;
	public int grenadeAmmo;

	public bool cooking = false;
	private float baseTime = 5.0f;
	public float grenadeTime = 5.0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// FixedUpdate is called once per frame, at a fixed rate of 60FPS
	void FixedUpdate () {
		/*
		 * --------------------------CREATE PROJECTILE---------------------------------------------------- 
		*/
		if (Input.GetKey(KeyCode.G) && grenadeAmmo > 0 && cooking == false) {
			cooking = true;
			grenadeTime = baseTime;
		}

		if (!Input.GetKey(KeyCode.G) && cooking == true) {
			Throw();
			cooking = false;
		}

		if (cooking == true) {
			grenadeTime = grenadeTime - (1.0f/60.0f);
			//TODO: Once "cooking" properly functional, set code here to detonate grenade if cooked too long.
		}

		//TODO: Implement "cooking" of grenade by holding G key.
		//Generally know how to do this - just set a boolean when G is pressed, reset it when G is released.
		//Keep track of how long G is held, and reduce the Grenade's duration by that amount when thrown (G released).
		//Just need to look up how to set variables in a spawned object, or find some similar way to reduce the duration of the new Grenade.
	}

	void Throw(){
		Vector3 grenadePosition = this.transform.position;
		Quaternion grenadeRotation = this.transform.rotation;
		var Grenade = Instantiate (grenadePrefab, grenadePosition, grenadeRotation);
		//var GrenadeScript = Grenade.GetComponent ("Grenade.cs");
		//GrenadeScript.setDetonate (grenadeTime);
		grenadeAmmo--;
	}
}
