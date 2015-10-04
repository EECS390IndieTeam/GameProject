using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerRegistry {
    private static Dictionary<string, PlayerInfo> players = new Dictionary<string, PlayerInfo>();

    public static void CreatePlayer(BoltConnection connection, string username) {
        PlayerInfo pair = new PlayerInfo(username, connection);
        players[username] = pair;

        if (connection != null) {
            connection.UserData = pair;
        }

        Debug.Log("Player connected: " + username);
    }

    public static IEnumerable<BoltConnection> Connections {
        get { return players.Values.Select(x => x.Connection).Where(x => x != null);}
    }

    public static IEnumerable<string> PlayerNames {
        get { return players.Keys; }
    }

    public static IEnumerable<IPlayer> Players {
        get { return players.Values.Select(x => x.Player); }
    }

    //assigns an IPlayer to the corresponding player in the registry
    public static void SetPlayer(IPlayer player) {
        if (player == null) Debug.Log("player is null");
        Debug.Log("IPlayer added to registry of type " + player.GetType().Name + " with username " + player.Username);
        players[player.Username].Player = player;//wow, this line sucks
    }

    public static string GetUserNameFromConnection(BoltConnection connection) {
        if (connection == null) return players.First(e => e.Value.Connection == null).Key;
        return ((PlayerInfo)connection.UserData).Username; //we also store the PlayerInfo as the connection's userdata for even faster lookup
    }

    public static BoltConnection GetConnectionFromUserName(string username) {
        return players[username].Connection;
    }

    public static IPlayer GetIPlayerForUserName(string username) {
        return players[username].Player;
    }

    public static void Remove(BoltConnection connection) {
        string username = GetUserNameFromConnection(connection);
        if(players.ContainsKey(username)) players.Remove(username);
    }

    public static bool UserConnected(string username) {
        return players.ContainsKey(username);
    }

    private class PlayerInfo {
        public string Username;
        public BoltConnection Connection;
        public IPlayer Player;

        public PlayerInfo(string name, BoltConnection con, IPlayer p = null) {
            Username = name;
            Connection = con;
            Player = p;
        }
    }
}
