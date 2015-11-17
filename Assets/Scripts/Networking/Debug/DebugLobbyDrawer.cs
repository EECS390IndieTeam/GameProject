using UnityEngine;
using System.Collections;

public class DebugLobbyDrawer : MonoBehaviour {
    private GUIStyle boxSkin;

    private const float HEIGHT = 445f;
    private const float WIDTH = 200f;

    void OnGUI() {
        if (boxSkin == null || boxSkin.name == "") {
            boxSkin = new GUIStyle(GUI.skin.box);
            boxSkin.margin.top = -3;
            boxSkin.margin.bottom = -3;
            boxSkin.overflow.top = -3;
            boxSkin.overflow.bottom = -3;
        }
        GUILayout.BeginArea(new Rect(10, (Screen.height - HEIGHT) / 2f, WIDTH, HEIGHT), "Players", GUI.skin.box);
        GUILayout.BeginVertical();
        GUILayout.Space(20f);
        foreach (Lobby.LobbyPlayer p in Lobby.AllPlayers) {
            GUILayout.BeginHorizontal(boxSkin);
            Color retColor = GUI.contentColor;
            GUI.contentColor = p.Connected ? Teams.Colors[p.Team] : Color.grey;
            GUILayout.Label((GameManager.instance.GameMode.UsesTeams ? p.Team + " - " : "") + p.Name + (p.Host ? " [H]" : ""));
            GUI.contentColor = retColor;
            GUILayout.EndHorizontal();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
