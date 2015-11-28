using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThrowGrenade : MonoBehaviour {
	//Variables for Grenade
	public int grenadeAmmo;

    public float ThrowForce = 10.0f;

    private Grenade heldGrenade;
    // public GameObject GrenadesObject;
    public List<GameObject> GrenadeList;
    // private PlayerGrenades grenadeUI; 

    void Start(){
        // grenadeUI = GrenadesObject.GetComponent<PlayerGrenades>();
    }

    void Update() {
        if (Input.GetButtonDown("Grenade") && heldGrenade == null && grenadeAmmo > 0) {
            heldGrenade = BoltNetwork.Instantiate(BoltPrefabs.Grenade).GetComponent<Grenade>();
            heldGrenade.SetThrower(GameManager.instance.CurrentUserName);
            heldGrenade.transform.parent = this.transform;//the grenade needs to follow us if we move while cooking it
            heldGrenade.transform.localPosition = new Vector3(0f,0f,3f);//put it in front of the player for now
            heldGrenade.transform.localRotation = Quaternion.identity;
            heldGrenade.transform.localScale = Vector3.one;
            heldGrenade.GetComponent<Rigidbody>().isKinematic = true;
            grenadeAmmo--;
            setActiveGrenades(grenadeAmmo);
        }
        if (Input.GetButtonUp("Grenade") && heldGrenade != null) {
            Rigidbody rgb = heldGrenade.GetComponent<Rigidbody>();
            rgb.isKinematic = false;
            rgb.AddForce(transform.forward * ThrowForce);
            heldGrenade.transform.parent = null;
            heldGrenade.GetComponent<Collider>().enabled = true;
            heldGrenade = null;
        }
    }

    public void setActiveGrenades(int grenadeAmmo){
        for (int i = 0; i < grenadeAmmo; i++){
            print(grenadeAmmo);
            GrenadeList[i].SetActive(false);
        }
    }
}
