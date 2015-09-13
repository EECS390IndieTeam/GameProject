using UnityEngine;
using System.Collections;

public class BoltPlayer {
    public string Name;
    public string ConnectionName;
    public BoltConnection Connection;
    public BoltEntity Entity;
    public int Team;

    public bool IsServer {
        get {
            return Connection == null;
        }
    }

    public static BoltPlayer LocalPlayer = new BoltPlayer();
}
