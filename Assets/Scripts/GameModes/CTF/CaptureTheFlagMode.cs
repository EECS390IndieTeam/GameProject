using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A game mode for a simple team deathmatch mode
/// </summary>
public class CaptureTheFlagMode : SimpleTeamGameMode {

    public int ScoreLimit = 3;

    //Hardcoded team limit here.
	private Flag[] flags = new Flag[3];
    private bool[] isFlagAtBase = new bool[3];

    public override GameModes Mode {
        get { return GameModes.CAPTURE_THE_FLAG; }
    }

    public override int MaxTeams {
        get { return 2; }
    }
    public override int MinPlayers {
        get { return 2; }
    }

    public override int MaxPlayers {
        get { return 16; }
    }

    public override string GameModeName {
        get { return "Capture the Flag"; }
    }
    public override string StatToDisplay {
        get { return "Flags"; }
    }

    public override void OnPreGame() {
        Lobby.AddStat("Kills");
        Lobby.AddStat("Flags");
        for(int i=0; i<isFlagAtBase.Length; i++)
        {
            isFlagAtBase[i] = true;
        }

    }

    public override bool GameOver() {
        for (int i = 0; i < 8; i++) {//iterate through all teams
            //we store the team's total score in their team's default pseudoplayer
            if (Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], "Flags") >= ScoreLimit) return true;
        }
        return false;
    }

    public override void OnGameStart() {
		Flag[] fgs = Object.FindObjectsOfType(typeof(Flag)) as Flag[];
		foreach (Flag f in fgs) {
			if(flags[f.teamID] == null){
				flags[f.teamID] = f;
				Debug.Log("Attaching flag for team: "+f.teamID);
				//BoltNetwork.Attach(f.gameObject);
			} else {
				Debug.LogError("Flag Collision for team number: "+f.teamID);
			}
		}
	}

    public override void OnGameEnd() {}

    public void setFlagAtBase(int flagNum, bool isAtBase)
    {
        isFlagAtBase[flagNum] = isAtBase;
    }

    public bool isFlagAtBaseForTeam(int teamNum)
    {
        return isFlagAtBase[teamNum];
    }

	public Vector3 GetFlagLocation(int teamNum){
		Flag f = flags [teamNum];
		if (f != null) {
			return f.gameObject.transform.position;
		} else {
			Debug.LogError("Could not find flag for team "+teamNum);
			return new Vector3();
		}
	}
}
