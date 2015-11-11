using UnityEngine;
using System.Collections;

public class DebugGoHome : MonoBehaviour {
    public float Width, Height;
    void OnGUI() {
        GUILayout.BeginArea(new Rect((Screen.width - Width) / 2, (Screen.height - Height) / 2, Width, Height), GUI.skin.box);

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("You shouldn't be here;\n We haven't made this scene yet;\nGo back to the main menu.");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Leave this place")) {
            GameManager.instance.ChangeGameState(GameManager.GameState.MENU);
            if (BoltNetwork.isRunning) {
                if (BoltNetwork.isClient) {
                    BoltNetwork.server.Disconnect();
                } else {
                    foreach (var c in BoltNetwork.connections) {
                        c.Disconnect();
                    }
                }
                BoltLauncher.Shutdown();
            }
            Application.LoadLevel(0);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
