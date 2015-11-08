using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FlagCapturePoint : MonoBehaviour {
    void Awake() {
        if (!BoltNetwork.isServer) {
			enabled = false;
		} else {
			if(teamID == 1) BoltNetwork.Instantiate(BoltPrefabs.Flag1,transform.position, Quaternion.identity);
			if(teamID == 2) BoltNetwork.Instantiate(BoltPrefabs.Flag2,transform.position, Quaternion.identity);

		}
    }
    
    public int teamID; //Team who owns this capture point and can score points here

    void OnTriggerEnter(Collider other)
    {
        if (!BoltNetwork.isServer) return;
		Debug.Log ("Collided with a capture point.");
        IGameMode currentGameMode = GameManager.instance.GameMode;
        if(GameManager.instance.GameMode.Mode == GameModes.CAPTURE_THE_FLAG)
        {
            CaptureTheFlagMode mode = (CaptureTheFlagMode)currentGameMode;
            Flag f = other.gameObject.GetComponentInParent<Flag>();
			Debug.Log ("Getting flag.");
            if (f != null)
            {
				if(f.player != null && f.player.Team == teamID)
				{
					
					if(f.teamID == teamID)
                    {
						Debug.Log ("Returning a flag");
                        //We are returning the flag to our base
                        mode.setFlagAtBase(teamID,true);
                        f.ReturnFlag();
                    } else
                    {
						Debug.Log ("Trying to capture a Flag.");
                        //The flag we are returning is not ours. Check and see if ours is returned and if so, you score!
                        if (mode.isFlagAtBaseForTeam(teamID))
                        {
							Debug.Log ("Flag Captured!!");
                            //update scores
                            Lobby.IncrementStatForPlayer(f.player.Username, "Flags", 1);
                            Lobby.IncrementStatForPlayer(Lobby.PP_TEAMS[f.player.Team], "Flags", 1);
                            f.ReturnFlag();
                        }
                    }
                }
            }
        }
    }
}
