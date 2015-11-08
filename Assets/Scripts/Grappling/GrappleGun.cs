using UnityEngine;
using System.Collections;

public class GrappleGun : MonoBehaviour {

	[System.NonSerialized]
	public FPSController controller;

    public ParticleSystem hitEffect;
    private GameObject hitInstantiation;

	public float beamSpeed = 1000;
	private float beamSpeedTimer = 0;

	public float maxDistance = 200f;

	public float reelSpeed = 15f;
	public float stabilizer = 1.1f;
	public int maxReelMultiplier = 3;

	public RaycastHit grappleHitInfo;

	public LayerMask grappleTo;

    public Transform firingPoint;

	public AudioSource grappleFireSound;

	private Lightning lightning;

    private AbstractPlayer player;

	private bool beamFiring = false;

	void Start() {
		lightning = GetComponent<Lightning>();
		lightning.maxLength = maxDistance;
		beamSpeedTimer = 0;
		GrapplePhysics.reelSpeed = reelSpeed;
		GrapplePhysics.stabilizer = stabilizer;
        player = (AbstractPlayer)GameManager.instance.CurrentPlayer;
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
			drawBeam (firingPoint.position + (GrapplePhysics.anchor - firingPoint.position) * beamSpeedTimer / GrapplePhysics.Length,
			          firingPoint.position);
		}

		if (!beamFiring && controller.grappled) {
			drawBeam(GrapplePhysics.anchor, firingPoint.position);
		}
	}

	public void fire() {
		Physics.Raycast(firingPoint.position, controller.cameraTransform.forward, out grappleHitInfo, maxDistance, grappleTo);
		if (grappleHitInfo.collider && !grappleHitInfo.collider.gameObject.GetComponent<Rigidbody>()) {
			beamFiring = true;
			lightning.enabled = true;
			GrapplePhysics.anchor = grappleHitInfo.point;
			GrapplePhysics.Length = grappleHitInfo.distance;
			if (grappleFireSound) {
				grappleFireSound.Play ();
			}
            hitInstantiation = Instantiate(hitEffect.gameObject) as GameObject;
            hitInstantiation.transform.position = grappleHitInfo.point;
            if(hitInstantiation.GetComponent<ParticleSystem>() != null)
            {
                ParticleSystem p = hitInstantiation.GetComponent<ParticleSystem>();
                hitInstantiation.transform.up = grappleHitInfo.normal;
                p.Play();
            }
            Destroy(hitInstantiation, 5);
            
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
		GrapplePhysics.reelMultiplier = 0;

		lightning.enabled = false;
        player.GrappleVisible = false;
	}

}