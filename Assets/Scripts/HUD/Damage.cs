using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour {

	PlayerHealth playerHealth;
	public GameObject player;

	// Use this for initialization
	void Start () {
		playerHealth = player.GetComponent <PlayerHealth> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F)){
			playerHealth.TakeDamage(25);
		}
	}
}
