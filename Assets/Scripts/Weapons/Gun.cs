using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*Controls gunfire and grenade launch. To be hit by gun, target must have a Rigidbody component attached.*/
public class Gun : MonoBehaviour {

    // Variables for bullets
    private RaycastHit hitInfo;
    private RaycastHit resetTo = new RaycastHit();
    private CharacterController controller;
    private Transform playerCamera;
        
    void Start()
    {
        if(transform.GetComponentInParent<CharacterController>() != null)
        {
            controller = transform.GetComponentInParent<CharacterController>();
        }
        else
        {
            Debug.LogError("Gun must be a child of the player object.");
        }
        playerCamera = GameObject.Find("Main Camera").transform;
    }
	void Update () {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.C))
        {
            Fire();
        }

	}
	
	private void Fire () {
        Physics.Raycast(transform.position, playerCamera.forward, out hitInfo, 10.0f);
        if(hitInfo.point != resetTo.point)
        {
            Debug.Log("Object hit at: " + hitInfo.point);
        }
        else
        {
            Debug.Log("Nothing hit");
        }
        // Resets hitInfo
        hitInfo = resetTo;
    }
    
}
