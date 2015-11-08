using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerGrenades : MonoBehaviour {
	public int grenadeCount;

	ThrowGrenade nadeScript;
	GameObject nade1;
	GameObject nade2;
	GameObject nade3;
	GameObject nade4;
	GameObject nade5;

	GameObject border1;
	GameObject border2;
	GameObject border3;
	GameObject border4;
	GameObject border5;

	public List<GameObject> grenades;
	public List<GameObject> borders;

	private List<Image> borderImages;

	Color iconColor;
	Color blueColor;

	bool noGrenadesFlash;
	public int noGrenadeFlashTime;

	private float t;

	// Use this for initialization
	void Start () {

		t = 1;

		noGrenadesFlash = false;

		// nade1 = this.transform.Find("Grenade Center 1").gameObject;
		// nade2 = this.transform.Find("Grenade Center 2").gameObject;
		// nade3 = this.transform.Find("Grenade Center 3").gameObject;
		// nade4 = this.transform.Find("Grenade Center 4").gameObject;
		// nade5 = this.transform.Find("Grenade Center 5").gameObject;

		iconColor = grenades.First().GetComponent<Image>().color;
		blueColor = new Color(37f/255f, 153f/255f, 219f/255f, 1f);
		Debug.Log(grenades.First().GetComponent<Image>().color);
		borderImages = new List<Image>();
		foreach (GameObject border in borders){
			Image borderImage = border.GetComponent<Image>();
			borderImage.color = blueColor;
			borderImages.Add(borderImage);

		}



		// grenades = {nade1, nade2, nade3, nade4, nade5};
		// this.borders = {border1, border2, border3, border4, border5};	

		// iconColor = "#";
	}
	
	// Update is called once per frame
	void Update () {

		Debug.Log("flash: " + noGrenadesFlash);
		if (noGrenadesFlash){
			foreach (Image borderImage in borderImages){
				borderImage.color = Color.red;
				
			}
			noGrenadesFlash = false;

		} else {
			foreach (Image borderImage in borderImages){
				borderImage.color = Color.Lerp(blueColor, borderImage.color, .95f);

		}
		}
	}

	public void updateUI(int grenadeCount){
		int i;
		for (i = 0; i < grenadeCount; i++){
			grenades.ElementAt(i).SetActive(true);
		}
		for (i = i; i < 5; i++){
			grenades.ElementAt(i).SetActive(false);
		}

		Debug.Log("updateUI called");
		
		
	}
	public void noGrenades(){
		noGrenadesFlash = true;

	}
}
