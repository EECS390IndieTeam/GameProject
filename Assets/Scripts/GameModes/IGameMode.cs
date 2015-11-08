﻿using UnityEngine;
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

    /// <summary>
    /// Moves the given list of players to start points
    /// </summary>
    /// <param name="players"></param>
    void MovePlayersToStartPoints(List<IPlayer> players);

    /// <summary>
    /// Moves the given player to a spawn point
    /// </summary>
    /// <param name="player"></param>
    void MovePlayerToSpawnPoint(IPlayer player);

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
    /// Called when the game starts. Called by both clients and the server
    /// </summary>
    void OnGameStart();

    /// <summary>
    /// Called when the game ends.  Called on both the clients and the server
    /// </summary>
    void OnGameEnd();
}

