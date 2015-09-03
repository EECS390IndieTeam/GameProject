﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
	public float startingHealth = 100f;                            // The amount of health the player starts the game with.
	public float currentHealth;                                   // The current health the player has.
	public Slider healthSlider;                                 // Reference to the UI's health bar.
	public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
	public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
	public float regenSpeed = 10f;
	public float regenDelay = 5f;
	

	bool isDead;                                                // Whether the player is dead.
	bool damaged;                                               // True when the player gets damaged.
	float currentDelay;
	
	
	void Awake ()
	{

		
		// Set the initial health of the player.
		currentHealth = startingHealth;
	}
	
	
	void Update ()
	{
		healthSlider.value = currentHealth;
		// If the player has just been damaged...
		if(damaged)
		{
			// ... set the colour of the damageImage to the flash colour.
			damageImage.color = flashColour;
			currentDelay = regenDelay;
		}
		// Otherwise...
		else
		{
			// ... transition the colour back to clear.
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		
		// Reset the damaged flag.
		damaged = false;

		if (currentDelay > 0) {
			
			currentDelay -= Time.deltaTime;
		} else {
			if (currentHealth < startingHealth){
				currentHealth += regenSpeed * Time.deltaTime;
			} else {
				currentHealth = startingHealth;		//to deal with Unity weirdness making the health greater than the specified value.
			}

		}
		

	}
	
	
	public void TakeDamage (int amount)
	{
		float currentDelay = regenDelay;

		// Set the damaged flag so the screen will flash.
		damaged = true;
		
		// Reduce the current health by the damage amount.
		currentHealth -= amount;
		
		// Set the health bar's value to the current health.
		

		currentDelay -= Time.deltaTime;


		
		// If the player has lost all it's health and the death flag hasn't been set yet...
		if(currentHealth <= 0 && !isDead)
		{
			// ... it should die.
			Death ();
		}
	}
	
	
	void Death ()
	{
		// Set the death flag so this function won't be called again.
		isDead = true;

	}       
}