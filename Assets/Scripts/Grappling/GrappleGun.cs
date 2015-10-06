using UnityEngine;
using System.Collections;

public class GrappleGun : MonoBehaviour {

	[System.NonSerialized]
	public FPSController controller;

	public float beamSpeed = 1000;
	private float beamSpeedTimer = 0;

	public float maxDistance = 200f;

	public float reelSpeed = 15f;
	public float stabilizer = 1.1f;
	public int maxReelMultiplier = 3;

	public RaycastHit grappleHitInfo;

	public LayerMask grappleTo;

	public AudioSource grappleFireSound;

	private Lightning lightning;

	private bool beamFiring = false;

	void Start() {
		lightning = GetComponent<Lightning>();
		lightning.maxLength = maxDistance;
		beamSpeedTimer = 0;
		GrapplePhysics.reelSpeed = reelSpeed;
		GrapplePhysics.stabilizer = stabilizer;
	}

	void Update() {
		if (beamFiring) {
			beamSpeedTimer += beamSpeed * Time.deltaTime;

			if (beamSpeedTimer >= GrapplePhysics.Length) {
				beamFiring = false;
				beamSpeedTimer = 0;
				controller.grappled = true;
			}
		}
	}

	void LateUpdate() {
		if (beamFiring) {
			drawBeam (controller.character.position + (GrapplePhysics.anchor - controller.character.position) * beamSpeedTimer / GrapplePhysics.Length);
		}

		if (!beamFiring && controller.grappled) {
			drawBeam(GrapplePhysics.anchor);
		}
	}

	public void fire() {
		Debug.Log ("firing");
		Physics.Raycast(controller.cameraTransform.position, controller.cameraTransform.forward, out grappleHitInfo, maxDistance, grappleTo);
		if (grappleHitInfo.collider && !grappleHitInfo.collider.gameObject.GetComponent<Rigidbody>()) {
			Debug.Log ("successful fire");
			beamFiring = true;
			lightning.enabled = true;
			GrapplePhysics.anchor = grappleHitInfo.point;
			GrapplePhysics.Length = grappleHitInfo.distance;
			if (grappleFireSound) {
				grappleFireSound.Play ();
			}
		}

	}

	public void drawBeam(Vector3 position) {
		lightning.targetPoint = position;
		lightning.length = Vector3.Magnitude(position - controller.character.position);
		lightning.numPoints = (int)(lightning.length / 6);
	}

	public void reelIn() {
		if (GrapplePhysics.reelMultiplier < maxReelMultiplier) {
			GrapplePhysics.reelMultiplier++;
		}
	}

	public void detach() {
		beamFiring = false;
		controller.grappled = false;
		GrapplePhysics.reelMultiplier = 0;

		lightning.enabled = false;
	}

}