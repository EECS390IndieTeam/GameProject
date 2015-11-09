using UnityEngine;
using System.Collections;

public class Flag : Bolt.EntityBehaviour<IFlagState> {
    public bool isEnabled {
        get {
            return state.isEnabled;
        }
        set {
            state.isEnabled = value;
        }
    }

    public int teamID; //The team that this flag belongs to
    public AbstractPlayer player; //The player holding this flag. Null if the flag is dropped.
	public MeshRenderer flagMaterial;
    private Vector3 flagSpawnPosition;
	private Quaternion flagSpawnRotation;
	public float timeDelay = 1.0f;

	private Light light;

	public bouncySpinnyCubeScript bSCS;

    Collider c;

    public override void Attached() {
        Debug.Log("Setting up a flag.");
        Debug.Log(this.gameObject);
        state.Transform.SetTransforms(this.transform);
        isEnabled = true;
    }

	// Use this for initialization
	void Start () {
		c = GetComponent<Collider>();
		light = GetComponentInChildren<Light>();
        flagSpawnPosition = transform.position;
		flagSpawnRotation = transform.rotation;
		SetFlagColor (Lobby.teamColors [teamID]);
		bSCS.enabled = true;
	}
	
    public void ReturnFlag()
    {
        if (!BoltNetwork.isServer) return;
        DropFlag();
        this.transform.position = flagSpawnPosition;
        this.transform.rotation = flagSpawnRotation;
    }

    

	public void DropFlag(){

        if (BoltNetwork.isServer) {
            StartCoroutine("DropFlagRoutine");
        } else {
            FlagDroppedEvent evnt = FlagDroppedEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.Flag = entity;
            evnt.Send();
        }
	}

	public void SetFlagColor(Color c){
		flagMaterial.materials [1].SetColor ("_EmissionColor", c * 2);
		light.color = c;
	}

    IEnumerator DropFlagRoutine()
    {
		transform.position += 2 * player.transform.forward;
		player.HoldingFlag = false;
		player = null;
        this.transform.parent = null;
        state.Holder = "";
		yield return new WaitForSeconds(timeDelay);
		Debug.Log ("Re-enabling flag");
		bSCS.enabled = true;
        isEnabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
		if (!isEnabled || !BoltNetwork.isServer) {
			return;
		}
        AbstractPlayer p = other.gameObject.GetComponentInParent<AbstractPlayer>();
        CaptureTheFlagMode mode = (CaptureTheFlagMode)GameManager.instance.GameMode;
        //You can only pick up this flag if
        // - you are a player
        // - there is not already someone picking up this flag
        // - either you are
        //    - on the enemy team, so you can pick it up wherever
        //    - or your a friendly player but the flag is not at spawn
        if(p != null && player == null && (p.Team != teamID || (p.Team == teamID && !mode.isFlagAtBaseForTeam(teamID))))
        {
            //Update who is holding flag
			OwnerPlayer p1 = other.gameObject.GetComponentInParent<OwnerPlayer>();
			if(p1 == null){
				Debug.Log ("Player doesn't exist.");
			} else {
				Debug.Log ("Picked up flag");
				Vector3 pos = p1.HandPoint.position;
				//this.transform.position = pos - Vector3.Scale(other.transform.up,new Vector3(1.0f,0.5f,1.0f));
				//this.transform.rotation = p1.HandPoint.rotation;

				this.gameObject.transform.parent = p1.HandPoint;
				transform.localPosition = 0.31f * Vector3.up;
				transform.localRotation = Quaternion.identity;
				player = p;
				p.HoldingFlag = true;
				state.Holder = player.Username;
				bSCS.enabled = false;
				isEnabled = false;
			}
        }
    }

	void Update(){
		if (entity.isAttached && state.Holder == GameManager.instance.CurrentUserName) {
			if(Input.GetButtonDown("Fire1")){
				DropFlag();
			}
		}
	}
}
