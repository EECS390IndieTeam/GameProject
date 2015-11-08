using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[BoltGlobalBehaviour]
public class Lobby : Bolt.GlobalEventListener {
    //the actual list of players
    private static List<LobbyPlayer> players = new List<LobbyPlayer>();

    //both of these contain the mappings of statistic names to their indicies for serialization
    private static Dictionary<string, byte> statNameMap = new Dictionary<string, byte>();
    private static List<string> statNames = new List<string>();

    //these "pseudoplayers" can be used to store global statistics
    private static Dictionary<string, Dictionary<byte, int>> pseudoplayers = new Dictionary<string, Dictionary<byte, int>>();

    private static List<string> displayedStatNames = new List<string>();

    /// <summary>
    /// returns a list of stats that should be displayed on the in-game scoreboard;
    /// </summary>
    public IEnumerable<string> DisplayedStatNames {
        get { return displayedStatNames.AsEnumerable(); }
    }

    /// <summary>
    /// Adds the given stat to the in-game stat display screen.
    /// SERVER ONLY
    /// </summary>
    /// <param name="statname"></param>
    public static void DisplayStat(string statname) {
        if (!BoltNetwork.isServer) throw new Exception("Only the server can add stats to display");
        if (!StatCreated(statname)) throw new Exception("Tried to display a stat that does not exist!");
        if (!displayedStatNames.Contains(statname)) {
            displayedStatNames.Add(statname);

        }
    }


    /// <summary>
    /// returns the total number of players in the lobby
    /// </summary>
    public static int PlayerCount { get { return players.Count; } }

    /// <summary>
    /// returns the number of players in the lobby who are currently connected.
    /// This value will always match PlayerCount when not in game, and is slower 
    /// to calculate than PlayerCount, so just use PlayerCount when not in game
    /// </summary>
    public static int ConnectedPlayerCount {
        get {
            return players.Count(player => player.Connected);
        }
    }


    private static string _GameModeName;
    /// <summary>
    /// Gets or sets the GameMode class name of the current game mode.  Only the server can set this value.  
    /// </summary>
    public static string GameModeName {
        get {
            return _GameModeName;
        }
        set {
            if (!BoltNetwork.isServer) throw new Exception("Only the server can set the current game mode");
            _GameModeName = value;
            GameModeUpdateEvent evnt = GameModeUpdateEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.Name = value;
            evnt.Send();
            FireChangeEvent(LobbyChange.MODE_CHANGED);
        }
    }

    private static string _MapName;
    public static string MapName {
        get {
            return _MapName;
        }
        set {
            if (!BoltNetwork.isServer) throw new Exception("Only the server can set the current game mode");
            _MapName = value;
            MapUpdateEvent evnt = MapUpdateEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.Name = value;
            evnt.Send();
            FireChangeEvent(LobbyChange.MAP_CHANGED);
            
        }
    }

    /// <summary>
    /// A list of all connected players
    /// </summary>
    public static IEnumerable<LobbyPlayer> AllPlayers {
        get {
            return players;
        }
    }

    /// <summary>
    /// returns the LobbyPlayer with the given name, or null if there is not one
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static LobbyPlayer GetPlayer(string name) {
        return players.FirstOrDefault(p => p.Name == name);
    }

    /// <summary>
    /// returns all players on a given team
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public static IEnumerable<LobbyPlayer> GetPlayersOnTeam(int team) {
        return players.Where(p => p.Team == team);
    }

    /// <summary>
    /// Sorts the lobby list
    /// </summary>
    private static void Sort() {
        players.Sort(new Sorter());
    }

    /// <summary>
    /// Gets the value of the given stat for the given player or pseudoplayer. If that player or pseudoplayer is not found
    /// or the requested stat has not been created yet, an exception is thrown.  If the stat has not been set for the given
    /// player or pseudoplayer, the default value of zero is returned
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="stat"></param>
    /// <returns></returns>
    public static int GetStatForPlayer(string playerName, string stat) {
        if(!statNameMap.ContainsKey(stat)){
            throw new Exception("Requested stat \"" + stat + "\" but it has not been created yet!");
        }
        byte statid = statNameMap[stat];
        LobbyPlayer player = GetPlayer(playerName);
        if (player == null) {
            if (pseudoplayers.ContainsKey(playerName)) {
                var pseudoplayer = pseudoplayers[playerName];
                if (pseudoplayer.ContainsKey(statid)) {
                    return pseudoplayer[statid];
                } else {
                    return 0;
                }
            } else {
                //throw new Exception("Requested stat \"" + stat + "\" from unknown player \"" + playerName + "\"");
                return 0;
            }
        } else {
            return player.GetStat(statid);
        }
    }

