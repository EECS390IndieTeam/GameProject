using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour("ingame")]
public class InGameEventListener : Bolt.GlobalEventListener {
    public static float StartTime {
        get;
        private set;
    }
    public override void SceneLoadLocalBegin(string map) {
        DebugHUD.setValue("load state", "Soading scene");
    }


    public override void SceneLoadLocalDone(string map) {
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerPrefab);
        if (BoltNetwork.isServer) {
            entity.TakeControl();
        } else {
            entity.AssignControl(BoltNetwork.server);
        }
        DebugHUD.setValue("load state", "Scene loaded, unsynced");
    }

    //Sync Event handler (client and server)
    public override void OnEvent(SyncEvent evnt) {
        Debug.Log("Sync event recieved");
        StartTime = evnt.StartTime;
        if (evnt.StartTime > BoltNetwork.serverTime) {
            BoltNetwork.server.Disconnect(new DisconnectReason("Sync Failure", "Failed to synchronize with the server in time"));
        }
        DebugHUD.setValue("load state", "Synch message recieved");
    }

    void Update() {
        
        if (BoltNetwork.serverTime >= StartTime) {
            DebugHUD.setValue("load state", "Game started");
        } else {
            DebugHUD.setValue("load state", "Sync message recieved, game starts in " + (StartTime - BoltNetwork.serverTime));
        }
        DebugHUD.setValue("server time", BoltNetwork.serverTime);
        DebugHUD.setValue("start time", StartTime);
    }
}
