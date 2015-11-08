using UnityEngine;
using System.Collections;

public class VisualEffectsEventListener : Bolt.GlobalEventListener {
    public GameObject WeaponFirePrefab;
    public GameObject ExplosionPrefab;
	public AudioSource explosionSound;


    public override void OnEvent(WeaponFireEvent evnt) {
        GameObject prefab = Instantiate(WeaponFirePrefab);
        WeaponShot shot = prefab.GetComponent<WeaponShot>();
        shot.StartPoint = evnt.StartPoint;
        shot.EndPoint = evnt.EndPoint;
        shot.BeamColor = evnt.Color;
		shot.transform.parent = gameObject.transform;
    }

    public override void OnEvent(ExplosionEvent evnt) {
        GameObject prefab = Instantiate(ExplosionPrefab);
        prefab.transform.position = evnt.Position;
		explosionSound.Play();
        Destroy(prefab, 10f);
    }
}
