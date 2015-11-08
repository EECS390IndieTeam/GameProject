using UnityEngine;
using System.Collections.Generic;
using UdpKit;

public class FullLobbyDataToken : Bolt.IProtocolToken {
    public StatListToken StatList;
    public List<Lobby.LobbyPlayer> players = new List<Lobby.LobbyPlayer>();
    public Dictionary<string, Dictionary<byte, int>> pseudoplayers = new Dictionary<string, Dictionary<byte, int>>();
    

    public void Read(UdpPacket packet) {
        //read the players
        int count = packet.ReadByte();
        players = new List<Lobby.LobbyPlayer>(count);
        for (int i = 0; i < count; i++) {
            players.Add(new Lobby.LobbyPlayer(packet));
        }

        //read the stat list
        StatList = new StatListToken();
        StatList.Read(packet);

        //read the pseudoplayers
        byte pspcount = packet.ReadByte();
        for (byte i = 0; i < pspcount; i++) {
            string name = packet.ReadString(); 
            if (!pseudoplayers.ContainsKey(name)) pseudoplayers.Add(name, new Dictionary<byte, int>());
            byte statcount = packet.ReadByte();
            for (byte s = 0; s < statcount; s++) {
                pseudoplayers[name].Add(packet.ReadByte(), packet.ReadInt());
            }
        }
    }

    public void Write(UdpPacket packet) {
        //write the players
        packet.WriteByte((byte)players.Count);
        foreach (Lobby.LobbyPlayer p in players) {
            p.Write(packet);
        }
        //write the statList
        if (StatList != null) StatList.Write(packet);

        //write the pseudoplayers
        packet.WriteByte((byte)pseudoplayers.Count);
        foreach (var player in pseudoplayers) {
            packet.WriteString(player.Key);
            packet.WriteByte((byte)player.Value.Count);
            foreach (var stat in player.Value) {
                packet.WriteByte(stat.Key);
                packet.WriteInt(stat.Value);
            }
        }
    }
}
