using UnityEngine;
using System.Collections;

[RequireComponent(typeof(bouncySpinnyCubeScript), typeof(Collider))]
public class Flag : Bolt.EntityBehaviour<IFlagState> {
    public int Team {
        get {
            return state.Team;
        }
        private set {
            state.Team = value;
        }
    }
	public Renderer flagRenderer;
    public Light flagLight;
	public float timeDelay = 1.0f;
    private float timer = 0f;
    private bool delayFinished = false;

    private Collider c;

    void Awake() {
        c = GetComponent<Collider>();
        if (!BoltNetwork.isServer) {
            c.enabled = false;
            GetComponent<bouncySpinnyCubeScript>().enabled = false;
        }
		
    }

    public override void Attached() {
        state.Transform.SetTransforms(this.transform);
        if (!BoltNetwork.isServer) c.enabled = false;
        state.AddCallback("Team", new Bolt.PropertyCallbackSimple(() => {
            SetFlagColor(Teams.Colors[Team]);
            CaptureTheFlagMode mode = (CaptureTheFlagMode)GameManager.instance.GameMode;
            mode.SetFlagForTeam(Team, this);
        }));
    }

    void Update() {
        if (!BoltNetwork.isServer) return;
        if (delayFinished) return;
        if (timer >= timeDelay) {
            c.enabled = true;
            delayFinished = true;
        } else {
            timer += Time.deltaTime;
        }
    }
	
    public void ReturnFlag()
    {
        if (!BoltNetwork.isServer) return;
        CaptureTheFlagMode mode = (CaptureTheFlagMode)GameManager.instance.GameMode;
        mode.GetCapPointForTeam(Team).ReturnFlag();
        Destroy(this);
    }

	public void SetFlagColor(Color c){
		flagRenderer.materials [1].SetColor ("_EmissionColor", c);
		flagLight.color = c;
	}

    void OnTriggerEnter(Collider other)
    {
		if (!BoltNetwork.isServer) {
			return;
		}
        Debug.Log("Flag triggered");
        AbstractPlayer player = other.gameObject.GetComponentInParent<AbstractPlayer>();
        CaptureTheFlagMode mode = (CaptureTheFlagMode)GameManager.instance.GameMode;
        //You can only pick up this flag if
        // - you are a player
		// - you are not holding a flag
        // - there is not already someone picking up this flag
        // - either you are
        //    - on the enemy team, so you can pick it up wherever
        //    - or your a friendly player but the flag is not at spawn
        if(player != null && (!player.HoldingFlag) && (player.Team != Team || (player.Team == Team && !mode.isFlagAtBaseForTeam(Team))))
        {          
			Debug.Log ("Picked up flag");
			player.PickupFlag(Team);
            Destroy(this.gameObject);
        }
    }

    public static void SpawnFlag(int team, Vector3 position, Quaternion rotation) {
        if (!BoltNetwork.isServer) throw new System.Exception("Only the server can create flags!");
        var ent = BoltNetwork.Instantiate(BoltPrefabs.Flag, position, rotation);
        ent.GetComponent<Flag>().Team = team;
    }
}
