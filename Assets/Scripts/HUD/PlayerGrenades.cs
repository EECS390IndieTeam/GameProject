using UnityEngine;
using System.Collections;

public class PlayerGrenades : MonoBehaviour {
	public GameObject playerCam;		//Grenade Throw is attached to the Camera
	public int grenadeCount;

	ThrowGrenade nadeScript;
	GameObject nade1;
	GameObject nade2;
	GameObject nade3;
	GameObject nade4;
	GameObject nade5;
	// Use this for initialization
	void Start () {
		nadeScript = playerCam.GetComponent<ThrowGrenade> ();

		nade1 = this.transform.Find("Grenade 1").gameObject;
		nade2 = this.transform.Find("Grenade 2").gameObject;
		nade3 = this.transform.Find("Grenade 3").gameObject;
		nade4 = this.transform.Find("Grenade 4").gameObject;
		nade5 = this.transform.Find("Grenade 5").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		grenadeCount = nadeScript.grenadeAmmo;
		if (grenadeCount < 1) {
			nade1.SetActive(false);
			nade2.SetActive(false);
			nade3.SetActive(false);
			nade4.SetActive(false);
			nade5.SetActive(false);
		}
		else if (grenadeCount < 2) {
			nade1.SetActive(true);
			nade2.SetActive(false);
			nade3.SetActive(false);
			nade4.SetActive(false);
			nade5.SetActive(false);
		}
		else if (grenadeCount < 3) {
			nade1.SetActive(true);
			nade2.SetActive(true);
			nade3.SetActive(false);
			nade4.SetActive(false);
			nade5.SetActive(false);
		}
		else if (grenadeCount < 4) {
			nade1.SetActive(true);
			nade2.SetActive(true);
			nade3.SetActive(true);
			nade4.SetActive(false);
			nade5.SetActive(false);
		}
		else if (grenadeCount < 5) {
			nade1.SetActive(true);
			nade2.SetActive(true);
			nade3.SetActive(true);
			nade4.SetActive(true);
			nade5.SetActive(false);
		}
		else {
			nade1.SetActive(true);
			nade2.SetActive(true);
			nade3.SetActive(true);
			nade4.SetActive(true);
			nade5.SetActive(true);
		}
	}
}
