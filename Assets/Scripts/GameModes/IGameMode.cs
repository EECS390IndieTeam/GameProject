using UnityEngine;
using System.Collections.Generic;

public interface IGameMode {

    /// <summary>
    /// A GameModes enum value corresponding to this GameMode
    /// </summary>
    GameModes Mode {
        get;
    }

    /// <summary>
    /// The minimum number of players that are needed to start this game mode
    /// </summary>
    int MinPlayers {
        get;
    }

    /// <summary>
    /// the maximum number of players that this game mode supports
    /// </summary>
    int MaxPlayers {
        get;
    }

    /// <summary>
    /// The maximum number of teams
    /// </summary>
    int MaxTeams {
        get;
    }

    /// <summary>
    /// True if this game mode uses teams
    /// </summary>
    bool UsesTeams {
        get;
    }

    /// <summary>
    /// The human-readable name for this game mode
    /// </summary>
    string GameModeName {
        get;
    }

    /// <summary>
    /// The name of the stat that should be displayed at the top of the screen
    /// If this is a team mode, it will show team pseudoplayers instead
    /// </summary>
    string StatToDisplay {
        get;
    }

    int ScoreLimit {
        get;
        set;
    }

    float TimeLimit {
        get;
        set;
    }

    float RespawnDelay {
        get;
        set;
    }

    /// <summary>
    /// Moves the given list of players to start points
    /// </summary>
    /// <param name="players"></param>
    void MovePlayersToStartPoints(List<IPlayer> players);

    /// <summary>
    /// Moves the given player to a spawn point.
    /// If the respawn parameter is true, IPlayer.RespawnAt() will be called instead of MoveTo()
    /// </summary>
    /// <param name="player"></param>
    /// <param name="respawn"></param>
    void MovePlayerToSpawnPoint(IPlayer player, bool respawn);

    /// <summary>
    /// Called after the host has begun to start the game, but before the game has started
    /// Will be called on both the clients and the server.  Use this function to register any
    /// stats that will be needed
    /// </summary>
    void OnPreGame();

    /// <summary>
    /// Checks the game state and returns true if the game should end.
    /// Will only be called on the server.  If the game ends due to the
    /// timer expirin, this function will not be called.
    /// </summary>
    /// <returns></returns>
    bool GameOver();

    /// <summary>
    /// Returns the name of the winner of the game.  In a non-team mode, it will return the username of the winner.
    /// In team modes, it will return the string corresponding to the winning team's id from Teams.Names.  
    /// If there is no winner, the top player/team is returned.
    /// If there is a tie, the result is either null or a comma-seperated list of winners
    /// </summary>
    /// <returns></returns>
    string GetWinner();

    /// <summary>
    /// Called when the game starts. Called by both clients and the server
    /// </summary>
    void OnGameStart();

    /// <summary>
    /// Called when the game ends.  Called on both the clients and the server
    /// </summary>
    void OnGameEnd();
}

