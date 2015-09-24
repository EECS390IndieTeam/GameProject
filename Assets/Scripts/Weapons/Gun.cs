using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*Controls gunfire and grenade launch. To be hit by gun, target must have a Rigidbody component attached.*/
public class Gun : MonoBehaviour, IWeapon {

    //  Variables for getters
    private float cooldownRate = 1.0f;
    private float cooldownDelay = 1.0f;

    private float energyPerShot = 1.0f;
    private float damagePerShot = 1.0f;

    private bool isOverheating = false;
    private int weaponID = 0;

    // Variables for bullets
    private RaycastHit hitInfo;
    private RaycastHit resetTo = new RaycastHit();
    private CharacterController controller;
    private Transform playerCamera;

    // Public weapon getters
    // Everything has set ability. Future implementation: weapon factory for producing different weapon types. Appropriate cooldown/damage
    // values will be looked up and set before gun is spawned.
    public float CooldownRate
    {
        get
        {
            return cooldownRate;
        }
        set
        {
            cooldownRate = value;
        }
    }
    public float CooldownDelay
    {
        get
        {
            return cooldownDelay;
        }
        set
        {
            cooldownDelay = value;
        }
    }

    public float EnergyPerShot
    {
        get
        {
            return energyPerShot;
        }
        set
        {
            energyPerShot = value;
        }
    }
    public float DamagePerShot
    {
        get
        {
            return damagePerShot;
        }
        set
        {
            damagePerShot = value;
        }
    }

    public bool IsOverheating
    {
        get
        {
            return isOverheating;
        }

        set
        {
            isOverheating = value;
        }
    }

    public int WeaponID
    {
        get
        {
            return weaponID;
        }

        set
        {
            weaponID = value;
        }
    }

    void Start()
    {
        if(transform.GetComponentInParent<CharacterController>() != null)
        {
            controller = transform.GetComponentInParent<CharacterController>();
        }
        else
        {
            Debug.LogError("Gun must be a child of the player object.");
            controller = null;
        }
        playerCamera = GameObject.Find("Main Camera").transform;
    }
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fire();
        }

	}
	
	public void Fire () {
        Physics.Raycast(transform.position, playerCamera.forward, out hitInfo, 10.0f);
        Debug.DrawRay(transform.position, playerCamera.forward * 10.0f, Color.green, 3.0f);
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
