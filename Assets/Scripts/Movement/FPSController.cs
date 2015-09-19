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
        mouseLook = new CustomMouseLook(character, mouseSensitivityX, mouseSensitivityY);
        rotator = new CharacterRotator(character);
        sMovement = new SurfaceMovement(character, maxSurfaceSpeed, maxXSpeedSurface, maxXSpeedSurface, grabDistance);
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
        RaycastHit hit;
        if (Physics.SphereCast(character.position, .5f, character.transform.forward, out hit, grabDistance))
        {
            sMovement.attachToSurface(hit);
            isAttachedToSurface = true;
            grappleGun.detach();
            
        }


        Vector2 input = GetInput();
        if(isAttachedToSurface)
        {
            sMovement.moveCharacter(input);
            if(Input.GetKey(KeyCode.Space))
            {
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
