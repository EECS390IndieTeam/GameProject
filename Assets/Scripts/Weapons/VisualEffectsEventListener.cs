using UnityEngine;
using System.Collections;

public class VisualEffectsEventListener : Bolt.GlobalEventListener {
    public GameObject WeaponFirePrefab;
    public GameObject ExplosionPrefab;


    public override void OnEvent(WeaponFireEvent evnt) {
        GameObject prefab = Instantiate(WeaponFirePrefab);
        WeaponShot shot = prefab.GetComponent<WeaponShot>();
        shot.StartPoint = evnt.StartPoint;
        shot.EndPoint = evnt.EndPoint;
        shot.BeamColor = evnt.Color;
    }

    public override void OnEvent(ExplosionEvent evnt) {
        GameObject prefab = Instantiate(ExplosionPrefab);
        prefab.transform.position = evnt.Position;
        Destroy(prefab, 10f);
    }
}
