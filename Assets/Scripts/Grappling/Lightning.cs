using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Lightning : MonoBehaviour {

	public Material lightningMaterial;

	[System.NonSerialized]
	public LineRenderer lineRenderer;

	[System.NonSerialized]
	public int numPoints;
	[System.NonSerialized]
	public float length;
	[System.NonSerialized]
	public float maxLength;

	public int framesPerUpdate = 1;
	private int frame;

	[System.NonSerialized]
	public Vector3 targetPoint;

	public AnimationCurve curve;
	public float multiplier = 1.2f;

	private float delta, percent;
	private Vector3 forward;
	

	// Use this for initialization
	void Start () {
		frame = 0;
		if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = true;
		lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRenderer.receiveShadows = false;
		lineRenderer.material = lightningMaterial;
		lineRenderer.useLightProbes = false;
		lineRenderer.SetWidth(1, 1);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (frame == 0) {
			draw ();
		}
		frame++;
		frame %= framesPerUpdate;
	}

	void draw() {
		lineRenderer.SetVertexCount(numPoints + 1);
		//lineRenderer.SetVertexCount(2 * numPoints + 1);

		Vector3 lineDirection = Vector3.Normalize(targetPoint - transform.position);

		Vector3 position = transform.position;

		float stepSize = length / numPoints;

		lineRenderer.SetPosition(0, position);

		for (int i = 1; i <= numPoints; i++) {
			float curveMultiplier = curve.Evaluate((float)i * stepSize / (float)maxLength);
			lineRenderer.SetPosition (i, position + lineDirection * i * stepSize + Random.onUnitSphere * multiplier * curveMultiplier);
		}
//		for (int j = 1; j <=numPoints; j++) {
//			float curveMultiplier = curve.Evaluate((float)(numPoints - j) * stepSize / (float)maxLength);
//			lineRenderer.SetPosition (j + numPoints, targetPoint - lineDirection * j * stepSize + Random.onUnitSphere * multiplier * curveMultiplier);
//		}
	}

	void OnEnable() {
		Start ();
		lineRenderer.enabled = true;
	}

	void OnDisable() {
		if (!lineRenderer) return;
		lineRenderer.SetVertexCount(0);
		lineRenderer.enabled = false;
	}
}
