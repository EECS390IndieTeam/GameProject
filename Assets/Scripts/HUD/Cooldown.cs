using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cooldown : MonoBehaviour {
	
	public Slider cooldownSlider;

	public Gun gunScript;
	Image sliderFill;
	Color fillColor;

	float CooldownRate;

	public static int maxDegrees{									//the max temp the weapon can be before overheat
		get;		
		set;
	}											

	public static float weaponTemp{
		get;
		set;
	}

	public static bool isOverheating{
		get;
		set;
	}


	// Use this for initialization
	void Start () {
		cooldownSlider.maxValue = gunScript.MaxTemperature;
		
		cooldownSlider.value = 0;
		weaponTemp = 0;
		isOverheating = false;

		CooldownRate = gunScript.CooldownRate;
		sliderFill = cooldownSlider.fillRect.GetComponentInChildren<Image>();
		fillColor = sliderFill.color;

	}
	
	//Update is called once per frame
	void Update () {
		weaponTemp = gunScript.Temperature;
//		print(gunScript);
		cooldownSlider.value = weaponTemp;
		isOverheating = gunScript.IsOverheating;
		if (isOverheating){
			sliderFill.color = Color.red;
		} else {
			sliderFill.color = fillColor;
		}
	}

}
