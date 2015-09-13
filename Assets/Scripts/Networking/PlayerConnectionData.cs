﻿//an instance of this class will be sent to the server with each connection request
public class ConnectionRequestData : Bolt.IProtocolToken{
    public string PlayerName;
    public string Password;

    public void Read(UdpKit.UdpPacket packet) {
        PlayerName = packet.ReadString();
        Password = packet.ReadString();
    }

    public void Write(UdpKit.UdpPacket packet) {
        packet.WriteString(PlayerName);
        packet.WriteString(Password);
    }
}
