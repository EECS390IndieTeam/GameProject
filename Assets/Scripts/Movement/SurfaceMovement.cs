using UnityEngine;
using System.Collections;
using System;

public class SurfaceMovement : MonoBehaviour {


    public Rigidbody character;

    public Transform pCamera;

    public float maxVelocity;

    public float maxXSpeed;

    public float maxYSpeed;

    public float grabDistance;

    private Rigidbody attachedSurface;

    private Vector3 lastNormal;


    public SurfaceMovement(Rigidbody character, float velocity, float maxXSpeed, float maxYSpeed, float grabDistance)
    {
        this.character = character;
        maxVelocity = velocity;
        this.maxXSpeed = maxXSpeed;
        this.maxYSpeed = maxYSpeed;
        this.grabDistance = grabDistance;
    }

    public void moveCharacter(Vector2 inputVector)
    {
        float movementSpeed  = character.velocity.sqrMagnitude;
        float desiredSpeed = calcupateDesiredSpeed(inputVector);
        Vector3 worldSpaceVector = pCamera.transform.TransformDirection(inputVector.x, inputVector.y, 0);
        Vector3 characterToObject = attachedSurface.position - character.position;
        if(movementSpeed < maxVelocity * maxVelocity)
        {
            RaycastHit hit;
            if (Physics.Raycast(character.position + (desiredSpeed * worldSpaceVector * Time.deltaTime), characterToObject, out hit, grabDistance + .5f))
            {
                if(Vector3.Angle(hit.normal, lastNormal) >= 90)
                {
                    character.velocity = new Vector3(0, 0, 0);
                }
                Vector3 moveVector = Vector3.ProjectOnPlane(worldSpaceVector, hit.normal);
                character.AddForce(moveVector * desiredSpeed * .05f, ForceMode.Impulse);
                lastNormal = hit.normal;

            } else
            {
                character.AddForce(characterToObject * .2f, ForceMode.Impulse);
            }
            characterToObject =   attachedSurface.position - character.position;
        }
        if (desiredSpeed == 0)
        {
            Debug.Log("Velocity cancelling");
            character.AddForce(-character.velocity, ForceMode.Impulse);
        }

    }

    private float calcupateDesiredSpeed(Vector2 inputVector)
    {
        float desiredMoveSpeed = 0;
        if(inputVector.y!=0)
        {
            desiredMoveSpeed = maxYSpeed;
        }
        if(inputVector.x != 0)
        {
            desiredMoveSpeed = maxXSpeed;
        }
        return desiredMoveSpeed;
    }

    

    internal void attachToSurface(RaycastHit hit)
    {
        
        attachedSurface = hit.rigidbody;
        character.MovePosition(((attachedSurface.position - character.position) * .3f * Time.deltaTime) + character.position);
        character.velocity = new Vector3(0, 0, 0);
        lastNormal = hit.normal;
    }
}
