using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cooldown : MonoBehaviour {
	
	public GameObject player;
	public int maxDegrees;											//the max temp the weapon can be before overheat
	public int degreesPerFire;										//the degrees added to weapon per weapon fire
	public float cooldownSpeed;										//the speed the weapon cools off, in degrees/second
	public float cooldownDelay;										//the delay before the weapon begins to cool down after firing
	public float overheatDelay;										//the delay before the weapon begins to cool down after firing
	public int overheatPenalty;										//the amount of health the player loses for trying to fire during an overheat

	public bool hurtOnOverheat;
	public Slider cooldownSlider;

	bool overheat;
	float currentDegrees;
	float currentDelay;
	float currentOverheatDelay;

	PlayerHealth playerHealth;


	// Use this for initialization
	void Start () {
		playerHealth = player.GetComponent <PlayerHealth> ();

		cooldownSlider.value = 0;
	}
	
	// Update is called once per frame
	void Update () {
		cooldownSlider.value = currentDegrees;

		if (currentDelay > 0){
			currentDelay -= Time.deltaTime;							//wait the specified delay
		} else {
			overheat = false;										//take weapon out of overheat;
			if (currentDegrees > 0){				
				currentDegrees -= cooldownSpeed * Time.deltaTime;	//and then cool down specified degrees per second
			} else {
				currentDegrees = 0;									//to deal with Unity weirdness making the degrees smaller than 0
			}
		}
	}

	public void FireWeapon(){
		if (overheat){
			playerHealth.TakeDamage(overheatPenalty);				//hurt the player if they try to fire while the weapon is in overheat
		} else {
			currentDegrees += degreesPerFire;						//add specified degrees 
			if (currentDegrees >= maxDegrees){						
				currentDegrees = maxDegrees;						//if the degree count is now larger than the max, set degree count to max
				overheat = true;									//set the weapon to overheat
				currentDelay = overheatDelay;						//set the delay to the overheat delay
			} else {
				currentDelay = cooldownDelay;						//otherwise just set the delay to normal cooldown delay;
			}
		}
	}
}
