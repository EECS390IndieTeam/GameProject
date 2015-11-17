using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {

    public float mouseSensitivityX = 10;
    public float mouseSensitivityY = 10;
    public float grabDistance = 0.5f;
    public float maxSurfaceSpeed = 24;
    public float maxXSpeedSurface = 8;
    public float maxYSpeedSurface = 8;
    public float velocityQuantum;
	public float rotationSpeed = 1;
	public LayerMask surfaceMovementMask;

    public Transform cameraTransform;

    private CustomMouseLook mouseLook;
    private CharacterRotator rotator;
    private SurfaceMovement sMovement;
    public GrappleGun grappleGun;
	public Gun gun;
	public ThrowGrenade grenade;
	public MeshRenderer gunModel;

    public bool isAttachedToSurface = false;
	private bool prepGrenade;
	
	private float debounceTime = 0.1f;
	private float debounce = 0;

	[System.NonSerialized]
	public Rigidbody character;

	[System.NonSerialized]
	public bool grappled = false;

	private bool previousHolding;

	private OwnerPlayer player;

    private bool debugMouseFree = false;

	// Use this for initialization
	void Awake ()
    {
		
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		character = GetComponent<Rigidbody>();
        mouseLook = GetComponent<CustomMouseLook>();
        rotator = GetComponent<CharacterRotator>();
        sMovement = GetComponent<SurfaceMovement>();
        


		if (grappleGun) grappleGun.controller = this;
		grappled = false;
		previousHolding = false;
	}

    void Start() {
        player = (OwnerPlayer)GameManager.instance.CurrentPlayer;
    }
	
	// Update is called once per frame
	void Update () {

        CheckForInput ();

		if (previousHolding != player.HoldingFlag) {
			previousHolding = player.HoldingFlag;
			if (previousHolding) {
				DisableGun();
			} else {
				EnableGun();
			}
		}
    }

	private void CheckForInput() {
		DebugHUD.setValue("gunModel.enabled", gunModel.enabled);

        // Mouse look and character rotation
        if (!debugMouseFree) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            if (Mathf.Abs(mouseX) > float.Epsilon || Mathf.Abs(mouseY) > float.Epsilon) {
                mouseLook.rotateView(mouseX, mouseY);
            }
        }
		if(Input.GetKey(KeyCode.Q))
		{
			rotator.rotateCharacter(rotationSpeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.E))
		{
			rotator.rotateCharacter(-rotationSpeed * Time.deltaTime);
		}

        //debugMouseFree
        if (Input.GetKeyDown(KeyCode.Backslash)) {
            debugMouseFree = !debugMouseFree;
            Cursor.lockState = debugMouseFree ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = debugMouseFree;
        }
		
		// -------------------------------------------------------------------------- //

        // Firing weapon / dropping flag
        if (player.HoldingFlag) {
            if (Input.GetButtonDown("Fire1")) {
                //I might change this later so that the OwnerPlayer can drop the flag immediately without having to go through Bolt first
                Debug.Log("Dropping flag");
                FlagDroppedEvent evnt = FlagDroppedEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
                evnt.FlagTeam = player.HeldFlagTeam;
                evnt.Carrier = player.Username;
                evnt.Send();
            }
        } else {
            
            if (Input.GetButtonDown("Fire1")) {
                gun.StartShot();
            }
            if (Input.GetButtonUp("Fire1")) {
                gun.EndShot();
            }
        }

		// -------------------------------------------------------------------------- //

		// Grapple/Degrapple
		debounce -= Time.deltaTime;
		if (debounce < 0) {
			debounce = 0;
		}
		if (Input.GetButtonDown("Fire2") && !(grappled || grappleGun.beamFiring) && debounce == 0) {
			grappleGun.fire();
			if (isAttachedToSurface) {
				isAttachedToSurface = false;
			}
			
			debounce = debounceTime;
		}
		if (Input.GetButtonDown("Fire2") && (grappled || grappleGun.beamFiring) && debounce == 0) {
			grappleGun.detach();
			debounce = debounceTime;
		}

		// -------------------------------------------------------------------------- //

		// Reel in or push off
		if (Input.GetButtonDown("Jump")){
			if (grappled) {
				grappleGun.reelIn();
			} else if (isAttachedToSurface) {
				isAttachedToSurface = false;
				sMovement.pushOff();
			}
		}

		// -------------------------------------------------------------------------- //

		// Grenades
        if (!player.HoldingFlag && Input.GetButtonDown("Grenade") && !previousHolding) {
			bool success = grenade.PrepGrenade();
			if (success) {
				DisableGun ();
			}
			prepGrenade = true;
		}
		if (Input.GetButtonUp("Grenade") && !previousHolding) {
			grenade.GrenadeThrow();
			if (!gunModel.enabled && prepGrenade) {
				EnableGun();
				prepGrenade = false;
			}
		}
	}

    void FixedUpdate ()
    {
		if (grappled) {
			character.velocity = GrapplePhysics.calculateVelocity(character.position, character.velocity);
		}
    }

	public void EnableGun() {
		gun.canShoot = true;
		gunModel.enabled = true;
	}

	public void DisableGun() {
		gun.canShoot = false;
		gunModel.enabled = false;
	}

    void OnCollisionStay(Collision c)
    {
        if(c.collider.gameObject.layer == 9 && c.contacts.Length > 0 && (Input.GetKey(KeyCode.LeftShift) || character.velocity.magnitude < velocityQuantum) && !isAttachedToSurface)
        {

            if (grappled)
            {
                grappleGun.detach();
            }
            sMovement.attachToSurface(c);
            isAttachedToSurface = true;
        }
    }
}
