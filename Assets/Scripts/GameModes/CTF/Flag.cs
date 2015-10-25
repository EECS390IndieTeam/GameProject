using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

    public int teamID; //The team that this flag belongs to
    public IPlayer player; //The player holding this flag. Null if the flag is dropped.
    private Vector3 flagSpawnPosition;
	private Quaternion flagSpawnRotation;
    Collider c;

	// Use this for initialization
	void Start () {
        flagSpawnPosition = transform.position;
		flagSpawnRotation = transform.rotation;
        c = GetComponent<Collider>();
	}
	
    public void ReturnFlag()
    {
        DropFlag();
        this.transform.position = flagSpawnPosition;
        this.transform.rotation = flagSpawnRotation;
        c.isTrigger = true;
    }

    public void DropFlag()
    {
        player = null;
        this.transform.parent = null;
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        IPlayer p = other.gameObject.GetComponentInParent<IPlayer>();
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
            c.isTrigger = false;
        }
    }
}
