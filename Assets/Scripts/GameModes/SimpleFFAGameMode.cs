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

    public int MaxTeams {
        get { return 1; }
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

    public void MovePlayerToSpawnPoint(IPlayer player, bool respawn) {
        SpawnPointManager mgr = SpawnPointManager.instance;
        if (mgr == null) {
            Debug.LogError("SpawnPointManager.instance was null!");
            return;
        }
        List<SpawnPoint> points = mgr.GetFFASpawnPoints(this.Mode).ToList();
        int rand = Random.Range(0, points.Count);
        if (respawn) {
            player.RespawnAt(points[rand].transform);
        } else {
            player.MoveTo(points[rand].transform);
        }

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

    public abstract string StatToDisplay {
        get;
    }
    public abstract int ScoreLimit {
        get;
        set;
    }

    public abstract float TimeLimit {
        get;
        set;
    }

    public abstract float RespawnDelay {
        get;
        set;
    }

    public abstract void OnPreGame();
    public virtual bool GameOver() {
        foreach (var p in Lobby.AllPlayers) {
            if (p.GetStat(StatToDisplay) >= ScoreLimit) return true;
        }
        return false;
    }
    public virtual string GetWinner() {
        List<string> winners = new List<string>();
        int max = 0;
        foreach (var p in Lobby.AllPlayers) {
            int stat = p.GetStat(StatToDisplay);
            if (stat > max) {
                max = stat;
                winners.Clear();
                winners.Add(p.Name);
            } else if (stat == max) {
                winners.Add(p.Name);
            }
        }
        return string.Join(", ", winners.ToArray());
    }
    public abstract void OnGameStart();
    public abstract void OnGameEnd();
    
}
