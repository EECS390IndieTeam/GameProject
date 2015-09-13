using UnityEngine;
using System.Collections;

public class DebugClientConnectMenu : Bolt.GlobalEventListener {

    private string username = "";
    private string password = "dickbutt";
    private string ip = "127.0.0.1:54321";

	// Use this for initialization
	void Start () {
        //if(!BoltNetwork.isRunning)
          //  BoltLauncher.StartClient();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        if (BoltNetwork.isRunning) {
            if (BoltNetwork.isClient) {
                DrawClientMenu();
            } else {
                DrawServerMenu();
            }
        } else {
            DrawLaunchMenu();
        }
    }

    private void DrawLaunchMenu() {
        StartBox();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Launch Server")) {
            BoltLauncher.StartServer();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Launch Client")) {
            BoltLauncher.StartClient();
        }
        GUILayout.EndHorizontal();
        EndBox();
    }

    private void DrawServerMenu() {
        StartBox();
        GUILayout.BeginHorizontal();
        GUILayout.Label("server password:");
        ServerLobbyGlobalEventListener.ServerPassword = GUILayout.TextField(ServerLobbyGlobalEventListener.ServerPassword);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start")) {
            BoltNetwork.LoadScene("ingame");
        }
        GUILayout.EndHorizontal();
        EndBox();
    }

    private void DrawClientMenu() {
        StartBox();
        GUILayout.BeginHorizontal();
        GUILayout.Label("IP:");
        ip = GUILayout.TextField(ip);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Username:");
        username = GUILayout.TextField(username);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("server password:");
        password = GUILayout.TextField(password);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Connect")) {
            if (!BoltNetwork.isRunning) BoltLauncher.StartClient();
            ConnectionRequestData data = new ConnectionRequestData();
            data.PlayerName = username;
            data.Password = password;
            BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(ip), data);
            BoltPlayer.LocalPlayer.Name = username;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Disconnect")) {
            BoltNetwork.server.Disconnect();
        }
        GUILayout.EndHorizontal();
        EndBox();
    }

    private void StartBox() {
        GUILayout.BeginArea(new Rect(600, 20, 300, 500), "connect", GUI.skin.box);
        GUILayout.BeginVertical();
        GUILayout.Space(15f);
    }
    private void EndBox() {
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public override void Disconnected(BoltConnection connection) {
        Bolt.IProtocolToken token = connection.DisconnectToken;
        if (token != null && token is DisconnectReason) {
            DisconnectReason reason = (DisconnectReason)token;
            Debug.Log("Disconnected from server: "+ reason.Reason + (reason.Message=="" ? "" : ": "+reason.Message));
        }
    }
}
