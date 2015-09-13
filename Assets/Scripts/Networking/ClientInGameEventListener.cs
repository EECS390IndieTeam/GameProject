using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour("ingame")]
public class ClientInGameEventListener : Bolt.GlobalEventListener {
    public override void SceneLoadLocalDone(string map) {
        ClientSidePlayerPrefab.Instantiate();
        ClientSidePlayerPrefab.instance.AttachTo(BoltNetwork.Instantiate(BoltPrefabs.PlayerPrefab).transform);
    }


    //public override void ControlOfEntityGained(BoltEntity entity) {
    //    if (entity.prefabId == BoltPrefabs.PlayerPrefab) {
    //        Debug.Log("controlling a player");
    //        ClientSidePlayerPrefab.instance.AttachTo(entity.transform);
    //    }
    //}
}
