using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An abstract IGameMode implemtation that handles the respawning for free-for-all games
/// </summary>
public abstract class SimpleFFAGameMode : IGameMode {

    public bool UsesTeams {
        get { return false; }
    }

    public void MovePlayersToStartPoints(List<IPlayer> players) {
        Debug.Log("Moving " + players.Count + " players to their spawn points");
        SpawnPointManager mgr = SpawnPointManager.instance;
        if (mgr == null) {
            Debug.LogError("SpawnPointManager.instance was null!");
            return;
        }
        var points = mgr.GetFFAStartPoints(this.Mode).GetEnumerator();
        foreach(IPlayer p in players){
            
            points.MoveNext();
            if (points.Current == null) {
                Debug.LogError("Error; not enough start points for this game mode!");
                points.Reset();
            }
            Debug.Log(points.Current);
            Debug.Log(p);
            Debug.Log("moving " + p.Username + " to " + points.Current.gameObject);
            p.MoveTo(points.Current.gameObject.transform);
        }
        points.Dispose();
    }

    public void MovePlayerToSpawnPoint(IPlayer player) {
        SpawnPointManager mgr = SpawnPointManager.instance;
        if (mgr == null) {
            Debug.LogError("SpawnPointManager.instance was null!");
            return;
        }
        List<SpawnPoint> points = mgr.GetFFASpawnPoints(this.Mode).ToList();
        int rand = Random.Range(0, points.Count);
        player.MoveTo(points[rand].transform);
    }

    public abstract GameModes Mode {
        get;
    }

    public abstract int MinPlayers {
        get;
    }

    public abstract int MaxPlayers {
        get;
    }

    public abstract string GameModeName {
        get;
    }

    public abstract void OnPreGame();
    public abstract bool GameOver();
    public abstract void OnGameStart();
    public abstract void OnGameEnd();
}
