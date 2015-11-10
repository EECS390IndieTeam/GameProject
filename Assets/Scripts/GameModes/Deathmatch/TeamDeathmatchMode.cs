using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A game mode for a simple team deathmatch mode
/// </summary>
public class TeamDeathmatchMode : SimpleTeamGameMode {

    public TeamDeathmatchMode() {
        this.ScoreLimit = 25;
        this.TimeLimit = 5f * 60f;
        this.RespawnDelay = 3f;
    }

    public override int MaxTeams {
        get { return 8; }
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

    public override GameModes Mode {
        get { return GameModes.TEAM_DEATHMATCH; }
    }

    public override int MinPlayers {
        get { return 1; }
    }

    public override int MaxPlayers {
        get { return 16; }
    }

    public override string GameModeName {
        get { return "Team Deathmatch"; }
    }

    public override void OnPreGame() {
        Lobby.AddStat("Kills");
    }

    public override string StatToDisplay {
        get { return "Kills"; }
    }

    public override bool GameOver() {
        for (int i = 0; i < 8; i++) {
            if (Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], "Kills") >= ScoreLimit) return true;
        }
        return false;
    }

    public override void OnGameStart() {}

    public override void OnGameEnd() {}
}
