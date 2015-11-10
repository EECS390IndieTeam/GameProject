using UnityEngine;
using System.Collections;
using System;

public class SurfaceMovement : MonoBehaviour {

    public Rigidbody character;

    //Factors for determinining speed
    public float pushMultiplier;

    //Realated to going around corners
    private Vector3 lastNormal;

    //Method that is called when first contact with a surface occurs
    public void attachToSurface(Collision hit)
    {
        character.velocity = new Vector3(0, 0, 0);
        lastNormal = hit.contacts[0].normal;
    }

    public void pushOff()
    {
        character.AddForce(lastNormal.normalized * pushMultiplier, ForceMode.Impulse);
    }
}
