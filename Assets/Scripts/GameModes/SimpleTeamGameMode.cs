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

    public void MovePlayerToSpawnPoint(IPlayer player, bool respawn) {
        SpawnPointManager mgr = SpawnPointManager.instance;
        if (mgr == null) {
            Debug.LogError("SpawnPointManager.instance was null!");
            return;
        }
        List<SpawnPoint> points = mgr.GetSpawnPointsForTeam(player.Team, this.Mode).ToList();
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
        for (int i = 0; i < 8; i++) {//iterate through all teams
            //we store the team's total score in their team's default pseudoplayer
            if (Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], StatToDisplay) >= ScoreLimit) {
                Debug.Log("Team " + i + " won with their score of " + Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], StatToDisplay) + " which is more than the limit of " + ScoreLimit);
                return true;
            }
        }
        return false;
    }
    public virtual string GetWinner() {
        List<string> winners = new List<string>();
        int max = 0;
        for (int i = 0; i < 8; i++) {
            int stat = Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], StatToDisplay);
            if (stat > max) {
                max = stat;
                winners.Clear();
                winners.Add(Teams.Names[i]);
            } else if (stat == max) {
                winners.Add(Teams.Names[i]);
            }
        }
        return string.Join(", ", winners.ToArray());
    }
    public abstract void OnGameStart();
    public abstract void OnGameEnd();
    
}
