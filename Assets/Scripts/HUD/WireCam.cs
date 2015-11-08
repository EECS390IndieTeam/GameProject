using UnityEngine;
using System.Collections;

public class WireCam : MonoBehaviour {

	int normal_quality;
    bool fogState;
    private Color oldFogColor;
    public Color Color;
    public bool Fog;
    public int QualityLevel = 1;

	void OnPreRender(){
        normal_quality = QualitySettings.GetQualityLevel();
        fogState = RenderSettings.fog;
        oldFogColor = RenderSettings.fogColor;


        RenderSettings.fog = Fog;
        RenderSettings.fogColor = Color;
		GL.wireframe = true;
		QualitySettings.SetQualityLevel (QualityLevel, false);
	}

	void OnPostRender(){
        RenderSettings.fog = fogState;
        RenderSettings.fogColor = oldFogColor;
        GL.wireframe = false;
		QualitySettings.SetQualityLevel (normal_quality, false);
	}

    void Update() { }
}
