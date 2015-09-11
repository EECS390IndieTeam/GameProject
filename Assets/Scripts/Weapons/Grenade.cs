using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
	private int bodyTimer = 0;			//Private counter to use for time-based effects.
	private float speed = 3;			//Speed at which the projectile moves.

	// Use this for initialization
	void Start () {
		// Provide velocity based on Rotation.
		//Y-rotation: Horizontal, positive = clockwise.
		//X-rotation: Up/down, positive = down.
		//thus, X = Cos (Y-rotation) times speed
		//Y = Sin (Y-rotation) times speed
		//Z = -Sin (X-rotation) times speed
		float zVel = Mathf.Cos(this.transform.eulerAngles.y * Mathf.Deg2Rad) * speed;
		float xVel = Mathf.Sin(this.transform.eulerAngles.y * Mathf.Deg2Rad) * speed;
		float yVel = 0 - (Mathf.Sin(this.transform.eulerAngles.x * Mathf.Deg2Rad) * speed);
		this.GetComponent<Rigidbody> ().velocity = new Vector3 (xVel, yVel, zVel);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//FixedUpdate called at a fixed framerate of 60
	void FixedUpdate() {
		//Enable body collision after 1 second.
		if (bodyTimer == 60) {
			this.GetComponent<SphereCollider> ().enabled = true;
		}
		//Self-destroy object after 10 seconds.
		//TODO: implement an array or something to allow recycling of the grenade projectiles.
		if (bodyTimer == 600) {
			//Put code to detonate and harm nearby players here.
			Destroy (this.gameObject);
		}
		bodyTimer++;
	}
}
