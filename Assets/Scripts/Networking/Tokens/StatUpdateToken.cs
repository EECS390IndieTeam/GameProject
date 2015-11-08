using UnityEngine;
using System.Collections.Generic;
using System;

public class StatUpdateToken : Bolt.IProtocolToken {

    public Dictionary<string,List<Change>> Changes = new Dictionary<string,List<Change>>();

    public void AddChange(string player, byte stat, int value) {
        if (Changes.Count >= 256) throw new Exception("Error, cannot send more than 256 players in a single StatUpdateToken");
        if (!Changes.ContainsKey(player)) Changes.Add(player, new List<Change>());
        List<Change> list = Changes[player];
        if (list.Count >= 256) throw new Exception("Error, cannot send more than 256 changes per player in a single StatUpdateToken");
        list.Add(new Change(stat, value));
    }

    public void Read(UdpKit.UdpPacket packet) {
        int playerCount = packet.ReadByte();
        for (int p = 0; p < playerCount; p++) {
            string name = packet.ReadString();
            int statCount = packet.ReadByte();
            for (int i = 0; i < statCount; i++) {
                byte statId = packet.ReadByte();
                int statValue = packet.ReadInt();
                AddChange(name, statId, statValue);
            }
        }
    }

    public void Write(UdpKit.UdpPacket packet) {
        packet.WriteByte((byte)Changes.Count);
        foreach (var pair in Changes) {
            packet.WriteString(pair.Key);
            packet.WriteByte((byte)pair.Value.Count);
            foreach (Change c in pair.Value) {
                packet.WriteByte(c.Stat);
                packet.WriteInt(c.NewValue);
            }
        }
    }

    public class Change {
        public byte Stat { get; private set; }
        public int NewValue { get; private set; }
        public Change(byte stat, int newValue) {
            this.Stat = stat;
            this.NewValue = NewValue;
        }
    }
}