    /// <summary>
    /// SERVER ONLY
    /// Sets the given stat to the given value for the given player or pseudoplayer.  
    /// An exception is thrown if the stat has not been created yet.
    /// If the player could not be found, a new pseudoplayer is created.
    /// </summary>
    /// <param name="playername"></param>
    /// <param name="statname"></param>
    /// <param name="value"></param>
    public static void SetStatForPlayer(string playername, string statname, int value) {
        if (!BoltNetwork.isServer) throw new Exception("Only the server can set stats for players");
        if (!StatCreated(statname)) {
            throw new Exception("Tried to set stat \"" + statname + "\" but it has not been created yet!");
        }
        byte stat = statNameMap[statname];
        LobbyPlayer player = GetPlayer(playername);
        if (player == null) {
            if (!pseudoplayers.ContainsKey(playername)) pseudoplayers.Add(playername, new Dictionary<byte, int>());
            pseudoplayers[playername][stat] = value;
        } else {
            player.Stats[stat] = value;
        }
        //tell the clients!
        StatUpdateEvent evnt = StatUpdateEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
        StatUpdateToken token = new StatUpdateToken();
        token.AddChange(playername, stat, value);
        evnt.Token = token;
        evnt.Send();

        Sort();
        FireChangeEvent(LobbyChange.STAT_CHANGED);
    }

    /// <summary>
    /// Increments (or decrements) the given stat for the given player by the given amount.
    /// Can only be called by the server.
    /// An exception is thrown if the stat has not been created yet.
    /// If the player could not be found, a new pseudoplayer is created.
    /// </summary>
    /// <param name="playername"></param>
    /// <param name="statname"></param>
    /// <param name="delta"></param>
    public static void IncrementStatForPlayer(string playername, string statname, int delta) {
        if (!BoltNetwork.isServer) throw new Exception("Only the server can set stats for players");
        if (!StatCreated(statname)) {
            throw new Exception("Tried to increment stat \"" + statname + "\" but it has not been created yet!");
        }
        byte stat = statNameMap[statname];
        LobbyPlayer player = GetPlayer(playername);
        int newval;
        if (player == null) {
            if (!pseudoplayers.ContainsKey(playername)) pseudoplayers.Add(playername, new Dictionary<byte, int>());
            var pseudoplayer = pseudoplayers[playername];
            if (!pseudoplayer.ContainsKey(stat)) {
                newval = delta;
            } else {
                newval = pseudoplayer[stat] + delta;
            }
            pseudoplayer[stat] = newval;
        } else {
            newval = player.GetStat(stat) + delta;
            player.SetStat(stat, newval);
        }
        //tell the clients!
        StatUpdateEvent evnt = StatUpdateEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
        StatUpdateToken token = new StatUpdateToken();
        token.AddChange(playername, stat, newval);
        evnt.Token = token;
        evnt.Send();

        Sort();
        FireChangeEvent(LobbyChange.STAT_CHANGED);
    }

    /// <summary>
    /// returns true if the given stat has been created yet
    /// </summary>
    /// <param name="stat"></param>
    /// <returns></returns>
    public static bool StatCreated(string stat) {
        return statNameMap.ContainsKey(stat);
    }

    /// <summary>
    /// Clears all stats from all players.
    /// Removes all stats that have been created.
    /// Removes all pseudoplayers.  
    /// Only the server can do this.
    /// </summary>
    public static void ClearAllStats() {
        if (!BoltNetwork.isServer) throw new Exception("Only the server can reset stats");
        foreach (LobbyPlayer p in players) {
            p.ClearStats();
        }
        statNameMap.Clear();
        statNames.Clear();
        pseudoplayers.Clear();
        FullLobbyDataToken token = generateFullDataToken();
        FullLobbyDataResponse evnt = FullLobbyDataResponse.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.Token = token;
        evnt.Send();
        Sort();
        FireChangeEvent(LobbyChange.ALL);
    }

