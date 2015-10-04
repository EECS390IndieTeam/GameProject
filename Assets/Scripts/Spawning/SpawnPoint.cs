using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
    public int Team;
    public bool UsableInTeamModes;
    public bool UsableInFreeForAll;
    public bool IsSpawnPoint;
    public bool IsStartPoint;
    public bool UsableInAnyGameMode;
    public GameModes[] ValidGameModes;
}
