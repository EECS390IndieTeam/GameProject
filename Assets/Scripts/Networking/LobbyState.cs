using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LobbyState : Bolt.EntityBehaviour<ILobbyObject> {

    private const int DEFAULT_TEAM = 0;

    public IEnumerable<PlayerEntry> ConnectedPlayers {
        get { return state.PlayerList.Where<PlayerEntry>(e => e.Connected); }
    }

    public int PlayerCount {
        get { return state.PlayerCount; }
    }

    public PlayerEntry this[int i] {
        get { return state.PlayerList[i]; }
    }

    public void AddPlayer(string username, int team = DEFAULT_TEAM) {
        SetPlayer(state.PlayerCount, username, team, true);
        state.PlayerCount++;
    }

    public bool HideDebugDraw = false;

    public void RemovePlayer(string username) {
        Debug.Log("removing player " + username);
        bool found = false;
        for (int i = 0; i < state.PlayerCount; i++) {
            if (!found && state.PlayerList[i].UserName == username) {
                Debug.Log("found player at index " + i);
                found = true;
                ClearPlayer(i);
            } else if (found) {
                ClearPlayer(i);
                CopyPlayer(i, i - 1);

            }
        }
        state.PlayerCount--;
    }

    public void SetPlayerTeam(string username, int team) {
        for (int i = 0; i < PlayerCount; i++) {
            if (state.PlayerList[i].UserName == username) {
                state.PlayerList[i].Team = team;
                return;
            }
        }
        Debug.LogWarning("Tried to change the team of a nonexistent player: " + username);
    }

    private void CopyPlayer(int from, int to) {
        Debug.Log("moved player at " + from + " to " + to);
        PlayerEntry p = state.PlayerList[from];
        SetPlayer(to, p.UserName, p.Team, p.Connected);
    }

    private void SetPlayer(int index, string username, int team, bool connected = true) {
        PlayerEntry p = state.PlayerList[index];
        p.UserName = username;
        p.Team = team;
        p.Connected = connected;
    }

    private void ClearPlayer(int i) {
        SetPlayer(i, "", DEFAULT_TEAM, false);
    }

    public void InitializeLobby() {
        Debug.Log("initalizing lobby!");
        int i = 0;
        foreach (string p in PlayerRegistry.PlayerNames) {
            SetPlayer(i, p, DEFAULT_TEAM, true);
            i++;
        }
        state.PlayerCount = i;
        for (int n = i; n < state.PlayerList.Length; n++) {
            ClearPlayer(n);
        }
    }

    void OnGUI() {
        if (HideDebugDraw) return;
        GUILayout.BeginArea(new Rect(10,10,500,500), "connected players", GUI.skin.box);
        GUILayout.BeginVertical();
        GUILayout.Space(20f);
        foreach (PlayerEntry p in state.PlayerList) {
            //GUILayout.BeginHorizontal();
            GUILayout.Label(p.Connected ? p.UserName + "Team: " + p.Team : "Disconnected");
            //GUILayout.EndHorizontal();
        }
        DrawTeamChangeButtons();
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
}
