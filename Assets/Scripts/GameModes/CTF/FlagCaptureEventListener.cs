using UnityEngine;
using System.Collections;

/// <summary>
/// This script listens for DeathEvents
/// </summary>
[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame*")]
public class FlagCaptureEventListener: Bolt.GlobalEventListener
{
    public override void OnEvent(FlagCapturedEvent evnt)
    {
        //update scores
        Lobby.IncrementStatForPlayer(evnt.Player, "Flags", 1);
        int team = Lobby.GetPlayer(evnt.Player).Team;
        Lobby.IncrementStatForPlayer(Lobby.PP_TEAMS[team], "Flags", 1);

    }
}
