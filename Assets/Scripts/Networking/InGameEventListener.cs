using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour("ingame")]
public class InGameEventListener : Bolt.GlobalEventListener {
    public static float StartTime {
        get;
        private set;
    }

    private bool running = false;
    public override void SceneLoadLocalBegin(string map) {
        DebugHUD.setValue("load state", "Soading scene");
        FindObjectOfType<LobbyBehaviour>().HideDebugDraw = true;
        //display loading screen
    }


    public override void SceneLoadLocalDone(string map) {
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerPrefab);
        if (!BoltNetwork.isServer) {
            entity.AssignControl(BoltNetwork.server);
        }
        DebugHUD.setValue("load state", "Scene loaded, unsynced");
    }

    //Sync Event handler (client and server)
    public override void OnEvent(SyncEvent evnt) {
        Debug.Log("Sync event recieved");
        StartTime = evnt.StartTime;
        if (evnt.StartTime < BoltNetwork.serverTime) {
            if (!BoltNetwork.isServer) {
                BoltNetwork.server.Disconnect(new DisconnectReason("Sync Failure", "Failed to synchronize with the server in time"));
            } else {
                Debug.LogWarning("Server recieved sync message that is before the current time?");
            }
        }
        DebugHUD.setValue("load state", "Synch message recieved");
        running = true;
        //hide loading screen
    }

    void Update() {
        if (!running) return;
        if (BoltNetwork.serverTime >= StartTime) {
            DebugHUD.setValue("load state", "Game started");
            //allow the player to move
            running = false;
        } else {
            DebugHUD.setValue("load state", "Sync message recieved, game starts in " + (StartTime - BoltNetwork.serverTime));
        }
        DebugHUD.setValue("server time", BoltNetwork.serverTime);
        DebugHUD.setValue("start time", StartTime);
    }
}
