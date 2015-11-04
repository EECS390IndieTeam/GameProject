using UnityEngine;
using System.Collections;

/// <summary>
/// This script listens for DeathEvents
/// </summary>
[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame*")]
public class DeathEventListener : Bolt.GlobalEventListener {
    public override void OnEvent(DeathEvent evnt) {
        //update scores
        //if (ServerConnectionEventListener.IndexMap.ContainsPlayer(evnt.Killer)) {
        //    GameStats.SetIntegerStat(evnt.Killer, "Kills", GameStats.GetIntegerStat(evnt.Killer, "Kills") + 1);
        //}

        //if (ServerConnectionEventListener.IndexMap.ContainsPlayer(evnt.Player)) {
        //    GameStats.SetIntegerStat(evnt.Player, "Deaths", GameStats.GetIntegerStat(evnt.Player, "Deaths") + 1);
        //}

        if (Lobby.StatCreated("Kills")) {
            Lobby.IncrementStatForPlayer(evnt.Killer, "Kills", 1);
            Lobby.IncrementStatForPlayer(Lobby.PP_TEAMS[Lobby.GetPlayer(evnt.Killer).Team], "Kills", 1);
        }
        if (Lobby.StatCreated("Deaths")) {
            Lobby.IncrementStatForPlayer(evnt.Player, "Deaths", 1);
            Lobby.IncrementStatForPlayer(Lobby.PP_TEAMS[Lobby.GetPlayer(evnt.Player).Team], "Deaths", 1);
        }

        //because of the Owner/ProxyPlayer abstraction, this line will either just move the server's player, or
        //send an MovePlayerEvent to the propper player!
        GameManager.instance.gameMode.MovePlayerToSpawnPoint(PlayerRegistry.GetIPlayerForUserName(evnt.Player));

        //GameManager.instance.CheckForGameOver();
       
    }
}
