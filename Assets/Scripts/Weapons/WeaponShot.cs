using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class WeaponShot : MonoBehaviour {
    public float FadeTime;
    public Color BeamColor;
    public AnimationCurve FadeCurve;
    public Vector3 EndPoint;
    public Vector3 StartPoint;

    private LineRenderer line;
    private float curFadeTime;

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
        line.SetColors(BeamColor, BeamColor);
        line.SetVertexCount(2);
        line.SetPosition(0, StartPoint);
        line.SetPosition(1, EndPoint);
        curFadeTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        curFadeTime += Time.deltaTime;
        if(curFadeTime >= FadeTime){
            Destroy(this.gameObject);
            return;
        }
        float fadePercent = curFadeTime / FadeTime;
        float adjustedFade = FadeCurve.Evaluate(fadePercent);

        Color newColor = BeamColor;
        newColor.a = adjustedFade;
        line.SetColors(newColor, newColor);
	}
}
