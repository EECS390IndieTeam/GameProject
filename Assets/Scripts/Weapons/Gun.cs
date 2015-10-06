using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*Controls gunfire and grenade launch. To be hit by gun, target must have a Rigidbody component attached.*/
public class Gun : MonoBehaviour, IWeapon
{

    //  Variables for getters
    public float CooldownRate = 20.0f;
    public float CooldownDelay = 0.1f;
    public float MaxTemperature = 100f;

    public float HeatPerShot = 10.0f;
    public float DamagePerShot = 10.0f;
    public float OverheatedCooldownMultiplier = 1.5f;

    public bool IsOverheating {
        get;
        private set;
    }

    public bool Automatic = false;

    public float MinDelayBetweenShots = 0.1f;
    private float timeUntilNextShot = 0f;
    private float timeUntilCooldownBegins = 0f;

    public float Temperature {
        get;
        private set;
    }

    public int WeaponID = 0;

    // Variables for bullets
    private RaycastHit hitInfo;

    public Transform SourceTransform;

    void Start() {
        IsOverheating = false;
        Temperature = 0f;
        SourceTransform = GetComponentsInChildren<Transform>()[1];
    }

    void Update()
    {
        DebugHUD.setValue("Gun temp", Temperature);
        DebugHUD.setValue("Gun overheated", IsOverheating);
		if (timeUntilCooldownBegins > 0f){
			timeUntilCooldownBegins = Mathf.Max (0f, timeUntilCooldownBegins - Time.deltaTime);
		}
        if (timeUntilCooldownBegins <= 0f && Temperature >= 0f) {
            Temperature -= CooldownRate * Time.deltaTime * (IsOverheating ? OverheatedCooldownMultiplier : 1.0f);
            if (Temperature <= 0f) {
                Temperature = 0f;
                IsOverheating = false;
            }
        }
        if (timeUntilNextShot > 0f) {
            timeUntilNextShot = Mathf.Max(0f, timeUntilNextShot - Time.deltaTime);
            return;
        }
		if (IsOverheating) {
			return;
		}
        if (Automatic) {
            if (Input.GetButton("Fire1")) {
                Fire();
            }
        } else {
            if (Input.GetButtonDown("Fire1")) {
                Fire();
            }
        }

    }

    public void Fire()
    {
        timeUntilNextShot = MinDelayBetweenShots;
        timeUntilCooldownBegins = CooldownDelay;
        Temperature+=HeatPerShot;
        if (Temperature >= MaxTemperature) {
            IsOverheating = true;
            Temperature = MaxTemperature;
        }
        Vector3 endpoint;
        if (Physics.Raycast(SourceTransform.position, SourceTransform.forward, out hitInfo))
        {
            Debug.Log("Hit " + hitInfo.transform.name);
            endpoint = hitInfo.point;
            IPlayer hitplayer = hitInfo.transform.GetComponent<AbstractPlayer>();
            if (hitplayer == null) hitplayer = hitInfo.transform.GetComponentInParent<AbstractPlayer>();
			//Add in check for friendly fire here.
            if (hitplayer != null)
            {
                hitplayer.TakeDamage(DamagePerShot, hitplayer.Username, -SourceTransform.forward, WeaponID);
            }
        }
        else
        {
            endpoint = SourceTransform.position + SourceTransform.forward * 1000000.0f;
        }

        WeaponFireEvent evnt = WeaponFireEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.Unreliable);
        evnt.EndPoint = endpoint;
        evnt.StartPoint = SourceTransform.position;
        evnt.Color = Color.red;
        evnt.Send();

        Debug.DrawLine(SourceTransform.position, endpoint, Color.cyan, 0.5f);
    }

    //doing it this way allows these properties to be set in the editor
    float IWeapon.CooldownRate {
        get { return CooldownRate; }
    }

    public float EnergyPerShot {
        get { return HeatPerShot; }
    }

    float IWeapon.DamagePerShot {
        get { return DamagePerShot; }
    }

    float IWeapon.CooldownDelay {
        get { return CooldownDelay; }
    }

    int IWeapon.WeaponID {
        get { return WeaponID; }
    }

    float IWeapon.MaxTemperature {
        get { return MaxTemperature; }
    }

    float IWeapon.OverheatedCooldownMultiplier {
        get { return OverheatedCooldownMultiplier; }
    }
}