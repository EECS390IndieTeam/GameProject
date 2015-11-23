using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LobbyCornerText : MonoBehaviour {
    public int frameSkip = 10;
    private int timer = 0;
    private Text text;

	// Use this for initialization
	void Start () {
        this.text = GetComponent<Text>();

	}
	
	// Update is called once per frame
	void Update () {
        if (!BoltNetwork.isRunning) return;
        if (timer < frameSkip) {
            timer++;
            return;
        }
        timer = 0;
        if (BoltNetwork.isServer) {
            try {
                text.text = "IP Address: " + System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[0];
            } catch {
                text.text = "IP Address Could not be Determined";
            }
        } else {
            BoltConnection server = BoltNetwork.server;
            if (server != null) {
                text.text = "Ping: " + (int)(server.PingNetwork * 100f) + "ms";
            }
	}
        }
}