    public delegate void LobbyUpdated(LobbyChange change);

    /// <summary>
    /// called when the lobby changes
    /// </summary>
    public static event LobbyUpdated LobbyUpdatedEvent;

    /// <summary>
    /// Sets the team of a player.  On the server, this will work for any player, but on clients, it will only work for the current player
    /// </summary>
    /// <param name="name"></param>
    /// <param name="team"></param>
    public static void SetPlayerTeam(string name, int team) {
        if (BoltNetwork.isClient && GameManager.instance.CurrentUserName == name) {
            if (GetPlayer(name).Team == team) return;
            TeamChangeEvent evnt = TeamChangeEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.NewTeam = team;
            evnt.Send();
            return;
        }
        if (!BoltNetwork.isServer) {
            Debug.LogError("Error! cannot change the team of other players!");
            return;
        }
        LobbyPlayer player = GetPlayer(name);
        if (player.Team == team) return;
        player.Team = team;
        //send the event to the clients
        LobbyUpdateToken token = new LobbyUpdateToken();
        token.AddChange(name, LobbyUpdateToken.ChangeField.TEAM, (byte)(team & 0xFF));
        LobbyUpdateEvent uevnt = LobbyUpdateEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
        uevnt.UpdateToken = token;
        uevnt.Send();
        Sort();
        FireChangeEvent(LobbyChange.PLAYER_CHANGED);
    }

    /// <summary>
    /// SERVER ONLY
    /// Adds a player to the lobby. (Do not use for pseudoplayers!)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="team"></param>
    public static void AddPlayer(string name, int team) {
        if (!BoltNetwork.isServer) {
            Debug.LogError("Only the server can add players to the lobby!");
            return;
        }
        if (players.Exists(player => name == player.Name)) {
            Debug.LogError("Error; tried to add a player that already exists!");
        }else if(pseudoplayers.ContainsKey(name)){
            Debug.LogError("Error; tried to add a player with the same name as a pseudoplayer!");
        } else {
            players.Add(new LobbyPlayer(name, team));
            Sort();
            FireChangeEvent(LobbyChange.PLAYER_ADDED);
            PlayerConnectedEvent evnt = PlayerConnectedEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.PlayerName = name;
            evnt.Team = team;
            evnt.Send();
        }
    }

    /// <summary>
    /// SERVER ONLY
    /// removes a player from the lobby (Do not use for pseudoplayers!)
    /// </summary>
    /// <param name="name"></param>
    public static void RemovePlayer(string name) {
        if (!BoltNetwork.isServer) {
            Debug.LogError("Only the server can remove players to the lobby!");
            return;
        }
        LobbyPlayer player = GetPlayer(name);
        if (player == null) {
            Debug.LogError("Error; tried to remove a player does not exists!");
        } else {
            players.Remove(player);
            Sort();
            FireChangeEvent(LobbyChange.PLAYER_REMOVED);
            PlayerDisconnectedEvent evnt = PlayerDisconnectedEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.PlayerName = name;
            evnt.Send();
        }
    }

    /// <summary>
    /// Creates a new stat that can then be used for a player or pseudoplayer
    /// </summary>
    /// <param name="statName"></param>
    public static void AddStat(string statName) {
        int index = statNames.Count;
        statNames.Add(statName);
        statNameMap.Add(statName, (byte)index);
    }

