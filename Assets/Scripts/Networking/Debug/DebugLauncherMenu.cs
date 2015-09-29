using UnityEngine;
using System.Collections;

public class DebugLauncherMenu : MonoBehaviour {

    //how wide the window is, its height will be set automatically
    public float WindowWidth = 200f;


    //these are used for some of the text fields
    private string ip = "127.0.0.1:54321";
    private string password = "";
    private string port = "54321";

    //used for the loading animation
    private int loadingDots = 0;

    //the states that this ui can be in
    private enum State : byte {
        INIT = 0,           //first screen the user sees
        SERVER_SETUP,       //setup the server prior to launching it
        LAUNCH_SERVER,      //loading screen while we wait for bolt to start up the server
        LAUNCH_CLIENT,      //loading screen while we wait for bolt to start up the client
        CLIENT              //after bolt starts the client, configure the client's properties and decide who to connect to
    }

    //the height of each line
    private const float lineHeight = 27.3f; //for some reason adding 0.3 makes the menu look less blury

    //this array is the number of lines the window needs to have room for for the corresponding state
    private static int[] stateLineCounts = { 2, 5, 1, 1, 5 };

    //keeps track of the current state
    private State state = State.INIT;

    void OnGUI() {
        StartBox(stateLineCounts[(int)state] * lineHeight);
        switch (state) {
            case State.INIT:
                DrawSelectionMenu();
                break;
            case State.LAUNCH_CLIENT:
                if (BoltNetwork.isRunning) {
                    state = State.CLIENT;
                } else {
                    DrawLoadingMessage();
                }
                break;
            case State.CLIENT:
                DrawClientMenu();
                //if (BoltNetwork.isConnected) Destroy(this.gameObject);
                break;
            case State.SERVER_SETUP:
                DrawServerSetupMenu();
                break;
            case State.LAUNCH_SERVER:
                if (BoltNetwork.isRunning) {
                    BoltNetwork.LoadScene(BoltScenes.lobby);
                } else {
                    DrawLoadingMessage();
                }
                break;
        }
        EndBox();
    }

    //draws the menu to set the server properties prior to launching it
    private void DrawServerSetupMenu() {
        FlexLabel("Server Setup");
        DrawFieldLine("Port", ref port);
        DrawFieldLine("Lobby Password", ref ServerConnectionEventListener.ServerPassword);
        DrawFieldLine("Username", ref GameManager.instance.CurrentUserName);
        GUILayout.BeginHorizontal();
        int parsedPort;
        GUI.enabled = int.TryParse(port, out parsedPort);
        if (GUILayout.Button("Launch")) {
            BoltLauncher.StartServer(parsedPort);
            state = State.LAUNCH_SERVER;
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        
    }

    //draws a centered label with the given text
    private void FlexLabel(string label) {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(label);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    //draws a single-line text field for the given variable, data, and labels it with label
    private void DrawFieldLine(string label, ref string data) {
        GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));
        GUILayout.Label(label, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        data = GUILayout.TextField(data, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
        GUILayout.EndHorizontal();
    }

    //draws a simple loading message
    private void DrawLoadingMessage() {
        string message = "Please wait";
        for (int i = 0; i < loadingDots; i++) {
            message += ".";
        }
        GUILayout.FlexibleSpace();
        FlexLabel(message);
        GUILayout.FlexibleSpace();
    }

    //draws the menu to specify what server to connect to
    private void DrawClientMenu() {
        FlexLabel("Connect");
        DrawFieldLine("IP", ref ip);
        DrawFieldLine("Username", ref GameManager.instance.CurrentUserName);
        DrawFieldLine("Password", ref password);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Connect")) {
            if (!BoltNetwork.isRunning) {
                BoltLauncher.StartClient();
            } else {
                BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(ip), new ConnectionRequestData(GameManager.instance.CurrentUserName, password));
            }
        }
        GUILayout.EndHorizontal();
    }

    //draws the initial menu that just asks if you want to launch as a server or client
    private void DrawSelectionMenu() {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Host Game")) {
            state = State.SERVER_SETUP;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Join Game")) {
            state = State.LAUNCH_CLIENT;
            BoltLauncher.StartClient();
        }
        GUILayout.EndHorizontal();
    }

    //starts a window with the given height
    private void StartBox(float height) {
        GUILayout.BeginArea(new Rect((Screen.width - WindowWidth) / 2f, (Screen.height - height) / 2f, WindowWidth, height), GUI.skin.box);
        GUILayout.BeginVertical();
    }

    //ends the window
    private void EndBox() {
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void Update() {
        if (state == State.LAUNCH_CLIENT || state == State.LAUNCH_SERVER) {
            loadingDots++;
            if (loadingDots > 3) loadingDots = 0;
        }
    }
}
