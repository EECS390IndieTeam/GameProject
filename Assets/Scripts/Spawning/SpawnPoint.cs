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

#if UNITY_EDITOR
    void OnDrawGizmosSelected(){
        if (Physics.CheckSphere(transform.position, 0.7f)) {
            Gizmos.color = Color.red;
        } else {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(transform.position, 0.7f);
    }
#endif
}
