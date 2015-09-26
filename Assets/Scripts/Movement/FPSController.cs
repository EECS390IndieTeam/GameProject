using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {

    public float mouseSensitivityX = 10;
    public float mouseSensitivityY = 10;
    public float grabDistance = 0.5f;
    public float maxSurfaceSpeed = 24;
    public float maxXSpeedSurface = 8;
    public float maxYSpeedSurface = 8;
	public float rotationSpeed = 1;

    public Transform cameraTransform;

    private CustomMouseLook mouseLook;
    private CharacterRotator rotator;
    private SurfaceMovement sMovement;
    private GrappleGun grappleGun;
    private bool isAttachedToSurface = false;
	
	private float debounceTime = 0.1f;
	private float debounce = 0;

	[System.NonSerialized]
	public Rigidbody character;

	[System.NonSerialized]
	public bool grappled = false;


	// Use this for initialization
	void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
		character = GetComponent<Rigidbody>();
        mouseLook = GetComponent<CustomMouseLook>();
        rotator = GetComponent<CharacterRotator>();
        sMovement = GetComponent<SurfaceMovement>();
        grappleGun = GetComponent<GrappleGun>();
		if (grappleGun) grappleGun.controller = this;
	}
	
	// Update is called once per frame
	void Update () {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mouseX) > float.Epsilon || Mathf.Abs(mouseY) > float.Epsilon)
        {
            mouseLook.rotateView(mouseX, mouseY);
        }
        if(Input.GetKey(KeyCode.Q))
        {
            rotator.rotateCharacter(rotationSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotator.rotateCharacter(-rotationSpeed);
        }

		if (!grappleGun) return;

		debounce -= Time.deltaTime;
		if (debounce < 0) {
			debounce = 0;
		}
		
		if (Input.GetButtonDown("Fire1") && !grappled && debounce == 0) {
			grappleGun.fire();
            if (isAttachedToSurface) {
                Debug.Log("Detached from surface");
                isAttachedToSurface = false;
            }
            
			debounce = debounceTime;
		}
		
		if (Input.GetButtonDown("Fire1") && grappled && debounce == 0) {
			grappleGun.detach();
			debounce = debounceTime;
		}

		if (Input.GetButtonDown("Jump") && grappled) {
			grappleGun.reelIn();
		}
    }

    void FixedUpdate ()
    {
        //Debug.Log(isAttachedToSurface);
        RaycastHit hit;
        if (Physics.Raycast(character.position, cameraTransform.forward, out hit, grabDistance) && !isAttachedToSurface)
        {
            grappleGun.detach();
            Debug.Log("Attached to surface");
            sMovement.attachToSurface(hit);
            isAttachedToSurface = true;
            
            
        }


        Vector2 input = GetInput();
        if(isAttachedToSurface)
        {
            sMovement.moveCharacter(input);
            if(Input.GetKey(KeyCode.Space))
            {
                Debug.Log("Detached from surface");
                isAttachedToSurface = false;
            }
        }

		doPhysics();
    }

	private void doPhysics() {
		if (grappled) {
			character.velocity = GrapplePhysics.calculateVelocity(character.position, character.velocity);
		}
	}

    private Vector2 GetInput()
    {

        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };
        
        return input;
    }
}
