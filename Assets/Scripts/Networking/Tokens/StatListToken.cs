using UnityEngine;
using System.Collections;

/// <summary>
/// This token is for sending the list of statistic names
/// </summary>
public class StatListToken : Bolt.IProtocolToken {
    public string[] Names;

    public void Read(UdpKit.UdpPacket packet) {
        int count = packet.ReadByte();
        Names = new string[count];
        for (int i = 0; i < count; i++) {
            Names[i] = packet.ReadString();
        }
    }

    public void Write(UdpKit.UdpPacket packet) {
        if (Names == null) return;
        packet.WriteByte((byte)Names.Length);
        foreach (string name in Names) {
            packet.WriteString(name);
        }
    }
}
