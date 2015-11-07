using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An abstract IGameMode implemtation that handles the respawning for Team games
/// </summary>
public abstract class SimpleTeamGameMode : IGameMode {
    public bool UsesTeams {
        get { return true; }
    }

    public void MovePlayersToStartPoints(List<IPlayer> players) {
        for (int team = 0; team < 8; team++) {
            MoveTeamToStartPoints(team, players.Where(pl => pl.Team == team));
        }
    }

    private void MoveTeamToStartPoints(int team, IEnumerable<IPlayer> players) {
        SpawnPointManager mgr = SpawnPointManager.instance;
        if (mgr == null) {
            Debug.LogError("SpawnPointManager.instance was null!");
            return;
        }
        var p = players.GetEnumerator();
        var points = mgr.GetStartPointsForTeam(team, this.Mode).GetEnumerator();
        while (p.MoveNext()) {
            points.MoveNext();
            if (points.Current == null) {
                Debug.LogError("Error; not enough start points for team " + team);
                points.Reset();
            }
            p.Current.MoveTo(points.Current.transform);
        }
        p.Dispose();
        points.Dispose();
    }

    public void MovePlayerToSpawnPoint(IPlayer player) {
        SpawnPointManager mgr = SpawnPointManager.instance;
        if (mgr == null) {
            Debug.LogError("SpawnPointManager.instance was null!");
            return;
        }
        List<SpawnPoint> points = mgr.GetSpawnPointsForTeam(player.Team, this.Mode).ToList();
        int rand = Random.Range(0, points.Count);
        player.MoveTo(points[rand].transform);
    }

    public abstract GameModes Mode {
        get;
    }

    public abstract int MaxTeams {
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
