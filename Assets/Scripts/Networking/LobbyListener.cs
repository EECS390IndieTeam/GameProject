﻿using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "testScene")]
public class LobbyListener : Bolt.GlobalEventListener {
    private BoltEntity lobby;

    public override void SceneLoadLocalDone(string map) {
        lobby = BoltNetwork.Instantiate(BoltPrefabs.LobbyList);
    }

    public override void Connected(BoltConnection connection) {
        lobby.SendMessage("UpdateLobby");
    }

    public override void Disconnected(BoltConnection connection) {
        lobby.SendMessage("UpdateLobby");
    }
}
