using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A game mode for a simple team deathmatch mode
/// </summary>
public class CaptureTheFlagMode : SimpleTeamGameMode {

    public CaptureTheFlagMode() {
        ScoreLimit = 3; //3 points to win
        TimeLimit = 5f * 60f; //5 minutes
        //TimeLimit = 3f; //3 seconds DEBUG
        RespawnDelay = 3f; //3 second respawn delay
    }

    public override int ScoreLimit {
        get;
        set;
    }

    public override float TimeLimit {
        get;
        set;
    }

    public override float RespawnDelay {
        get;
        set;
    }

    //Hardcoded team limit here.
    private FlagCapturePoint[] capPoints = new FlagCapturePoint[2];
	private Flag[] flags = new Flag[2];

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
        Lobby.AddStat("Deaths");
        Lobby.AddStat("Flags");
    }

    public override void OnGameStart() {
        FlagCapturePoint[] points = Object.FindObjectsOfType<FlagCapturePoint>();
        foreach (FlagCapturePoint pt in points) {
            if (GetCapPointForTeam(pt.teamID) == null) {
                SetCapPointForTeam(pt.teamID, pt);
                Debug.Log("Cap Point registered for team " + pt.teamID);
            } else {
                Debug.LogError("Duplicate Cap Point found for team " + pt.teamID);
            }
        }
	}

    public override void OnGameEnd() {}

    public bool isFlagAtBaseForTeam(int teamNum)
    {
        return GetCapPointForTeam(teamNum).FlagAtBase;
    }

    public FlagCapturePoint GetCapPointForTeam(int team) {
        return capPoints[team-1];
    }

    private void SetCapPointForTeam(int team, FlagCapturePoint point) {
        capPoints[team - 1] = point;
	}
	
	public Flag GetFlagForTeam(int team) {
		return flags[team - 1];
	}

	public void SetFlagForTeam(int team, Flag flag) {
		flags[team - 1] = flag;
	}
}
