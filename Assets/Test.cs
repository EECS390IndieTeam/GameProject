using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	float speed = 6.0F;
	float  jumpSpeed = 8.0F;
	float  gravity = 20.0F;
	int health = 10;
	UnityEngine.UI.Text health_text;

	private Vector3 moveDirection = Vector3.zero;

	// Use this for initialization
	void Start () {
		RectTransform[] HUD_elements = GetComponentsInChildren<RectTransform>();
		foreach (RectTransform r in HUD_elements){
			if (r.name == "Health"){
				health_text = r.GetComponent<UnityEngine.UI.Text> ();
			}
		}

		health_text.text = health.ToString();
		//Component[] temp = health_text.GetComponents<Component>();
		//foreach (Component t in temp) {
		//	print(t.GetType());
		//	if t.GetType() == UnityEngine.UI.
		//}
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller;
		controller = (CharacterController)GetComponent("CharacterController");
		if (controller.isGrounded) {
			// We are grounded, so recalculate
			// move direction directly from axes
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0,
			                        Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			
			if (Input.GetButton ("Jump")) {
				moveDirection.y = jumpSpeed;
			}
		}
		
		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller
		controller.Move(moveDirection * Time.deltaTime);
	}
}
