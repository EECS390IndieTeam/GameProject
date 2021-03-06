﻿using UnityEngine;
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
    private Vector3 flagSpawnPosition;
	private Quaternion flagSpawnRotation;
	private float timeDelay = 3.0f;
    Collider c;

    public override void Attached() {
        Debug.Log("Setting up a flag.");
        Debug.Log(this.gameObject);
        state.Transform.SetTransforms(this.transform);
        isEnabled = true;
    }

	// Use this for initialization
	void Start () {
        flagSpawnPosition = transform.position;
		flagSpawnRotation = transform.rotation;
        c = GetComponent<Collider>();
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

    IEnumerator DropFlagRoutine()
    {
		transform.position += 2 * player.transform.forward;
		player = null;
        this.transform.parent = null;
        state.Holder = "";
		yield return new WaitForSeconds(timeDelay);
		Debug.Log ("Re-enabling flag");
        isEnabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
		if (!isEnabled || !BoltNetwork.isServer) {
			return;
		}
        AbstractPlayer p = other.gameObject.GetComponentInParent<AbstractPlayer>();
        CaptureTheFlagMode mode = (CaptureTheFlagMode)GameManager.instance.gameMode;
        //You can only pick up this flag if
        // - you are a player
        // - there is not already someone picking up this flag
        // - either you are
        //    - on the enemy team, so you can pick it up wherever
        //    - or your a friendly player but the flag is not at spawn
        if(p != null && player == null && (p.Team != teamID || (p.Team == teamID && !mode.isFlagAtBaseForTeam(teamID))))
        {
            //Update who is holding flag
            this.gameObject.transform.parent = other.gameObject.transform;
            player = p;
            state.Holder = player.Username;
            isEnabled = false;
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
