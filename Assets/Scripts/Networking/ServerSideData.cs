﻿using UnityEngine;
using System.Collections;

public static class ServerSideData {
    //public static LobbyState Lobby;
    
    //public static ServerConnectionEventListener ConnectionEventListener;
    //public static GameStartManager GameStartManager;
    public static string MOTD = "It's a server!";
    public static string ServerName = "Hosted Server";
    public static bool IsDedicated = false;
    public static string Password = "";

    public static void UpdateZeusData() {
        if (!BoltNetwork.isServer) throw new System.Exception("Attempted to update zeus data on a client!");
        ServerInfoToken token = new ServerInfoToken();
        token.IsDedicatedServer = IsDedicated;
        token.GameMode = GameManager.instance.gameMode.GameModeName;
        token.MapName = "TEMP_NULL";
        token.MaxPlayerCount = (byte)GameManager.instance.gameMode.MaxPlayers;
        token.PlayerCount = (byte)GameManager.instance.Lobby.PlayerCount;
        token.MOTD = MOTD;
        token.PasswordRequired = !string.IsNullOrEmpty(Password);
        token.ServerName = ServerName;
        GameManager.GameState state = GameManager.instance.CurrentGameState;
        token.HideInServerList = state == GameManager.GameState.LOBBY || state == GameManager.GameState.POST_GAME;
        BoltNetwork.SetHostInfo(ServerName, token);
    }
}