    /// <summary>
    /// Sends the full data of the lobby to the given client.  Can only be send by the server.
    /// </summary>
    /// <param name="connection"></param>
    public static void SendFullDataToClient(BoltConnection connection) {
        if (!BoltNetwork.isServer) throw new Exception("Only the server can send out full lobby updates!");
        FullLobbyDataResponse evnt = FullLobbyDataResponse.Create(connection, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.Token = generateFullDataToken();
        evnt.Send();
    }

    /// <summary>
    /// Generates the token to be used for the FullLobbyDataResponse event
    /// </summary>
    /// <returns></returns>
    private static FullLobbyDataToken generateFullDataToken() {
        FullLobbyDataToken token = new FullLobbyDataToken();
        token.GameModeName = GameModeName;
        token.MapName = MapName;
        token.players = players;
        token.pseudoplayers = pseudoplayers;
        token.StatList = new StatListToken();
        token.StatList.Names = statNames.ToArray();
        return token;
    }

    /// <summary>
    /// Reqests a full refresh of all lobby data from the server.  Can only be called on clients
    /// </summary>
    public static void RequestFullDataFromServer() {
        if (!BoltNetwork.isClient) throw new Exception("Only clients can request full lobby updates from the server!");
        LobbyUpdateRequestEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered).Send();
    }
    
    /// <summary>
    /// Sets wether the given player is connected or not
    /// Can only be called by the server
    /// </summary>
    /// <param name="name"></param>
    /// <param name="connected"></param>
    public static void SetPlayerConnected(string name, bool connected) {
        if (!BoltNetwork.isServer) throw new Exception("Only the server can set the connected state of a player");
        LobbyPlayer player = GetPlayer(name);
        if (player == null) throw new Exception("Error; tried to set the connected state of an unknown player " + name);
        if(player.Connected != connected) {
            player.Connected = connected;
            LobbyUpdateToken token = new LobbyUpdateToken();
            token.AddChange(name, LobbyUpdateToken.ChangeField.STATUS, player.flags);
            LobbyUpdateEvent evnt = LobbyUpdateEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.UpdateToken = token;
            evnt.Send();
        }
    }

    /// <summary>
    /// Sets wether the given player is the game's host. 
    /// Can only be called by the server. 
    /// Note: there is no check to ensure that only one player is the host.  this function will only change the
    /// Host status of the given player
    /// </summary>
    /// <param name="name"></param>
    /// <param name="host"></param>
    public static void SetPlayerIsHost(string name, bool host) {
        if (!BoltNetwork.isServer) throw new Exception("Only the server can set a player's host status");
        LobbyPlayer player = GetPlayer(name);
        if (player == null) throw new Exception("Error; tried to set the host status of an unknown player " + name);
        if (player.Host != host) {
            player.Host = host;
            LobbyUpdateToken token = new LobbyUpdateToken();
            token.AddChange(name, LobbyUpdateToken.ChangeField.STATUS, player.flags);
            LobbyUpdateEvent evnt = LobbyUpdateEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.UpdateToken = token;
            evnt.Send();
        }
    }

    /// <summary>
    /// Removes all players who are marked as "disconnected".
    /// Can only be called by the server.
    /// 
    /// This will cause events to be triggered for each player removed
    /// </summary>
    public static void RemoveAllDisconnectedPlayers() {
        if (!BoltNetwork.isServer) throw new Exception("Only the server can remove players from the lobby!");
        LobbyPlayer[] toRemove = players.Where(player => !player.Connected).ToArray(); //we put it into an array to force linq to compute the full list now
        foreach (LobbyPlayer player in toRemove) {
            RemovePlayer(player.Name);
        }
    }

    #region Bolt Global Event Handlers

    public override void OnEvent(LobbyUpdateRequestEvent evnt) {
        if (!BoltNetwork.isServer) return;
        SendFullDataToClient(evnt.RaisedBy);
    }

    public override void OnEvent(StatListUpdateEvent evnt) {
        if (!BoltNetwork.isClient) return;
        StatListToken token = evnt.Token as StatListToken;
        parseStatListToken(token);
        Sort();
        FireChangeEvent(LobbyChange.STAT_LIST_CHANGED);
    }

    private static void parseStatListToken(StatListToken token) {
        statNames = new List<string>(token.Names);
        statNameMap.Clear();
        for (byte i = 0; i < statNames.Count; i++) {
            statNameMap.Add(statNames[i], i);
        }
    }

    public override void OnEvent(FullLobbyDataResponse evnt) {
        if (!BoltNetwork.isClient) return;
        FullLobbyDataToken token = evnt.Token as FullLobbyDataToken;
        _MapName = token.MapName;
        GameManager.instance.GameMode = GameModeManager.GetGameModeFromClassName(token.GameModeName);
        _GameModeName = token.GameModeName;
        players = token.players;
        pseudoplayers = token.pseudoplayers;
        parseStatListToken(token.StatList);
        Sort();
        FireChangeEvent(LobbyChange.ALL);
    }

