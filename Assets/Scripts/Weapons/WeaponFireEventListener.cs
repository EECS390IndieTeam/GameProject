using UnityEngine;
using System.Collections;

public class WeaponFireEventListener : Bolt.GlobalEventListener {
    public GameObject WeaponFirePrefab;


    public override void OnEvent(WeaponFireEvent evnt) {
        GameObject prefab = Instantiate(WeaponFirePrefab);
        WeaponShot shot = prefab.GetComponent<WeaponShot>();
        shot.StartPoint = evnt.StartPoint;
        shot.EndPoint = evnt.EndPoint;
        shot.BeamColor = evnt.Color;
    }
}
