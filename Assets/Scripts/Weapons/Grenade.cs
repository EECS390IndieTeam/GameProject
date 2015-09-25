using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour, IGrenade 
{
	private int bodyTimer = 0;			//Private counter to use for time-based effects.
	private float speed = 3;			//Speed at which the projectile moves.

	//Variables that are also used by IGrenade
	private float detonateTime = 5.0f;	//Time until grenade detonates, in Seconds. TODO: Set within the ThrowGrenade class.
	public float explosionRadius = 5.0f;
	public float damage = 20.0f;
	public string thrower = "N/A";

	// Use this for initialization
	void Start () {
		// Provide velocity based on Rotation.
		//Y-rotation: Horizontal, positive = clockwise.
		//X-rotation: Up/down, positive = down.
		//thus, X = Cos (Y-rotation) times speed
		//Y = Sin (Y-rotation) times speed
		//Z = -Sin (X-rotation) times speed
		float zVel = Mathf.Cos(this.transform.eulerAngles.y * Mathf.Deg2Rad) * speed;
		float xVel = Mathf.Sin(this.transform.eulerAngles.y * Mathf.Deg2Rad) * speed;
		float yVel = 0 - (Mathf.Sin(this.transform.eulerAngles.x * Mathf.Deg2Rad) * speed);
		this.GetComponent<Rigidbody> ().velocity = new Vector3 (xVel, yVel, zVel);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//FixedUpdate called at a fixed framerate of 60
	void FixedUpdate() {
		//Enable body collision after half a second. Shorten timing once proper model created and can test minimum time needed.
		if (bodyTimer == 30) {
			this.GetComponent<SphereCollider> ().enabled = true;
		}
		//Self-destroy object after 10 seconds.
		//TODO: implement an array or something to allow recycling of the grenade projectiles.
		if (bodyTimer == (int)(detonateTime * 60)) {
			//Put code to detonate and harm nearby players here.
			Detonate();
		}
		bodyTimer++;
	}

	public void Detonate(){
		Collider[] targets = Physics.OverlapSphere(this.transform.position, explosionRadius);
		int i = 0;
		while (i < targets.Length) {
			//If something in the radius is a player, deal damage to them.
			if(targets[i].gameObject.tag == "Player"){
				GameObject target = targets[i].gameObject;
				IPlayer hitplayer = target.GetComponent<AbstractPlayer>();
				hitplayer.TakeDamage(damage, hitplayer.Username, (target.transform.position - this.transform.position));
			}
			i++;
		}
		//--------Put in something here to create an explosion particle effect------------
		//Once the whole array has been checked through, delete the grenade.
		Destroy (this.gameObject);
	}

	public void setDetonate(float time){
		detonateTime = time;
	}

	//doing it this way allows these properties to be set in the editor
	float IGrenade.Strength {
		get { return damage; }
	}
	
	float IGrenade.Radius {
		get { return explosionRadius; }
	}
	
	float IGrenade.FuseTime {
		get { return detonateTime; }
	}
	
	string IGrenade.Thrower {
		get { return thrower; }
	}
}