    public override void OnEvent(LobbyUpdateEvent evnt) {
        if (!BoltNetwork.isClient) return;
        LobbyUpdateToken token = evnt.UpdateToken as LobbyUpdateToken;
        token.ApplyChangesToLobby();
        Sort();
        FireChangeEvent(LobbyChange.PLAYER_CHANGED);
    }

    public override void OnEvent(PlayerConnectedEvent evnt) {
        if (!BoltNetwork.isClient) return;
        if (players.Exists(player => evnt.PlayerName == player.Name)) {
            Debug.LogError("Error; tried to add a player that already exists!");
        } else if(pseudoplayers.ContainsKey(evnt.PlayerName)){
            Debug.LogError("Error; tried to add a player with the same name as a pseudoplayer");
        } else {
            players.Add(new LobbyPlayer(evnt.PlayerName, evnt.Team));
            Sort();
            FireChangeEvent(LobbyChange.PLAYER_ADDED);
        }
    }

    public override void OnEvent(PlayerDisconnectedEvent evnt) {
        if (!BoltNetwork.isClient) return;
        LobbyPlayer player = GetPlayer(evnt.PlayerName);
        if (player == null) {
            Debug.LogError("Error; tried to remove a player that is not in the lobby");
        } else {
            players.Remove(player);
            Sort();
            FireChangeEvent(LobbyChange.PLAYER_REMOVED);
        }

    }

    public override void OnEvent(StatUpdateEvent evnt) {
        if (!BoltNetwork.isClient) return;
        StatUpdateToken token = evnt.Token as StatUpdateToken;
        foreach (var changepair in token.Changes) {
            string name = changepair.Key;
            LobbyPlayer player = GetPlayer(name);
            if (player != null) {
                foreach (var change in changepair.Value) {
                    player.SetStat(change.Stat, change.NewValue);
                }
            } else {
                if (!pseudoplayers.ContainsKey(name)) pseudoplayers.Add(name, new Dictionary<byte, int>());
                var dict = pseudoplayers[name];
                foreach (var change in changepair.Value) {
                    dict[change.Stat] = change.NewValue;
                }
            }
        }
        Sort();
        FireChangeEvent(LobbyChange.STAT_CHANGED);
    }

    public override void OnEvent(GameModeUpdateEvent evnt) {
        if (!BoltNetwork.isClient) return;
        _GameModeName = evnt.Name;
        GameManager.instance.GameMode = GameModeManager.GetGameModeFromClassName(_GameModeName);
        FireChangeEvent(LobbyChange.MODE_CHANGED);
    }

    public override void OnEvent(MapUpdateEvent evnt) {
        if (!BoltNetwork.isClient) return;
        _MapName = evnt.Name;
        FireChangeEvent(LobbyChange.MAP_CHANGED);
    }

    #endregion Bolt Gobal Event Handlers

    private static void FireChangeEvent(LobbyChange change) {
        if (LobbyUpdatedEvent != null) LobbyUpdatedEvent(change);
    }


    #region Internal Classes

    /// <summary>
    /// This class represents a player in the lobby
    /// </summary>
    public class LobbyPlayer : Bolt.IProtocolToken{
        private const byte FLAG_CONNTECTED = 0x1;
        private const byte FLAG_HOST = 0x2;
        internal byte flags = 0;

        /// <summary>
        /// True if this player is currently connected, currently unused
        /// </summary>
        public bool Connected {
            get { return (flags & FLAG_CONNTECTED) != 0; }
            internal set {
                if (value) {
                    flags |= FLAG_CONNTECTED;
                } else {
                    flags &= (byte)((~FLAG_CONNTECTED) & 0xFF); //why do I have to do this!? this is stupid!
                }
            }
        }

        /// <summary>
        /// returns true if this player is the host
        /// </summary>
        public bool Host {
            get {
                return (flags & FLAG_HOST) != 0;
            }
            internal set {
                if (value) {
                    flags |= FLAG_HOST;
                } else {
                    flags &= (byte)((~FLAG_HOST) & 0xFF); //still stupid
                }
            }
        }
        /// <summary>
        /// This player's name
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// This player's team number (0-7)
        /// </summary>
        public int Team { get; internal set; }

