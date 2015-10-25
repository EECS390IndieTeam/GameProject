using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LobbyState : Bolt.EntityBehaviour<ILobbyObject> {

    private const int DEFAULT_TEAM = 0;

    private GUIStyle boxSkin;

    private const float HEIGHT = 445f;
    private const float WIDTH = 200f;

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

    public IDictionary<string, int> GetTeamLookup() {
        return state.PlayerList.Where(x => x.Connected).ToDictionary(e => e.UserName, e => e.Team);
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

    public void SetPlayerStatIndex(string name, int index) {
        for (int i = 0; i < PlayerCount; i++) {
            if (state.PlayerList[i].UserName == name) {
                state.PlayerList[i].StatIndex = index;
                return;
            }
        }
        Debug.LogError("Tried to change the stat index of a nonexistent player: "+name);
    }

    public void InitializeLobby() {
        Debug.Log("initalizing lobby!");
        int i = 0;
        foreach (string p in PlayerRegistry.PlayerNames) {
            SetPlayer(i, p, DEFAULT_TEAM, true);
            state.PlayerList[i].StatIndex = ServerConnectionEventListener.IndexMap.GetIndexForPlayer(p);
            i++;
        }
        state.PlayerCount = i;
        for (int n = i; n < state.PlayerList.Length; n++) {
            ClearPlayer(n);
        }
    }

    public override void Attached() {
        GameManager.instance.Lobby = this;
        if(BoltNetwork.isServer) ServerSideData.UpdateZeusData();
    }

    void OnGUI() {
        if (HideDebugDraw) return;
        if (boxSkin == null || boxSkin.name == "") {
            boxSkin = new GUIStyle(GUI.skin.box);
            boxSkin.margin.top = -3;
            boxSkin.margin.bottom = -3;
            boxSkin.overflow.top = -3;
            boxSkin.overflow.bottom = -3;
        }
        GUILayout.BeginArea(new Rect(10, (Screen.height - HEIGHT) / 2f, WIDTH, HEIGHT), "Connected Players", GUI.skin.box);
        GUILayout.BeginVertical();
        GUILayout.Space(20f);
        foreach (PlayerEntry p in state.PlayerList) {
            GUILayout.BeginHorizontal(boxSkin);
            Color retColor = GUI.contentColor;
            GUI.contentColor = p.Connected ? teamColors[p.Team] : Color.grey;
            GUILayout.Label(p.Connected ? p.Team + " - " + p.UserName: "[Waiting for player...]");
            GUI.contentColor = retColor;
            GUILayout.EndHorizontal();
        }
        //GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private Color[] teamColors = {
                                     Color.white,                               //team 0
                                     Color.red,                                 //team 1
                                     Color.blue,                                //team 2
                                     Color.green,                               //team 3
                                     Color.yellow,                              //team 4
                                     Color.Lerp(Color.red,Color.yellow, 0.5f),  //team 5
                                     Color.Lerp(Color.red, Color.white, 0.5f),  //team 6
                                     Color.Lerp(Color.red, Color.blue, 0.5f)    //team 7
                                 };

    internal int GetStatIndexForPlayer(string userName) {
        for (int i = 0; i < PlayerCount; i++) {
            var player = state.PlayerList[i];
            if (player.Connected && player.UserName == userName) {
                return player.StatIndex;
            }
        }
        return -1;
    }
}
