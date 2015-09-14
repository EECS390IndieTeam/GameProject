using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour("ingame")]
public class InGameEventListener : Bolt.GlobalEventListener {
    public override void SceneLoadLocalDone(string map) {
        ClientSidePlayerPrefab.Instantiate();
        ClientSidePlayerPrefab.instance.AttachTo(BoltNetwork.Instantiate(BoltPrefabs.PlayerPrefab).transform);
    }
}
