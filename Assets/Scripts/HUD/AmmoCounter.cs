using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoCounter : MonoBehaviour {

	public Text loadedText;
	public Text reserveText;
	public int weaponCapacity;
	public int reserveCapacity;
	public int startingAmmo;

	int ammoInWeapon;
	int ammoInReserve;
	int totalAmmo;

	// Use this for initialization
	void Start () {
		ammoInReserve = startingAmmo;
		ammoInWeapon = 0;
		Reload();
	
	}
	
	// Update is called once per frame
	void Update () {
		loadedText.text = ammoInWeapon.ToString();
		reserveText.text = ammoInReserve.ToString();
	}

	public void FireWeapon () {
		if (ammoInWeapon > 0){
			ammoInWeapon -= 1;
		} else {
			//some feedback indicating reload necessary
		}
	}

	public void Reload () {
		int ammoToReload = weaponCapacity - ammoInWeapon;
		if (ammoInReserve < ammoToReload){					//if the player has less ammo in reserve than can fit in the weapon, all of it is loaded
			ammoInWeapon += ammoInReserve;
			ammoInReserve -= ammoInReserve;
		} else {
			ammoInWeapon += ammoToReload;					//otherwise the weapon is loaded to max
			ammoInReserve -= ammoToReload;
		}
	}
}
