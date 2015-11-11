﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the player sync process from the player's perspective.
/// See GameStartManager for the server's perspective
/// </summary>
[BoltGlobalBehaviour("ingame*")]
public class PlayerSyncEventListener : Bolt.GlobalEventListener {
    public static float StartTime {
        get;
        private set;
    }

    private SpawningBlind blind;
    private OwnerPlayer player;

    private bool running = false;

    public override void SceneLoadLocalBegin(string map) {
        GameManager.instance.ChangeGameState(GameManager.GameState.PRE_GAME);
        DebugHUD.setValue("load state", "Loading scene");
        //display loading screen
    }


    public override void SceneLoadLocalDone(string map) {
        //instantiate the player
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerPrefab);
        PlayerState ps = entity.GetComponent<PlayerState>();
        player = (OwnerPlayer)ps.Player;
        blind = FindObjectOfType<SpawningBlind>();
        ps.Name = GameManager.instance.CurrentUserName; //set their name
        ps.Team = Lobby.GetPlayer(ps.Name).Team;
        if (!BoltNetwork.isServer) {
            entity.AssignControl(BoltNetwork.server);  //this line is completely useless and should be removed
        }
        DebugHUD.setValue("load state", "Scene loaded, unsynced");
        //the scene has now been loaded for this player, but we have not synchronized the players yet 
        player.ControlEnabled = false;
        blind.Show();
        blind.Text = "Waiting for other players...";
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
        DebugHUD.setValue("load state", "Sync message recieved");
        running = true;
        blind.Text = "Synchronized!\nGame will begin in " + (StartTime - BoltNetwork.serverTime).ToString("F2") + " seconds";
        blind.FadeOut(StartTime - BoltNetwork.serverTime);
    }

    void Update() {
        if (!running) return;
        if (BoltNetwork.serverTime >= StartTime) {
            DebugHUD.setValue("load state", "Game started");
            //allow the player to move
            player.ControlEnabled = true;
            running = false;
            GameManager.instance.ChangeGameState(GameManager.GameState.IN_GAME);
            Destroy(this);
        } else {
            DebugHUD.setValue("load state", "Sync message recieved, game starts in " + (StartTime - BoltNetwork.serverTime));
            blind.Text = "Synchronized!\nGame will begin in " + (StartTime - BoltNetwork.serverTime).ToString("F2") + " seconds";
        }
        DebugHUD.setValue("server time", BoltNetwork.serverTime);
        DebugHUD.setValue("start time", StartTime);
    }
}
