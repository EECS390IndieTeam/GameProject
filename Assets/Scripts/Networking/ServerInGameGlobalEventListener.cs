using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "ingame")]
public class ServerInGameGlobalEventListener : Bolt.GlobalEventListener {
    //public override void SceneLoadRemoteDone(BoltConnection connection) {
    //    BoltEntity ent = BoltNetwork.Instantiate(BoltPrefabs.PlayerPrefab);
    //    ent.AssignControl(connection);
    //}
    //public override void SceneLoadLocalDone(string map) {
    //    BoltNetwork.Instantiate(BoltPrefabs.PlayerPrefab).TakeControl();
    //}
}