        /// <summary>
        /// This player's stats.  THIS IS NOT HOW YOU SHOULD BE READING THE PLAYER'S STATS;
        /// USE Lobby.GetStatForPlayer() INSTEAD!
        /// </summary>
        internal Dictionary<byte, int> Stats;

        /// <summary>
        /// creates a new player by immediately reading it from a packet, only for internal use
        /// </summary>
        /// <param name="packet"></param>
        internal LobbyPlayer(UdpKit.UdpPacket packet) {
            Stats = new Dictionary<byte, int>();
            Read(packet);
        }

        /// <summary>
        /// makes a new player
        /// </summary>
        /// <param name="name"></param>
        /// <param name="team"></param>
        internal LobbyPlayer(string name, int team) {
            this.Name = name;
            this.Team = team;
            Connected = true;
            Stats = new Dictionary<byte, int>();
        }

        public void Read(UdpKit.UdpPacket packet) {
            this.Name = packet.ReadString();
            this.Team = packet.ReadByte();
            this.flags = packet.ReadByte();
            byte statCount = packet.ReadByte();
            for (byte i = 0; i < statCount; i++) {
                Stats.Add(packet.ReadByte(), packet.ReadInt());
            }
        }

        public void Write(UdpKit.UdpPacket packet) {
            packet.WriteString(Name);
            packet.WriteByte((byte)Team);
            packet.WriteByte(flags);
            packet.WriteByte((byte)Stats.Count);
            foreach (var pair in Stats) {
                packet.WriteByte(pair.Key);
                packet.WriteInt(pair.Value);
            }
        }

        internal void SetStat(byte statid, int newValue) {
            Stats[statid] = newValue;
        }

        internal int GetStat(byte statid) {
            if (!Stats.ContainsKey(statid)) return 0;
            return Stats[statid];
        }

        public int GetStat(string stat) {
            return GetStat(statNameMap[stat]);
        }

        internal void ClearStats() {
            Stats.Clear();
        }
    }


    /// <summary>
    /// this is the default sorter for the players in the lobby, later this will be provided by the current game mode
    /// </summary>
    private class Sorter : IComparer<LobbyPlayer> {
        public int Compare(LobbyPlayer x, LobbyPlayer y) {
            if (x.Team != y.Team) return x.Team - y.Team;
            return 0;
        }
    }

    /// <summary>
    /// An enum of different changes that can occur
    /// </summary>
    public enum LobbyChange {
        /// <summary>
        /// The entire player list was rewritten
        /// </summary>
        ALL,

        /// <summary>
        /// A new player was added to the lobby
        /// </summary>
        PLAYER_ADDED,

        /// <summary>
        /// A player was removed from the lobby
        /// </summary>
        PLAYER_REMOVED,

        /// <summary>
        /// A player's name or team was changed
        /// </summary>
        PLAYER_CHANGED,

        /// <summary>
        /// The list of stats has changed
        /// </summary>
        STAT_LIST_CHANGED,

        /// <summary>
        /// The value of a stat for a player has changed
        /// </summary>
        STAT_CHANGED,

        /// <summary>
        /// The current game mode has changed
        /// </summary>
        MODE_CHANGED,

        /// <summary>
        /// The current map has changed
        /// </summary>
        MAP_CHANGED,
        

    }

    #endregion

    //here are some useful predefined pseudoplayer names
    public const string PP_PREFIX = "@";
    public const string PP_GLOBAL = PP_PREFIX+"global";
    public const string PP_TEAM_PREFIX = PP_PREFIX + "team_";
    public static string[] PP_TEAMS = { 
        PP_TEAM_PREFIX + "0", 
        PP_TEAM_PREFIX + "1", 
        PP_TEAM_PREFIX + "2", 
        PP_TEAM_PREFIX + "3", 
        PP_TEAM_PREFIX + "4", 
        PP_TEAM_PREFIX + "5", 
        PP_TEAM_PREFIX + "6", 
        PP_TEAM_PREFIX + "7" };
   
}
