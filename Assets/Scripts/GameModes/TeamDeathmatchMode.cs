using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A game mode for a simple team deathmatch mode
/// </summary>
public class TeamDeathmatchMode : SimpleTeamGameMode {

    public int ScoreLimit = 25;

    public override GameModes Mode {
        get { return GameModes.TEAM_DEATHMATCH; }
    }

    public override int MinPlayers {
        get { return 1; }
    }

    public override int MaxPlayers {
        get { return 16; }
    }

    public override IGameLevel level {
        get {
            throw new System.NotImplementedException();
        }
        set {
            throw new System.NotImplementedException();
        }
    }

    public override string GameModeName {
        get { return "Team Deathmatch"; }
    }

    public override void OnPreGame() {
        GameStats.CreateNewIntegerStat("Kills");
    }

    public override bool GameOver() {
        IntegerStatTracker teamStat = GameStats.GetFullIntegerStat("Team");
        IntegerStatTracker killStat = GameStats.GetFullIntegerStat("Kills");
        int[] teamScores = new int[8];
        for(int i = 0; i < ServerConnectionEventListener.IndexMap.PlayerCount; i++){
            teamScores[teamStat[i]] += killStat[i];
        }
        for (int i = 0; i < teamScores.Length; i++) {
            if (teamScores[i] >= ScoreLimit) return true;
        }
        return false;
    }

    public override void OnGameStart() {}

    public override void OnGameEnd() {}
}
