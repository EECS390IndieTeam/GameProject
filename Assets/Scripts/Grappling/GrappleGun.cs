using UnityEngine;
using System.Collections;

public class GrappleGun : MonoBehaviour {

	[System.NonSerialized]
	public FPSController controller;

	public float beamSpeed = 1000;
	private float beamSpeedTimer = 0;

	public float maxDistance = 200f;
    public float minDistance = 2.0f;

	public float reelSpeed = 15f;
	public float stabilizer = 1.1f;
	public int maxReelMultiplier = 3;

	public RaycastHit grappleHitInfo;

	public LayerMask grappleTo;

	public AudioSource grappleFireSound;
	public AudioSource grappleOnSound;
	public AudioSource grappleDetachSound;

	private Lightning lightning;

    private AbstractPlayer player;

	[System.NonSerialized]
	public bool beamFiring = false;

	void Start() {
		lightning = GetComponent<Lightning>();
		beamSpeedTimer = 0;
		GrapplePhysics.reelSpeed = reelSpeed;
		GrapplePhysics.stabilizer = stabilizer;
        player = (AbstractPlayer)GameManager.instance.CurrentPlayer;
	}

	void Update() {
        DebugHUD.setValue("Grappled", controller.grappled);

		if (beamFiring) {
			beamSpeedTimer += beamSpeed * Time.deltaTime;

			if (beamSpeedTimer >= GrapplePhysics.Length) {
				beamFiring = false;
				beamSpeedTimer = 0;
				controller.grappled = true;
			}
		}
        
        if (controller.grappled && GrapplePhysics.sqrDistance != 0 && GrapplePhysics.sqrDistance <= minDistance * minDistance)
        {
            detach();
        }
	}

	void LateUpdate() {
		if (beamFiring) {
			drawBeam (controller.character.position + (GrapplePhysics.anchor - controller.character.position) * beamSpeedTimer / GrapplePhysics.Length,
			          controller.character.transform.position);
		}

		if (!beamFiring && controller.grappled) {
			drawBeam(GrapplePhysics.anchor, controller.character.transform.position);
		}
	}

	public void fire() {

		Physics.Raycast(controller.cameraTransform.position, controller.cameraTransform.forward, out grappleHitInfo, maxDistance, grappleTo);
		if (grappleHitInfo.distance < minDistance) return; // Don't grapple if we're too close
		if (grappleHitInfo.collider && !grappleHitInfo.collider.gameObject.GetComponent<Rigidbody>()) {
			beamFiring = true;
			lightning.enabled = true;
			GrapplePhysics.anchor = grappleHitInfo.point;
			GrapplePhysics.Length = grappleHitInfo.distance;
			if (grappleFireSound) {
				grappleFireSound.pitch = Random.Range(1.2f, 1.8f);
				grappleFireSound.Play ();
			}
			if (grappleOnSound) {
				grappleOnSound.Play (); // audio source loops
			}
            ///NETWORKING///
            // for networking, we make the beam visible a bit earler than it actually becomes active.
            // this is so that, hopefully, by the time the other players can see it, the beam is now actually active.
            player.GrappleEndpoint = grappleHitInfo.point;
            player.GrappleVisible = true;
		}

	}

	public void drawBeam(Vector3 to, Vector3 from) {
		lightning.centerPoint = from;
		lightning.targetPoint = to;
		lightning.length = Vector3.Magnitude(to - from);
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
		if (grappleOnSound) {
			grappleOnSound.Stop (); // audio source loops
		}
		if (grappleDetachSound) {
			grappleDetachSound.pitch = Random.Range(1.2f, 1.8f);
			grappleDetachSound.Play ();
		}
		GrapplePhysics.reelMultiplier = 0;
        GrapplePhysics.sqrDistance = 0;
		lightning.enabled = false;
        player.GrappleVisible = false;
	}

}