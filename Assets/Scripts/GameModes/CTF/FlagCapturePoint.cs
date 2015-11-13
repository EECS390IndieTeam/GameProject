using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FlagCapturePoint : MonoBehaviour {
    void Awake() {
        if (GameManager.instance.GameMode.Mode != GameModes.CAPTURE_THE_FLAG) Destroy(this.gameObject);
        ReturnFlag();
        if (!BoltNetwork.isServer) {
            GetComponent<Collider>().enabled = false;
        }
    }
    
    public int teamID; //Team who owns this capture point and can score points here

    public bool FlagAtBase {
        get;
        set;
    }

    public void ReturnFlag() {
        if (!BoltNetwork.isServer || FlagAtBase) return;
        Flag.SpawnFlag(teamID, transform.position, transform.rotation);
        FlagAtBase = true;
    }

    void OnTriggerEnter(Collider other) {
        if (!BoltNetwork.isServer) return;
        Debug.Log("Collided with a capture point.");
        CaptureTheFlagMode mode = GameManager.instance.GameMode as CaptureTheFlagMode;
        AbstractPlayer player = other.GetComponentInParent<AbstractPlayer>();
        if (player != null && player.HoldingFlag && player.Team == teamID) {//a player with a flag hit the capture point
            Debug.Log("A player carrying a flag triggered their capture point");
            if (player.HeldFlagTeam == teamID) {
                Debug.Log("Player returned flag");
                //the player is returning their team's flag
                FlagReturnedEvent evnt = FlagReturnedEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
                evnt.Team = teamID;
                evnt.Send();
                ReturnFlag();
            } else if (player.HeldFlagTeam != teamID && FlagAtBase) {
                //the player is capturing another team's flag
                FlagCapturedEvent evnt = FlagCapturedEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
                evnt.FlagTeam = player.HeldFlagTeam;
                evnt.Player = player.Username;
                evnt.Send();
                mode.GetCapPointForTeam(player.HeldFlagTeam).ReturnFlag();

                //update scores
                Lobby.IncrementStatForPlayer(evnt.Player, "Flags", 1);
                Lobby.IncrementStatForPlayer(Lobby.PP_TEAMS[teamID], "Flags", 1);
            }
        }
        //we don't care about players who are not carrying flags, the flag itself will take care of that case
    }
}
