using UnityEngine;
using System.Collections;

public class PlayerGrenades : MonoBehaviour {
	public GameObject playerCam;		//Grenade Throw is attached to the Camera
	public int grenadeCount;
	private int previous;
	private int total;

	public ThrowGrenade nadeScript;
	public GameObject[] grenades;

	void Start() {
		total = nadeScript.grenadeAmmo;
		previous = total;
		refill();
	}

	// Update is called once per frame
	void Update () {
		grenadeCount = nadeScript.grenadeAmmo;
		if (grenadeCount != previous) {
			for (int i = grenadeCount; i < grenades.Length; i++) {
				grenades[i].SetActive(false);
			}
		}
		if (grenadeCount == total) {
			refill();
		}
		previous = grenadeCount;
	}

	private void refill() {
		for (int i = 0; i < grenades.Length; i++) {
			grenades[i].SetActive(true);
		}
	}
}
