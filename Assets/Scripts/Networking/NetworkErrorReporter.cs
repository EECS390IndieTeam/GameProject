using UnityEngine;
using System.Collections;

public class NetworkErrorReporter : Bolt.GlobalEventListener {
    const int windowHeight = 150;
    const int windowWidth = 300;
    string messageBody;
    static NetworkErrorReporter instance = null;

    private bool messageDisplayed = false;
    private string title = "Error";

    void Start() {
        if (instance != null) DestroyImmediate(this.gameObject);
        instance = this;
        DontDestroyOnLoad(this);
    }


    public override void BoltStartFailed() {
        title = "Bolt Error";
        messageBody = "Bolt failed to start!";
        messageDisplayed = true;
    }

    public override void Disconnected(BoltConnection connection) {
        if (BoltNetwork.isServer) return;
        if (connection.DisconnectToken != null && connection.DisconnectToken is DisconnectReason) {
            title = "Disconnected from server";
            DisconnectReason reason = (DisconnectReason)connection.DisconnectToken;
            messageBody = reason.Reason + (reason.Message == "" ? "" : ": " + reason.Message);
            messageDisplayed = true;
        } else {
            title = "Disconnected from server";
            messageBody = "Connection closed";
            messageDisplayed = true;
        }
        Application.LoadLevel(0);
    }

    public override void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
        title = "Connection Refused";
        if (token != null && token is DisconnectReason) {
            DisconnectReason reason = (DisconnectReason)token;
            messageBody = reason.Reason + (reason.Message == "" ? "" : ": " + reason.Message);
        } else {
            messageBody = "Unknown Error";
        }
        messageDisplayed = true;
        
    }


    void OnGUI() {
        if (!messageDisplayed) return;
        Rect box = new Rect(Screen.width/2 - windowWidth/2, Screen.height/2 - windowHeight/2, windowWidth, windowHeight);
        GUI.ModalWindow(0, box, DrawWindow, title);
    }

    void DrawWindow(int id) {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label(messageBody, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        GUILayout.EndVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK")) {
            messageDisplayed = false;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
}
