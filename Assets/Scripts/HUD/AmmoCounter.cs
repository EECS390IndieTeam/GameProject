using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoCounter : MonoBehaviour {

	public Text loadedText;
	public Text reserveText;
	public int weaponCapacity;					//how much ammo the weapon can hold
	public int startingAmmo;					//how much ammo the player starts with
	public int reserveCapacity;					//how much ammo the player can carry outside the weapon

	int ammoInWeapon;							//ammo currently in the weapon
	int ammoInReserve;							//ammo currently held, but not in the weapon

	// Use this for initialization
	void Start () {
		ammoInReserve = startingAmmo;
		ammoInWeapon = 0;
		Reload();
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FireWeapon () {
		if (ammoInWeapon > 0){								//player can only fire if they have at least one bullet in the weapon
			ammoInWeapon -= 1;
		} else {
			//some feedback indicating reload necessary
		}
		UpdateAmmoText();
	}

	public void Reload () {
		int ammoToReload = weaponCapacity - ammoInWeapon;	//the amount of space currently left in the weapon
		if (ammoInReserve <= ammoToReload){					
			ammoInWeapon += ammoInReserve;					//if the player has less ammo in reserve than can fit in the weapon, all of the reserve ammo is loaded
			ammoInReserve = 0;					//and the reserve ammo is emptied
		} else {
			ammoInWeapon = weaponCapacity;					//otherwise the weapon is loaded to max
			ammoInReserve -= ammoToReload;					//the ammo loaded into the weapon is subtracted from the reserve
		}
		UpdateAmmoText();

	}

	void UpdateAmmoText(){
		loadedText.text = ammoInWeapon.ToString();
		reserveText.text = ammoInReserve.ToString();
	}

	public int PickupAmmo (int size) {
		int ammoToPickup = reserveCapacity - ammoInReserve;	//The amount of space left in reserve
		int leftInPickup;									//The amount of ammo left in the pickup after as much as possible is put in reserve;

		if (size <= ammoToPickup) {
			ammoInReserve += size;							//if the pickup has less ammo than can fit in reserve, Add all the ammo from the pickup to reserve
			leftInPickup = 0;								//no ammo left in pickup
		} else {
			ammoInReserve = reserveCapacity;				//the reserve is filled to capacity
			leftInPickup = size - ammoToPickup;				
		}

		UpdateAmmoText();
		return leftInPickup;

	}
}
