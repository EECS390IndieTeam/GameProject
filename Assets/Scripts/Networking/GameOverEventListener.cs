using UnityEngine;
using System.Collections;


[BoltGlobalBehaviour(BoltNetworkModes.Client, "ingame*")]
public class GameOverEventListener : Bolt.GlobalEventListener {
    //Tells clients to end the game
    public override void OnEvent(GameOverEvent evnt) {
        GameManager.instance.GameOver();
    }
}
