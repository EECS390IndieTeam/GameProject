using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This script listens for DeathEvents
/// </summary>
[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame*")]
public class DeathEventListener : Bolt.GlobalEventListener {
    private Queue<Player> playersToRespawn = new Queue<Player>();
    private float nextRespawnTime = 0f;
    private bool respawning = false;


    public override void OnEvent(DeathEvent evnt) {
        if (Lobby.StatCreated("Kills")) {
            Lobby.IncrementStatForPlayer(evnt.Killer, "Kills", 1);
            Lobby.IncrementStatForPlayer(Lobby.PP_TEAMS[Lobby.GetPlayer(evnt.Killer).Team], "Kills", 1);
        }
        if (Lobby.StatCreated("Deaths")) {
            Lobby.IncrementStatForPlayer(evnt.Player, "Deaths", 1);
            Lobby.IncrementStatForPlayer(Lobby.PP_TEAMS[Lobby.GetPlayer(evnt.Player).Team], "Deaths", 1);
        }

        //the server player has died
        //Removed this because it was causing a "snapback" after player respawn, due to the player moving twice per respawn
        //GameManager.instance.GameMode.MovePlayerToSpawnPoint(PlayerRegistry.GetIPlayerForUserName(evnt.Player), true);
        float nextTime = BoltNetwork.serverTime + GameManager.instance.GameMode.RespawnDelay;
        if (playersToRespawn.Count == 0) {
            nextRespawnTime = nextTime;
            respawning = true;
        }
        //Attemped change to deal with player bouncing after death
        //Disable the player so it effective disappears on death
        AbstractPlayer p = (AbstractPlayer)PlayerRegistry.GetIPlayerForUserName(evnt.Player);
        p.gameObject.SetActive(false);
        //end change
        playersToRespawn.Enqueue(new Player(PlayerRegistry.GetIPlayerForUserName(evnt.Player), nextTime));
    }

    void Update() {
        if (!respawning) return;
        if (BoltNetwork.serverTime >= nextRespawnTime) {
            Player p = playersToRespawn.Dequeue();
            //Second part of change to deal with player bouncing after death
            //Re-enable the player after respawning
            ((AbstractPlayer)p.player).gameObject.SetActive(true);
            //End of change
            GameManager.instance.GameMode.MovePlayerToSpawnPoint(p.player, true);
            if (playersToRespawn.Count == 0) {
                respawning = false;
            } else {
                nextRespawnTime = playersToRespawn.Peek().respawnTime;
            }
        }
    }

    private struct Player {
        public Player(IPlayer p, float t) {
            this.player = p;
            this.respawnTime = t;
        }

        public float respawnTime;
        public IPlayer player;
    }
}
