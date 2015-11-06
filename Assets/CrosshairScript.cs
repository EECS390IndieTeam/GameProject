using UnityEngine;
using UnityEngine.UI;

public class CrosshairScript : MonoBehaviour {

    public Image crosshair;
    private bool aimAssist = false;
    private Color32 red = new Color32(255, 47, 47, 255);
    private Color32 blue = new Color32(0, 162, 204, 255);

    public void ToggleAimAssist(bool assist)
    {
        aimAssist = assist;
    }
	
	// Update is called once per frame
	void Update () {
        if (aimAssist)
        {
            crosshair.CrossFadeColor(red, 0.05f, true, true);
        }
        else if(!aimAssist && crosshair.color != blue)
        {
            crosshair.CrossFadeColor(blue, 0.05f, true, true);
        }
	
	}
}
