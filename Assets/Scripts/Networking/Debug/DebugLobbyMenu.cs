using UnityEngine;
using System.Collections;

public class DebugLobbyMenu : Bolt.GlobalEventListener {

    private string tempPassword = ServerSideData.Password;

    private float width = 300f;

    private const int SERVER_MENU_LINE_COUNT = 6;
    private const int CLIENT_MENU_LINE_COUNT = 3;

    private const float LINE_HEIGHT = 27.3f;

    private int selectedGameMode = 0;
    void Update() {
        DebugHUD.setValue("IsSever", BoltNetwork.isServer);
        if (BoltNetwork.isClient) {
            DebugHUD.setValue("ping", BoltNetwork.server.PingNetwork);
        }
        DebugHUD.setValue("username", GameManager.instance.CurrentUserName);
    }

    void OnGUI() {
        if(!BoltNetwork.isRunning){
            Application.LoadLevel(1);
            return;
        }

        
        if (BoltNetwork.isClient) {
            StartBox(CLIENT_MENU_LINE_COUNT * LINE_HEIGHT);
            DrawClientMenu();
        } else {
            StartBox((SERVER_MENU_LINE_COUNT + GameModeManager.GameModes.Length) * LINE_HEIGHT);
            DrawServerMenu();
        }
        EndBox();
    }

    private void DrawServerMenu() {
        GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));
        GUILayout.Label("Lobby Password:", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        tempPassword = GUILayout.TextField(tempPassword, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUI.enabled = tempPassword != ServerSideData.Password;
        if (GUILayout.Button("Apply new Password")) {
            ServerSideData.Password = tempPassword;
            ServerSideData.UpdateZeusData();
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Game mode:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        selectedGameMode = GUILayout.SelectionGrid(selectedGameMode, GameModeManager.GameModeNames, 1);
        GameManager.instance.gameMode = GameModeManager.GameModes[selectedGameMode];
        GUILayout.EndHorizontal();


        DrawTeamChangeButtons();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start Game")) {
            BoltNetwork.LoadScene("ingame");
        }
        GUILayout.EndHorizontal();
    }

    private void DrawClientMenu() {
        DrawTeamChangeButtons();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Disconnect")) {
            BoltNetwork.server.Disconnect();
        }
        GUILayout.EndHorizontal();
    }

    private void StartBox(float height) {
        GUILayout.BeginArea(new Rect(Screen.width - width - 10, (Screen.height - height)/2f, width, height), GUI.skin.box);
        GUILayout.BeginVertical();
    }
    private void EndBox() {
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawTeamChangeButtons() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Set team:");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        for (int i = 0; i <= 7; i++) {
            if (GUILayout.Button(i + "")) {
                TeamChangeEvent evnt = TeamChangeEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
                evnt.NewTeam = i;
                evnt.Send();
            }
        }
        GUILayout.EndHorizontal();
    }

    public override void Disconnected(BoltConnection connection) {
        Bolt.IProtocolToken token = connection.DisconnectToken;
        if (token != null && token is DisconnectReason) {
            DisconnectReason reason = (DisconnectReason)token;
            Debug.Log("Disconnected from server: "+ reason.Reason + (reason.Message=="" ? "" : ": "+reason.Message));
        }
    }
}
