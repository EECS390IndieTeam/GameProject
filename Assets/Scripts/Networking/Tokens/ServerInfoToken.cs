using System.Collections;

public class ServerInfoToken : Bolt.IProtocolToken {
    public string ServerName { get; set; }
    public string MOTD { get; set; }
    public string GameMode { get; set; }
    public string MapName { get; set; }
    public byte PlayerCount { get; set; }
    public byte MaxPlayerCount { get; set; }
    public bool PasswordRequired { get; set; }
    public bool IsDedicatedServer { get; set; }
    public bool HideInServerList { get; set; }

    private const byte PASSWORD_REQUIRED_MASK = 0x01;
    private const byte DEDICATED_MASK = 0x02;
    private const byte HIDE_MASK = 0x04;
    public void Read(UdpKit.UdpPacket packet) {
        ServerName = packet.ReadString();
        MOTD = packet.ReadString();
        GameMode = packet.ReadString();
        MapName = packet.ReadString();
        PlayerCount = packet.ReadByte();
        MaxPlayerCount = packet.ReadByte();
        byte flags = packet.ReadByte();
        PasswordRequired = (flags & PASSWORD_REQUIRED_MASK) != 0;
        IsDedicatedServer = (flags & DEDICATED_MASK) != 0;
        HideInServerList = (flags & HIDE_MASK) != 0;
    }

    public void Write(UdpKit.UdpPacket packet) {
        packet.WriteString(ServerName);
        packet.WriteString(MOTD);
        packet.WriteString(GameMode);
        packet.WriteString(MapName);
        packet.WriteByte(PlayerCount);
        packet.WriteByte(MaxPlayerCount);
        byte flags = 0;
        if (PasswordRequired) flags |= PASSWORD_REQUIRED_MASK;
        if (IsDedicatedServer) flags |= DEDICATED_MASK;
        if (HideInServerList) flags |= HIDE_MASK;
        packet.WriteByte(flags);
    }
}
