using UnityEngine;
using System.Collections;
using System.Reflection;

[BoltGlobalBehaviour]
public class BoltDebugInfoAutoHide : MonoBehaviour {
    private BoltConsole console;
    private Bolt.DebugInfo debugInfo;

    FieldInfo field;

	// Use this for initialization
	void Start () {
        console = FindObjectOfType<BoltConsole>();
        debugInfo = FindObjectOfType<Bolt.DebugInfo>();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        field = typeof(BoltConsole).GetField("visible", flags);
	}
	
	// Update is called once per frame
	void Update () {

        debugInfo.enabled = (bool)field.GetValue(console);
	}
}
