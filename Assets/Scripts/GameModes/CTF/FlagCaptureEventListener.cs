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
        if (ServerConnectionEventListener.IndexMap.ContainsPlayer(evnt.Player))
        {
            GameStats.SetIntegerStat(evnt.Player, "Flags", GameStats.GetIntegerStat(evnt.Player, "Flags") + 1);
        }
        
        GameManager.instance.CheckForGameOver();

    }
}
