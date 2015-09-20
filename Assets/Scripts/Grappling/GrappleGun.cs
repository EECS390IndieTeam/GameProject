using UnityEngine;
using System.Collections;

public class GrappleGun : MonoBehaviour {

	[System.NonSerialized]
	public FPSController controller;

	public float maxDistance = 200f;

	public float reelSpeed = 15f;
	public float stabilizer = 1.1f;
	public int maxReelMultiplier = 3;

	public RaycastHit grappleHitInfo;

	private Lightning lightning;

	void Start() {
		lightning = GetComponent<Lightning>();
		lightning.maxLength = maxDistance;

		GrapplePhysics.reelSpeed = reelSpeed;
		GrapplePhysics.stabilizer = stabilizer;
	}

	void LateUpdate() {
		if (controller.grappled) {
			drawBeam();
		}
	}

	public void fire() {
		Physics.Raycast(controller.cameraTransform.position, controller.cameraTransform.forward, out grappleHitInfo, maxDistance);
		if (grappleHitInfo.collider /*&& hit.collider.gameObject.something.grapplable*/) {
			attach ();

			lightning.enabled = true;
		}

	}
	
	public void attach() {
		controller.grappled = true;
		GrapplePhysics.anchor = grappleHitInfo.point;
		GrapplePhysics.Length = grappleHitInfo.distance;
	}

	public void drawBeam() {
		lightning.targetPoint = GrapplePhysics.anchor;
		lightning.length = Vector3.Magnitude(GrapplePhysics.anchor - controller.character.position);
		lightning.numPoints = (int)(lightning.length / 6);
	}

	public void reelIn() {
		if (GrapplePhysics.reelMultiplier < maxReelMultiplier) {
			GrapplePhysics.reelMultiplier++;
		}
	}

	public void detach() {
		controller.grappled = false;
		GrapplePhysics.reelMultiplier = 0;

		lightning.enabled = false;
	}

}