using UnityEngine;
using System.Collections;

public class WireCam : MonoBehaviour {

	int normal_quality;

	// Use this for initialization
	void Start () {
		normal_quality = QualitySettings.GetQualityLevel();
	}


	void OnPreRender(){
		GL.wireframe = true;
		QualitySettings.SetQualityLevel (1, false);
	}

	void OnPostRender(){
		GL.wireframe=false;
		QualitySettings.SetQualityLevel (normal_quality, false);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
