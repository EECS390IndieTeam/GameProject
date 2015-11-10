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
        GameManager.instance.GameMode.MovePlayerToSpawnPoint(PlayerRegistry.GetIPlayerForUserName(evnt.Player), true);
        float nextTime = BoltNetwork.serverTime + GameManager.instance.GameMode.RespawnDelay;
        if (playersToRespawn.Count == 0) {
            nextRespawnTime = nextTime;
            respawning = true;
        }

        playersToRespawn.Enqueue(new Player(PlayerRegistry.GetIPlayerForUserName(evnt.Player), nextTime));
    }

    void Update() {
        if (!respawning) return;
        if (BoltNetwork.serverTime >= nextRespawnTime) {
            Player p = playersToRespawn.Dequeue();
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
