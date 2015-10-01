using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cooldown : MonoBehaviour {
	
	public Slider cooldownSlider;
	public Gun gun;
	public static int maxDegrees{									//the max temp the weapon can be before overheat
		get{
			return maxDegrees;
		}
		set{
			maxDegrees = value;
		}
	}											

    

	public static float weaponTemp{
		get{
			return weaponTemp;
		}
		set{
			weaponTemp = value;
		}
	}

	public static bool isOverheating{
		get{
			return isOverheating;
		}
		set{
			isOverheating = value;
		}
	}


	// Use this for initialization
	void Start () {
		cooldownSlider.maxValue = gun.MaxTemperature;
		print(cooldownSlider.maxValue);
		cooldownSlider.value = 0;
		weaponTemp = 0;
		isOverheating = false;
	}
	
	// Update is called once per frame
	void Update () {
		weaponTemp = gun.Temperature;
		cooldownSlider.value = weaponTemp;
		if (!isOverheating){
			
			} else {

			}
	}
}
