using UnityEngine;
using System.Collections;

public class ThrowGrenade : MonoBehaviour {
	//Variables for Grenade
	public int grenadeAmmo;

    public float ThrowForce = 10.0f;

	public AudioSource grenadeThrow;

    private Grenade heldGrenade;

	private AbstractPlayer player;

	void Start() {
		player = (AbstractPlayer)GameManager.instance.CurrentPlayer;
	}

	// Returns bool for success
	public bool PrepGrenade() {
		if (heldGrenade || grenadeAmmo <= 0) return false;
		heldGrenade = BoltNetwork.Instantiate(BoltPrefabs.Grenade).GetComponent<Grenade>();
		heldGrenade.SetThrower(GameManager.instance.CurrentUserName);
		heldGrenade.transform.parent = this.transform;//the grenade needs to follow us if we move while cooking it
		heldGrenade.transform.localPosition = Vector3.zero;
		heldGrenade.transform.localRotation = Quaternion.identity;
		heldGrenade.GetComponent<Rigidbody>().isKinematic = true;
		grenadeAmmo--;
		return true;
	}

	// Returns bool for success
	public bool GrenadeThrow() {
		if (!heldGrenade) return false;
		Rigidbody rgb = heldGrenade.GetComponent<Rigidbody>();
		rgb.isKinematic = false;
		if (grenadeThrow) grenadeThrow.Play ();
		heldGrenade.transform.parent = null;
		rgb.velocity = player.GetComponent<Rigidbody>().velocity;
		rgb.AddForce(player.transform.forward * ThrowForce);
		heldGrenade.GetComponent<Collider>().enabled = true;
		heldGrenade = null;
		return true;
	}
}
