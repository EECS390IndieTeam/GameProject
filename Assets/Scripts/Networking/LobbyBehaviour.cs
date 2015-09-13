using UnityEngine;
using System.Collections;

public class LobbyBehaviour : Bolt.EntityBehaviour<ILobbyObject> {

    public static ILobbyObject CurrentLobby {
        get;
        private set;
    }

    public override void Attached() {
        //UpdateLobby();
        CurrentLobby = state;
    }

    void UpdateLobby() {
        Debug.Log("Updating lobby!");
        int i = 0;
        foreach (BoltPlayer p in PlayerRegistry.AllPlayers) {
            state.PlayerList[i].ConnectionName = p.ConnectionName;
            state.PlayerList[i].Team = p.Team;
            state.PlayerList[i].UserName = p.Name;
            state.PlayerList[i].Connected = true;
            i++;
        }
        state.PlayerCount = i;
        for (int n = i; n < state.PlayerList.Length; n++) {
            state.PlayerList[n].Connected = false;
        }
    }

    void OnGUI() {
        GUILayout.BeginArea(new Rect(10,10,500,500), "connected players", GUI.skin.box);
        GUILayout.BeginVertical();
        GUILayout.Space(20f);
        foreach (PlayerEntry p in state.PlayerList) {
            //GUILayout.BeginHorizontal();
            GUILayout.Label(p.Connected ? p.UserName + "Team: " + p.Team : "Disconnected");
            //GUILayout.EndHorizontal();
        }
        //DrawTeamChangeButtons();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }


    private void DrawTeamChangeButtons() {
        PlayerEntry me = FindMe();
        if (me == null) {
            Debug.Log("Could not find me!");
            return;
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-")) {
            me.Team--;
        }

        GUILayout.Label("Team");

        if (GUILayout.Button("+")) {
            me.Team++;
        }
        GUILayout.EndHorizontal();
    }

    private PlayerEntry FindMe() {
        for (int i = 0; i < state.PlayerCount; i++) {
            if (state.PlayerList[i].UserName == BoltPlayer.LocalPlayer.Name) {
                return state.PlayerList[i];
            }
        }
        return null;
    }
}
