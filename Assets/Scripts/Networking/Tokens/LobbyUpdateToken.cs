using UnityEngine;
using System.Collections.Generic;
using System;
using UdpKit;

/// <summary>
/// This Token allows for serializing a list of changes that may have occured for the Lobby
/// </summary>
public class LobbyUpdateToken : Bolt.IProtocolToken {

    private List<Change> changes = new List<Change>();

    public void ApplyChangesToLobby() {
        foreach (Change c in changes) {
            switch (c.Field) {
                case ChangeField.NAME:
                    Lobby.GetPlayer(c.Name).Name = (string)c.NewValue;
                    break;
                case ChangeField.TEAM:
                    Lobby.GetPlayer(c.Name).Team = (int)c.NewValue;
                    break;
                case ChangeField.STATUS:
                    Lobby.GetPlayer(c.Name).flags = (byte)c.NewValue;
                    break;
                default:
                    throw new Exception("Error: Invalid change requested in LobbyUpdateToken!");
            }
        }
    }

    public void AddChange(string player, ChangeField field, object newValue) {
        if (changes.Count >= 256) throw new Exception("Error, cannot send more than 256 changes in a single LobbyUpdateToken");
        changes.Add(new Change(player, field, newValue));
    }


    public void Read(UdpPacket packet) {
        byte changeCount = packet.ReadByte();
        for (int i = 0; i < changeCount; i++) {
            changes.Add(readChange(packet));
        }
    }

    private Change readChange(UdpPacket packet) {
        string name = packet.ReadString();
        ChangeField field = (ChangeField)packet.ReadByte();
        object newValue = null;
        switch (field) {
            case ChangeField.NAME:
                newValue = packet.ReadString();
                break;
            case ChangeField.TEAM:
            case ChangeField.STATUS:
                newValue = packet.ReadByte();
                break;
            default:
                throw new Exception("Fatal Error: failed to read LobbyUpdateToken!");
        }
        return new Change(name, field, newValue);
    }

    public void Write(UdpPacket packet) {
        packet.WriteByte((byte)changes.Count);
        foreach (Change c in changes) {
            WriteChange(c, packet);
        }
    }

    private void WriteChange(Change c, UdpPacket packet) {
        packet.WriteString(c.Name);
        packet.WriteByte((byte)c.Field);
        switch (c.Field) {
            case ChangeField.NAME:
                packet.WriteString((string)c.NewValue);
                break;
            case ChangeField.TEAM:
            case ChangeField.STATUS:
                packet.WriteByte((byte)c.NewValue);
                break;
            default:
                throw new Exception("Error: failed to write LobbyUpdateToken!");
        }
    }


    private class Change {
        public string Name;
        public ChangeField Field;
        public object NewValue;
        public Change(string name, ChangeField field, object newValue) {
            this.Name = name;
            this.Field = field;
            this.NewValue = newValue;
        }
    }

    public enum ChangeField : byte{
        NAME,   //change the player's name
        TEAM,   //change the player's team
        STATUS  //change wether the player is connected or not
    }
}
