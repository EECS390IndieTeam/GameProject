using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*Controls gunfire and grenade launch. To be hit by gun, target must have a Rigidbody component attached.*/
public class Gun : MonoBehaviour, IWeapon
{

    //  Variables for getters
    private float cooldownRate = 20.0f;
    private float cooldownDelay = 0.3f;

    private float energyPerShot = 10.0f;
    private float damagePerShot = 10.0f;

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
        if (transform.GetComponentInParent<CharacterController>() != null)
        {
            controller = transform.GetComponentInParent<CharacterController>();
        }
        else
        {
            Debug.LogError("Gun must be a child of the player object.");
            controller = null;
        }
        playerCamera = transform.parent.GetComponentInChildren<Camera>().transform;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fire();
        }

    }

    public void Fire()
    {
        Vector3 endpoint;
        if (Physics.Raycast(transform.position, playerCamera.forward, out hitInfo, 10.0f))
        {
            endpoint = hitInfo.point;

            if (hitInfo.transform.GetComponent<Player>() != null)
            {
                hitInfo.transform.GetComponent<Player>().TakeDamage(damagePerShot, transform.parent.name);
            }
        }
        else
        {
            endpoint = transform.position + playerCamera.forward * 1000000.0f;
        }

        WeaponFireEvent evnt = WeaponFireEvent.Create(Bolt.GlobalTargets.Everyone);
        evnt.EndPoint = endpoint;
        evnt.StartPoint = transform.position;
        evnt.Color = Color.red;
        evnt.Send();

        Debug.DrawLine(transform.position, endpoint, Color.cyan, 0.5f);

        if (transform.GetComponentInParent<Cooldown>() != null) {
            
            Cooldown temp = transform.GetComponentInParent<Cooldown>();

            temp.cooldownDelay = cooldownDelay;
            temp.cooldownSpeed = cooldownRate;
            temp.degreesPerFire = energyPerShot;

            isOverheating = temp.overheat;

            temp.FireWeapon();
        }
    }
}