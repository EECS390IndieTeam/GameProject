using UnityEngine;
using System.Collections;
using Bolt;
using UdpKit;

public class NetBoltMenuEvents : Bolt.GlobalEventListener
{
    public Canvas netConnectingCanvas;
    public Canvas netErrorCanvas;
    public Canvas lobbyCanvas;

    private bool dead;

    public void ConnectionAttempt()
    {
        if (!this.dead)
        {
            MenuActions.Instance.NavigateAndPushCanvas(netConnectingCanvas);
        }
    }

    public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
    {
        if (BoltNetwork.isClient && !this.dead)
        {
            // Pop the connecting page.
            MenuActions.Instance.NavigateAndPopCanvas();

            MenuActions.Instance.NavigateAndPushCanvas(this.netErrorCanvas);
        }
    }

    public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.isClient)
        {
            MenuActions.Instance.NavigateAndPopCanvas();
            MenuActions.Instance.NavigateAndPushCanvas(this.lobbyCanvas);
        }
    }

    public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
    {
        if (BoltNetwork.isClient && !this.dead)
        {
            // Pop the loading page.
            MenuActions.Instance.NavigateAndPopCanvas();

            MenuActions.Instance.NavigateAndPushCanvas(this.netErrorCanvas);
        }
    }

    public override void Disconnected(BoltConnection connection)
    {
        if (BoltNetwork.isClient && !this.dead)
        {
            // Pop the loading page.
            MenuActions.Instance.NavigateAndPopCanvas();

            MenuActions.Instance.NavigateAndPushCanvas(this.netErrorCanvas);
        }
    }

    public void CloseErrorPage()
    {
        MenuActions.Instance.NavigateAndPopCanvas();
        MenuActions.Instance.NavigateAndPopCanvas();
    }

    void Start()
    {
        if (this.netConnectingCanvas == null)
        {
            Debug.LogError("NetBoltMenuEvents expects netConnectingCanvas to be non-null");
            this.dead = true;
            return;
        }

        if (this.netErrorCanvas == null)
        {
            Debug.LogError("NetBoltMenuEvents expects netErrorCanvas to be non-null");
            this.dead = true;
            return;
        }

        if (this.lobbyCanvas == null)
        {
            Debug.LogError("NetBoltMenuEvents expects netErrorCanvas to be non-null");
            this.dead = true;
            return;
        }
    }
}
