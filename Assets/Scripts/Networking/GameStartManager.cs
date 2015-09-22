using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame")]
public class GameStartManager : Bolt.GlobalEventListener {
    private const float CONTROL_TIMEOUT = 5f;
    private const float SYNC_TIME = 4f;

    private float timer;

    private List<BoltEntity> controlledPlayers = new List<BoltEntity>();

    private LobbyState lobby;

    private bool running = false;

    void Start() {
        lobby = FindObjectOfType<LobbyState>();
    }

    public override void SceneLoadLocalDone(string map) {
        Begin();
    }

    public void Begin() {
        Debug.Log("GameStartManager.Begin()");
        timer = 0f;
        running = true;
    }
    
	
	// Update is called once per frame
	void Update () {
        if (!running) return;
        if(controlledPlayers.Count == lobby.PlayerCount){
            Finished();
            return;
        }
        timer += Time.deltaTime;
        if (timer > CONTROL_TIMEOUT) {
            KickAllNonCompliantPlayers();
            Finished();
        }
	}

    private void Finished() {
        Debug.Log("GameStartManager.Finished()");
        MoveAllPlayersToSpawnPoint();
        float startTime = BoltNetwork.serverTime + SYNC_TIME;
        SyncEvent evnt = SyncEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.StartTime = startTime;
        evnt.Send();

        Destroy(this);
    }

    private void MoveAllPlayersToSpawnPoint() {
        Debug.Log("All Players moved to spawn points!");
        //TODO this
    }


    public override void ControlOfEntityGained(BoltEntity entity) {
        if (entity.prefabId == BoltPrefabs.PlayerPrefab) {
            controlledPlayers.Add(entity);
        }
    }

    private void KickAllNonCompliantPlayers() {
        var playersToKick = PlayerRegistry.Connections.Where<BoltConnection>(c => controlledPlayers.Count(b => b.source == c) == 0);
        foreach (BoltConnection c in playersToKick) {
            c.Disconnect(new DisconnectReason("AntiCheat Violation", "Did not surrender control to server in time"));
        }
    }
}
