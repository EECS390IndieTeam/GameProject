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
	private IPlayer player;

	// Use this for initialization
	void Start () {
		
		player = GetComponentInParent<IPlayer>();

        line = GetComponent<LineRenderer>();
        line.SetColors(BeamColor, BeamColor);
        line.SetVertexCount(2);
		line.SetPosition(0, player.MuzzlePoint);
		line.SetPosition(1, player.LaserEndpoint);
        curFadeTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		DebugHUD.setValue("laser endpoint", player.LaserEndpoint);
		line.enabled = player.LaserVisible;
		line.SetPosition (0, player.MuzzlePoint);
		line.SetPosition (1, player.LaserEndpoint);
//        curFadeTime += Time.deltaTime;
//        if(curFadeTime >= FadeTime){
//            Destroy(this.gameObject);
//            return;
//        }
//        float fadePercent = curFadeTime / FadeTime;
//        float adjustedFade = FadeCurve.Evaluate(fadePercent);
//
//        Color newColor = BeamColor;
//        newColor.a = adjustedFade;
//        line.SetColors(newColor, newColor);
	}
}
