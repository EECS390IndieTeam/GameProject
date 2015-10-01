using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
	public GameObject player;
	public float currentHealth;                                   // The current health the player has.
	public Slider healthSlider;                                 // Reference to the UI's health bar.

	public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
	public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
	

	bool isDead;                                                // Whether the player is dead.
	bool damaged;                                               // True when the player gets damaged.
	float oldHealth;
	OwnerPlayer playerScript;	
	
	void Start () {

		
		// Set the initial health of the player.
		playerScript = player.GetComponent<OwnerPlayer>();
		healthSlider.maxValue = playerScript.MaxHealth;
		healthSlider.value = healthSlider.maxValue;
		damaged = false;
	}
	
	
	void Update () {
		currentHealth = playerScript.Health;
		healthSlider.value = currentHealth;

		// damaged = !(currentHealth < oldHealth);
		// // If the player has just been damaged...
		// if(damaged) {
			
		// 	damageImage.color = flashColour;				// set the colour of the damageImage to the flash colour.				//this bit is to give the player feedback when they've
		// }																														//been shot. Not currently functional with player health
		// // Otherwise...
		// else {
			
		// 	// damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);		
		// }
		
		oldHealth = currentHealth;
		damaged = false;										// Reset the damaged flag.
		
		if (Input.GetKeyDown(KeyCode.T)){
			playerScript.TakeDamage(10f, "Debug", new Vector3());
			print(currentHealth);
		}

	}      
}