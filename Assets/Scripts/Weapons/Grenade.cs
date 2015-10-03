using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grenade : Bolt.EntityBehaviour<IGrenadeState>, IGrenade 
{
	//Properties of this specific Grenade instance
    public float StartingFuseTime = 5.0f;
	public float explosionRadius = 5.0f;
	public float damage = 20.0f;

    public override void Attached() {
        state.Transform.SetTransforms(transform);
        if (!entity.isOwner) {
            GetComponent<Rigidbody>().isKinematic = true;
        } else {
            DetonateTime = BoltNetwork.serverTime + StartingFuseTime;
        }
    }

	//FixedUpdate called at a fixed framerate of 60
    //SimulateOwner is called from FixedUpdate, but only for the owner of the object
	public override void SimulateOwner() {
        float timeSinceThrown = StartingFuseTime - FuseTimeRemaining;
        DebugHUD.setValue("DetonateTime", DetonateTime);
        DebugHUD.setValue("ServerTime", BoltNetwork.serverTime);
        DebugHUD.setValue("FuseTimeRemaining", FuseTimeRemaining);

		//Self-destroy object after its fuse is done.
		if (FuseTimeRemaining <= 0f) {
			Detonate();
		}
	}

	void OnCollisionEnter(Collision struck){
		GameObject struckObject = struck.gameObject;
		IPlayer hitplayer = struckObject.GetComponent<AbstractPlayer>();
		if(hitplayer != null){
			//Attach the grenade to the hit player.
			this.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.GetComponent<Rigidbody>().isKinematic = true;
			this.GetComponent<Rigidbody>().detectCollisions = false;
			this.GetComponent<Collider>().enabled = false;
			this.transform.parent = struckObject.transform;
		}
	}

	public void Detonate(){
		Collider[] targets = Physics.OverlapSphere(this.transform.position, explosionRadius);
		int i = 0;
		while (i < targets.Length) {
			//If something in the radius is a player, deal damage to them.
			GameObject target = targets[i].gameObject;
			IPlayer hitplayer = target.GetComponent<AbstractPlayer>();
			if(hitplayer != null) hitplayer.TakeDamage(damage, Thrower, (target.transform.position - this.transform.position));
			i++;
		}
		//--------Put in something here to create an explosion particle effect------------
        //OK, I will, here goes:
        ExplosionEvent evnt = ExplosionEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.Unreliable);
        evnt.Position = transform.position;
        evnt.Send();
        //done

		//Once the whole array has been checked through, delete the grenade.
		BoltNetwork.Destroy (this.gameObject);
	}

	//doing it this way allows these properties to be set in the editor
	public float Strength {
		get { return damage; }
	}
	
	public float Radius {
		get { return explosionRadius; }
	}
	
	public float FuseTimeRemaining {
		get { return DetonateTime - BoltNetwork.serverTime; }
	}

    public float FuseTime {
        get { return StartingFuseTime; }
    }
	
	public string Thrower {
        get { return state.Thrower; }
	}

    public void SetThrower(string t) {
        state.Thrower = t;
    }

    public float DetonateTime {
        get { return state.DetonateTime; }
        set { state.DetonateTime = value; }
    }
}
