using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A game mode for a simple team deathmatch mode
/// </summary>
public class CaptureTheFlagMode : SimpleTeamGameMode {

    public int ScoreLimit = 3;

    //Hardcoded team limit here.
    private bool[] isFlagAtBase = new bool[2];

    public override GameModes Mode {
        get { return GameModes.CAPTURE_THE_FLAG; }
    }

    public override int MinPlayers {
        get { return 2; }
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
        get { return "Capture the Flag"; }
    }

    public override void OnPreGame() {
        GameStats.CreateNewIntegerStat("Kills");
        GameStats.CreateNewIntegerStat("Flags");
        for(int i=0; i<isFlagAtBase.Length; i++)
        {
            isFlagAtBase[i] = true;
        }
    }

    public override bool GameOver() {
        IntegerStatTracker teamStat = GameStats.GetFullIntegerStat("Team");
        IntegerStatTracker flagStat = GameStats.GetFullIntegerStat("Flags");
        int[] teamScores = new int[8];
        for(int i = 0; i < ServerConnectionEventListener.IndexMap.PlayerCount; i++){
            teamScores[teamStat[i]] += flagStat[i];
        }
        for (int i = 0; i < teamScores.Length; i++) {
            if (teamScores[i] >= ScoreLimit) return true;
        }
        return false;
    }

    public override void OnGameStart() {}

    public override void OnGameEnd() {}

    public void setFlagAtBase(int flagNum, bool isAtBase)
    {
        isFlagAtBase[flagNum] = isAtBase;
    }

    public bool isFlagAtBaseForTeam(int teamNum)
    {
        return isFlagAtBase[teamNum];
    }
}
