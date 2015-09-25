using UnityEngine;
using System.Collections;

public class ThrowGrenade : MonoBehaviour {
	//Variables for Grenade
	public int grenadeAmmo;

    public float ThrowForce = 10.0f;

    private Grenade heldGrenade;

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
        }
        if (Input.GetButtonUp("Grenade") && heldGrenade != null) {
            Rigidbody rgb = heldGrenade.GetComponent<Rigidbody>();
            rgb.isKinematic = false;
            rgb.AddForce(transform.forward * ThrowForce);
            heldGrenade.transform.parent = null;
            heldGrenade = null;
        }
    }
}
