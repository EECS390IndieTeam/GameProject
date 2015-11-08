using UnityEngine;
using System.Collections;

public class WireCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnPreRender(){
		GL.wireframe = true;
		GL.Color (Color.blue);

	}

	void OnPostRender(){
		GL.wireframe=false;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
