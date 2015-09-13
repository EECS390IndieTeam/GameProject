using UnityEngine;
using System.Collections;

public class DisconnectReason : Bolt.IProtocolToken {
    public string Reason, Message;

    public void Read(UdpKit.UdpPacket packet) {
        Reason = packet.ReadString();
        Message = packet.ReadString();
    }

    public void Write(UdpKit.UdpPacket packet) {
        packet.WriteString(Reason);
        packet.WriteString(Message);
    }
}
