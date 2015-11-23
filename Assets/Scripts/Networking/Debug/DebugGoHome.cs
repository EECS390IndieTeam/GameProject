using UnityEngine;
using System.Collections;

public class DebugGoHome : MonoBehaviour {
    public float Width, Height;
    private bool pressed = false;
    void OnGUI() {
        GUILayout.BeginArea(new Rect((Screen.width - Width) / 2, (Screen.height - Height) / 2, Width, Height), GUI.skin.box);

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("We haven't made this scene yet;\nGo back to the main menu.");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (pressed) GUI.enabled = false;
        if (GUILayout.Button("Leave this place")) {
            pressed = true;
            if (BoltNetwork.isRunning) BoltLauncher.Shutdown();
        }
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void Update() {
        if (!pressed) return;
        if (BoltNetwork.isRunning) return;
        Application.LoadLevel(0);
        GameManager.instance.ChangeGameState(GameManager.GameState.MENU);
    }
}
