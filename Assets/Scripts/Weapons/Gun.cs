using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*Controls gunfire and grenade launch.*/
public class Gun : MonoBehaviour, IWeapon
{

	public LayerMask shootableLayers;

    public ParticleSystem muzzleFlash;

    //  Variables for getters
    public float CooldownRate = 20.0f;
    public float CooldownDelay = 0.1f;
    public float MaxTemperature = 100f;

    public float HeatPerShot = 3.0f;
    public float DamagePerShot = 10.0f;
    public float OverheatedCooldownMultiplier = 1.5f;

	[System.NonSerialized]
	public bool canShoot;

    public bool IsOverheating {
        get;
        private set;
    }

    public bool Automatic = false;

    public float MinDelayBetweenShots = 0.1f;
    //private float timeUntilNextShot = 0f;
    private float timeUntilCooldownBegins = 0f;

    public float Temperature {
        get;
        private set;
    }

    public int WeaponID = 0;

    // Variables for bullets
    private RaycastHit hitInfo;

    public Transform SourceTransform;
    public Transform GunShotStartTransform;
	
    private bool assisting = false;
    private Vector3 radius;
    private CustomMouseLook look;
    private float assistedSpeedMultiplier = 0.5f;
    private float marginOfError = 2.0f;
    
	private Vector3 endpoint;

	private bool currentlyFiring;

	private AbstractPlayer player;

    void Start() {
        IsOverheating = false;
        Temperature = 0f;
        look = GetComponentInParent<CustomMouseLook>();
		player = (AbstractPlayer)GameManager.instance.CurrentPlayer;
		canShoot = true;
    }

    void Update()
    {
		DebugHUD.setValue("Currently Firing", currentlyFiring);
		DebugHUD.setValue ("Can shoot", canShoot);

        if (Physics.Raycast(SourceTransform.position, SourceTransform.forward, out hitInfo, float.PositiveInfinity, shootableLayers))
        {
            IPlayer hitplayer = hitInfo.transform.GetComponent<AbstractPlayer>();
            if(hitplayer != null)
            {
                if (!assisting)
                {
                    AimAssist(hitplayer);
                }
            }
            else
            {
                if (assisting)
                {
                    ResetAimAssist();
                }
            }
        }

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
//        if (timeUntilNextShot > 0f) {
//            timeUntilNextShot = Mathf.Max(0f, timeUntilNextShot - Time.deltaTime);
//            return;
//        }
		if (IsOverheating) {
			player.LaserVisible = false;
			return;
		}

		if (currentlyFiring) {
			player.LaserVisible = true;
			Firing();
		} else {
			player.LaserVisible = false;
		}
    }

	public void StartShot() {
		currentlyFiring = canShoot;
	}

	public void EndShot() {
		currentlyFiring = false;
	}

	public void CreateShot() {
		WeaponFireEvent evnt = WeaponFireEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.Unreliable);
		evnt.EndPoint = endpoint;
		evnt.StartPoint = GunShotStartTransform.position;
		evnt.Color = Color.red;
		evnt.Send();
	}

	public bool RefreshRaycast() {
		if (Physics.Raycast(SourceTransform.position, SourceTransform.forward, out hitInfo, float.PositiveInfinity, shootableLayers))
		{
			endpoint = hitInfo.point;
			return true;
		}
		else
		{
			endpoint = SourceTransform.position + SourceTransform.forward * 1000000.0f;
			return false;
		}
	}

	public IPlayer GetTarget() {
			IPlayer hitplayer = hitInfo.transform.GetComponent<AbstractPlayer>();
			if (hitplayer == null) hitplayer = hitInfo.transform.GetComponentInParent<AbstractPlayer>();
			return hitplayer;
	}

    public void Firing()
	{
		Debug.Log ("Hey I should start shooting now");
        if(muzzleFlash != null)
        {
            //StartCoroutine(MuzzleFlash());
			muzzleFlash.Play();

        }

        timeUntilCooldownBegins = CooldownDelay;
        Temperature += HeatPerShot * Time.deltaTime;
        if (Temperature >= MaxTemperature) {
            IsOverheating = true;
            Temperature = MaxTemperature;
			player.LaserVisible = false;
			currentlyFiring = false;
        } else {
			if (RefreshRaycast()) {
				IPlayer target = GetTarget();
				
				//Add in check for friendly fire here.
				if (target != null && target.Team != player.Team)
				{
					target.TakeDamage(DamagePerShot, target.Username, -SourceTransform.forward, WeaponID);
				}
			}
			player.MuzzlePoint = GunShotStartTransform.position;
			player.LaserEndpoint = endpoint;
			player.LaserVisible = true;
		}
    }

    private void AimAssist(IPlayer hitplayer)
    {
       if(hitplayer != null)
       {
           look.sensitivityX *= assistedSpeedMultiplier;
           look.sensitivityY *= assistedSpeedMultiplier;
       }
        assisting = true;
    }

    private void ResetAimAssist()
    {
        look.sensitivityX = look.sensitivityX / assistedSpeedMultiplier;
        look.sensitivityY = look.sensitivityY / assistedSpeedMultiplier;
        assisting = false;
    }

    private IEnumerator MuzzleFlash()
    {
        if (muzzleFlash.isPlaying)
        {
            muzzleFlash.Stop();
        }
        muzzleFlash.startSize = 0.25f;
        muzzleFlash.Play();
        yield return new WaitForEndOfFrame();
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