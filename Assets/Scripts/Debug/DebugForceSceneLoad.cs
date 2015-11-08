using UnityEngine;
using System.Collections;

public class DebugForceSceneLoad : MonoBehaviour {
    private bool draw = false;

    private bool networkLoad = false;
    private bool loadAdditive = false;

    private float width = 300f;
    private const float LINE_HEIGHT = 27.3f;
    private const int BASE_LINE_COUNT = 2;

    void Awake() {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (!draw && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.F4)) {
            draw = true;
        } else if (draw && Input.GetKeyDown(KeyCode.Escape)) {
            draw = false;
        }
	}

    void OnGUI() {
        if (!draw) return;
        float height = LINE_HEIGHT * (BASE_LINE_COUNT + Application.levelCount);
        GUILayout.BeginArea(new Rect((Screen.width-width)/2, Screen.height-height, width, height), GUI.skin.box);
        GUILayout.BeginVertical();

        //title line
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Debug Scene Loader");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //settings line
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.enabled = !networkLoad;
        loadAdditive = GUILayout.Toggle(loadAdditive, "Load Additive");
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        GUI.enabled = !loadAdditive;
        networkLoad = GUILayout.Toggle(networkLoad, "Load via Bolt");
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //scene selection lines
        foreach (string scene in BoltScenes.AllScenes) {
            if (GUILayout.Button(scene)) {
                if (networkLoad) {
                    BoltNetwork.LoadScene(scene);
                } else {
                    if (loadAdditive) {
                        Application.LoadLevelAdditive(scene);
                    } else {
                        Application.LoadLevel(scene);
                    }
                    
                }
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
