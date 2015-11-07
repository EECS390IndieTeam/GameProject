using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// this class acts as an EventListener and manager for the connect via zeus menu
/// </summary>
public class FindGameMenuActions : Bolt.GlobalEventListener
{
    private bool refresh = true;
    private bool zeusConnected = false;
    private UdpKit.UdpEndPoint zeusServer;
    private bool natProbeValid = false;
    private UdpKit.NatFeatures natFeatures;
    private bool launch = true;

    public RectTransform scrollPanel;
    public GameObject listEntryPrefab;
    public PasswordPanel passwordPanel;

    public UnityEngine.UI.Text natInfoText;

    private List<GameObject> listEntries = new List<GameObject>();

    public UnityEngine.UI.Button refreshButton;

    public override void ZeusConnected(UdpKit.UdpEndPoint endpoint)
    {
        Debug.Log("Connected to Zeus server " + endpoint);
        zeusServer = endpoint;
        zeusConnected = true;
    }

    public override void ZeusConnectFailed(UdpKit.UdpEndPoint endpoint)
    {
        Debug.LogWarning("Failed to connect to Zeus server " + endpoint);
        zeusConnected = false;
    }

    public override void ZeusDisconnected(UdpKit.UdpEndPoint endpoint)
    {
        Debug.LogWarning("Disconnected from Zeus server " + endpoint);
        zeusConnected = false;
    }

    public override void ZeusNatProbeResult(UdpKit.NatFeatures features)
    {
        natProbeValid = true;
        natFeatures = features;
        Debug.Log("Nat Probe Result:\nAllowsUnsolicitedTraffic: " + features.AllowsUnsolicitedTraffic +
            "\nLANEndpoint: " + features.LanEndPoint +
            "\nSupportsEndPointPreservation: " + features.SupportsEndPointPreservation +
            "\nSupportsHairpinTranslation: " + features.SupportsHairpinTranslation +
            "\nWanEndPoint: " + features.WanEndPoint);
        natInfoText.text = "<b>NAT Status</b>" +
            "\nAllows unsolicited traffic: " + colorFeatState(features.AllowsUnsolicitedTraffic) +
            "\nLAN endpoint: " + features.LanEndPoint +
            "\nSupports endpoint preservation: " + colorFeatState(features.SupportsEndPointPreservation) +
            "\nSupports hairpin translation: " + colorFeatState(features.SupportsHairpinTranslation) +
            "\nWAN endpoint: " + features.WanEndPoint;
    }

    private static string colorFeatState(UdpKit.NatFeatureStates val)
    {
        switch (val)
        {
            case UdpKit.NatFeatureStates.Yes: return "<color=green>Yes</color>";
            case UdpKit.NatFeatureStates.No: return "<color=red>No</color>";
            case UdpKit.NatFeatureStates.Unknown: return "<color=blue>Unknown</color>";
            default: return "";
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!BoltNetwork.isRunning && launch)
        {
            BoltLauncher.StartClient();
            refresh = true;
            launch = false;
        }
        if (BoltNetwork.isRunning && Bolt.Zeus.IsConnected && refresh)
        {
            refresh = false;
            Bolt.Zeus.RequestSessionList();
            refreshButton.interactable = false;
        }
    }

    public void PollSessionList()
    {
        refresh = true;
    }

    private void destroyAllEntries()
    {
        foreach (var e in listEntries)
        {
            Destroy(e);
        }
        listEntries.Clear();
    }

    //creates an item in the list
    private void createEntryForSession(UdpKit.UdpSession session)
    {
        GameObject obj = Instantiate<GameObject>(listEntryPrefab);
        listEntries.Add(obj);
        obj.transform.SetParent(scrollPanel, false);
        EntryPanel entry = obj.GetComponent<EntryPanel>();
        entry.session = session;
        entry.passwordPanel = passwordPanel;
    }

    //callback when the session list is updated, redraws the list
    public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList)
    {
        Debug.Log("Zeus session list updated");
        destroyAllEntries();
        foreach (var entry in sessionList)
        {
            createEntryForSession(entry.Value);
        }
        refreshButton.interactable = true;
    }
}
