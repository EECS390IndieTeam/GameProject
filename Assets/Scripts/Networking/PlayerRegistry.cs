using UnityEngine;
using System.Collections.Generic;

public class PlayerRegistry {
    private static Dictionary<string, BoltPlayer> players = new Dictionary<string, BoltPlayer>();

    public static BoltPlayer CreatePlayer(BoltConnection connection) {
        string name = connection == null ? "SERVER" : connection.ConnectionId.ToString();

        BoltPlayer p;
        if (players.ContainsKey(name)) {
            p = players[name];
        } else {
            p = new BoltPlayer();
        }

        p.Connection = connection;

        if (!p.IsServer) {
            p.Connection.UserData = p;
        }

        p.ConnectionName = name;

        players.Add(name, p);

        UnityEngine.Debug.Log("Created player with name " + name);

        Debug.Log("Player connected: " + name);

        return p;
    }

    public static IEnumerable<BoltPlayer> AllPlayers {
        get { return players.Values; }
    }

    public static BoltPlayer GetPlayerFromConnection(BoltConnection connection) {
        if (connection == null) return players["SERVER"];
        return (BoltPlayer)connection.UserData;
    }

    public static void Remove(BoltConnection connection) {
        if (connection == null) {
            players.Remove("SERVER");
            return;
        }
        string name = connection.ConnectionId.ToString();
        if(players.ContainsKey(name)) players.Remove(name);
    }

    public static bool usernameConnected(string username) {
        foreach (BoltPlayer p in AllPlayers) {
            if (p.Name == username) return true;
        }
        return false;
    }
}
