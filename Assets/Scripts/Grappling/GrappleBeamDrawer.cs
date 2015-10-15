using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Lightning), typeof(IPlayer))]
public class GrappleBeamDrawer : MonoBehaviour {
    private Lightning lightning;
    private IPlayer player;

	// Use this for initialization
	void Start () {
        lightning = GetComponent<Lightning>();
        lightning.enabled = false;
        player = GetComponent<IPlayer>();
        lightning.maxLength = 200f;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        lightning.enabled = player.GrappleVisible; //avoid excess OnEnable and OnDisable events
        drawBeam(player.GrappleEndpoint);
	}

    public void drawBeam(Vector3 position) {
        lightning.targetPoint = position;
        lightning.length = Vector3.Magnitude(position - transform.position);
        lightning.numPoints = (int)(lightning.length / 6);
    }

}
