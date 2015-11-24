using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cooldown : MonoBehaviour {
	
	public Slider cooldownSlider;
	public Gun gun;
	public Color overheatColor;
	public Color normalColor;

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
		cooldownSlider.maxValue = gun.MaxTemperature;
		
		cooldownSlider.value = 0;
		weaponTemp = 0;
		isOverheating = false;

		CooldownRate = gun.CooldownRate;
		sliderFill = cooldownSlider.fillRect.GetComponentInChildren<Image>();
		fillColor = sliderFill.color;

	}
	
	//Update is called once per frame
	void Update () {
		weaponTemp = gun.Temperature;
		cooldownSlider.value = weaponTemp;
		isOverheating = gun.IsOverheating;
		if (isOverheating){
			sliderFill.color = overheatColor;
		} else {
			sliderFill.color = fillColor;
		}
	}

}
