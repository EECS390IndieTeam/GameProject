using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A game mode for a simple free-for-all deathmatch mode
/// </summary>
public class FFADeathmatchMode : SimpleFFAGameMode {

    public int ScoreLimit = 25;

    public override GameModes Mode {
        get { return GameModes.FFA_DEATHMATCH; }
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
        get { return "Deathmatch"; }
    }

    public override void OnPreGame() {
        GameStats.CreateNewIntegerStat("Kills");
    }

    public override bool GameOver() {
        for (int i = 0; i < ServerConnectionEventListener.IndexMap.PlayerCount; i++) {
            if (GameStats.GetIntegerStat(i, "Kills") >= ScoreLimit) return true;
        }
        return false;
    }

    public override void OnGameStart() {}

    public override void OnGameEnd() {}
}
