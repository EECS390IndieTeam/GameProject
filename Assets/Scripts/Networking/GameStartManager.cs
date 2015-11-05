using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This script manages the sequence of events that occur when the game begins.  This script
/// only runs on the server.  
/// </summary>
[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame*")]
public class GameStartManager : Bolt.GlobalEventListener {
    //how many seconds we give the players to assign control to the server
    private const float CONTROL_TIMEOUT = 5f;
    //the game will start this many seconds after the sync event is sent
    private const float SYNC_TIME = 4f;

    //a timer for kicking players who take too long
    private float timer;

    //a list of all players who have given control of their PlayerEntity to the server
    private List<IPlayer> controlledPlayers = new List<IPlayer>();

    //true if this script should run.  We can't just use enabled because then it won't recieve the Bolt events it needs to start
    private bool running = false;

    public override void SceneLoadLocalDone(string map) {
        Begin();
    }

    public void Begin() {
        Debug.Log("GameStartManager.Begin()");
        timer = 0f;
        running = true;
    }

    private bool localPlayerSet = false;
    
	
	// Update is called once per frame
	void Update () {
        if (!running) return;
        //a strange roundabout way to find the server player, but it works so shut up
        if (!localPlayerSet) {
            if (GameManager.instance.CurrentPlayer != null) {
                controlledPlayers.Add(GameManager.instance.CurrentPlayer);
                PlayerRegistry.SetPlayer(GameManager.instance.CurrentPlayer);
                localPlayerSet = true;
                Debug.Log("Local player set!");
            }
        }

        //if all players have passed control back to the server
        if(controlledPlayers.Count == Lobby.PlayerCount){
            Finished();
            return;
        }
        timer += Time.deltaTime;
        if (timer > CONTROL_TIMEOUT) {//the timeout has expired
            KickAllNonCompliantPlayers();
            Finished();
        }
	}

    //called after all players have returned control to the server, or have been kicked for not doing so after the timeout has expired
    private void Finished() {
        Debug.Log("GameStartManager.Finished()");
        MoveAllPlayersToStartPoints();
        float startTime = BoltNetwork.serverTime + SYNC_TIME;
        SyncEvent evnt = SyncEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.StartTime = startTime;
        evnt.Send();

        Destroy(this);
    }

    private void MoveAllPlayersToStartPoints() {
        GameManager.instance.gameMode.MovePlayersToStartPoints(controlledPlayers);
        Debug.Log("All Players moved to spawn points!");
    }

    //once each player has finished loading the scene and spawning their players, they must give control back to the server
    //to indicate that they're ready and compliant.  this function will be called when they do.  Due to timing issues, this method
    //cannot be used for the server's local player, so that one is handled in Update()
    public override void ControlOfEntityGained(BoltEntity entity) {
        if (entity.prefabId == BoltPrefabs.PlayerPrefab && !entity.isOwner) {//first ensure that its a player that this event is being called on
            IPlayer player = entity.gameObject.GetComponentInChildren<AbstractPlayer>();
            controlledPlayers.Add(player);//add them to the list
            PlayerRegistry.SetPlayer(player); //add them to the PlayerRegistry, if this is the server's player, it will be done elsewhere
        }
    }

    private void KickAllNonCompliantPlayers() {
        //linq!
        // select all connections whose associated username is not contained within the list of all usernames in the controlledPlayers list
        var playersToKick = PlayerRegistry.Connections.Where(c => !controlledPlayers.Select(x => x.Username).Contains(PlayerRegistry.GetUserNameFromConnection(c)));
        //playersToKick contains all players who are not in the controlledPlayers list
        foreach (BoltConnection c in playersToKick) {
            c.Disconnect(new DisconnectReason("AntiCheat Violation", "Did not surrender control to server in time"));
        }
    }
}
