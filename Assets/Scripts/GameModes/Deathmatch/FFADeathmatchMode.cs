using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A game mode for a simple free-for-all deathmatch mode
/// </summary>
public class FFADeathmatchMode : SimpleFFAGameMode {
    public FFADeathmatchMode() {
        this.ScoreLimit = 25;
        this.TimeLimit = 5f * 60f;
        this.RespawnDelay = 3f;
    }

    public override GameModes Mode {
        get { return GameModes.FFA_DEATHMATCH; }
    }

    public override int MinPlayers {
        get { return 1; }
    }

    public override int MaxPlayers {
        get { return 16; }
    }

    public override string GameModeName {
        get { return "Deathmatch"; }
    }

    public override void OnPreGame() {
        Lobby.AddStat("Kills");
    }

    public override string StatToDisplay {
        get { return "Kills"; }
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

    public override void OnGameStart() {}

    public override void OnGameEnd() {}
}
