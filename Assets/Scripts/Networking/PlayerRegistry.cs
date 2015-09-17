using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerRegistry {
    private static Dictionary<string, BoltConnection> players = new Dictionary<string, BoltConnection>();

    public static void CreatePlayer(BoltConnection connection, string username) {
        players[username] = connection;

        if (connection != null) {
            connection.UserData = username;
        }

        Debug.Log("Player connected: " + username);
    }

    public static IEnumerable<BoltConnection> Connections {
        get { return players.Values.Where<BoltConnection>(c => c != null);}
    }

    public static IEnumerable<string> PlayerNames {
        get { return players.Keys; }
    }

    public static string GetUserNameFromConnection(BoltConnection connection) {
        if (connection == null) return players.First<KeyValuePair<string, BoltConnection>>(e => e.Value == null).Key;
        return (string)connection.UserData; //we also store the username as the connection's userdata for even faster lookup
    }

    public static void Remove(BoltConnection connection) {
        string username = GetUserNameFromConnection(connection);
        if(players.ContainsKey(username)) players.Remove(username);
    }

    public static bool UserConnected(string username) {
        return players.ContainsKey(username);
    }
}
