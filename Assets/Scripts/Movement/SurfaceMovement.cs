using UnityEngine;
using System.Collections;
using System;

public class SurfaceMovement{


    Rigidbody character;

    float maxVelocity;

    float maxXSpeed;

    float maxYSpeed;

    float grabDistance;

    Rigidbody attachedSurface;


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
        Vector3 worldSpaceVector = character.transform.TransformDirection(inputVector.x, inputVector.y, 0);
        if(movementSpeed < maxVelocity * maxVelocity)
        {
            if (Physics.Raycast(character.position + desiredSpeed * worldSpaceVector * Time.deltaTime, character.transform.forward, grabDistance + 1))
            {
                character.MovePosition(character.position + desiredSpeed * worldSpaceVector * Time.deltaTime);
            }
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
        character.velocity = new Vector3(0, 0, 0);
    }
}
