using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//this class manages each entry in the server list
public class EntryPanel : MonoBehaviour {
    public Text serverNameText, motdText, playerCountText, mapText, modeText, dedicatedText;
    public Button button;
    public UdpKit.UdpSession session;
    public PasswordPanel passwordPanel;
    private ServerInfoToken token;

	// Use this for initialization
	void Start () {
        if (session == null) return;
        token = (ServerInfoToken)session.GetProtocolToken();
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
            if (token.PasswordRequired) {
                passwordPanel.Pop(session);
            } else {
                BoltNetwork.Connect(session, new ConnectionRequestData(GameManager.instance.CurrentUserName));
            }
        }));

        serverNameText.text = token.ServerName;
        motdText.text = token.MOTD;

        string pct = token.PlayerCount + " Of " + token.MaxPlayerCount;
        if (token.PlayerCount >= token.MaxPlayerCount) pct = "<color=red>" + pct + "</color>";
        playerCountText.text = pct;
        mapText.text = token.MapName;
        modeText.text = token.GameMode;
        dedicatedText.text = "";
        if (token.IsDedicatedServer) dedicatedText.text += "[Dedicated]";
        if (token.PasswordRequired) dedicatedText.text += "[Password]";
	}
}
