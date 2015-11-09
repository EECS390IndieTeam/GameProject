using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class WeaponShot : MonoBehaviour {
    public float FadeTime;
    public Color BeamColor;
    public AnimationCurve FadeCurve;
    public Vector3 EndPoint;
    public Vector3 StartPoint;
	public AudioSource laserSound;

    private LineRenderer line;
	private IPlayer player;

	// Use this for initialization
	void Start () {
		
		player = GetComponentInParent<IPlayer>();

        line = GetComponent<LineRenderer>();
        line.SetColors(BeamColor, BeamColor);
        line.SetVertexCount(2);
		line.SetPosition(0, player.MuzzlePoint);
		line.SetPosition(1, player.LaserEndpoint);
	}
	
	// Update is called once per frame
	void Update () {
		line.enabled = player.LaserVisible;
		if (line.enabled) {
			if (!laserSound.isPlaying) laserSound.Play (); // audio source loops
			line.SetPosition (0, player.MuzzlePoint);
			line.SetPosition (1, player.LaserEndpoint);
		} else {
			if (laserSound.isPlaying) laserSound.Stop ();
		}
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
