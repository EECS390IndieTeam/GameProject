using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnPointManager : MonoBehaviour {
    public static SpawnPointManager instance;
    private List<SpawnPoint> spawnPoints;

    void Awake() {
        spawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        instance = this;
    }

    /// <summary>
    /// returns all spawn points that are for the given team number in the given game mode
    /// </summary>
    /// <param name="team"></param>
    /// <param name="gameMode"></param>
    /// <returns></returns>
    public IEnumerable<SpawnPoint> GetSpawnPointsForTeam(int team, GameModes gameMode) {
        return spawnPoints.Where(sp => sp.UsableInTeamModes && (sp.UsableInAnyGameMode || sp.ValidGameModes.Contains(gameMode)) && sp.Team == team && sp.IsSpawnPoint);
    }

    /// <summary>
    /// returns all start points that are for the given team number in the given game mode
    /// </summary>
    /// <param name="team"></param>
    /// <param name="gameMode"></param>
    /// <returns></returns>
    public IEnumerable<SpawnPoint> GetStartPointsForTeam(int team, GameModes gameMode) {
        return spawnPoints.Where(sp => sp.UsableInTeamModes && (sp.UsableInAnyGameMode || sp.ValidGameModes.Contains(gameMode)) && sp.Team == team && sp.IsStartPoint);
    }

    /// <summary>
    /// returns all free-for-all spawn points for the given game mode
    /// </summary>
    /// <param name="gameMode"></param>
    /// <returns></returns>
    public IEnumerable<SpawnPoint> GetFFASpawnPoints(GameModes gameMode) {
        return spawnPoints.Where(sp => sp.UsableInFreeForAll && (sp.UsableInAnyGameMode || sp.ValidGameModes.Contains(gameMode)) && sp.IsSpawnPoint);
    }

    /// <summary>
    /// returns all free-for-all start points for the given game mode
    /// </summary>
    /// <param name="gameMode"></param>
    /// <returns></returns>
    public IEnumerable<SpawnPoint> GetFFAStartPoints(GameModes gameMode) {
        return spawnPoints.Where(sp => sp.UsableInFreeForAll && (sp.UsableInAnyGameMode || sp.ValidGameModes.Contains(gameMode)) && sp.IsStartPoint);
    }
}
