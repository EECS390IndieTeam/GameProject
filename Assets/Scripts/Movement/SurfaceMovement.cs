using UnityEngine;
using System.Collections;
using System;

public class SurfaceMovement : MonoBehaviour {


    public Rigidbody character;

    public Transform pCamera;

    //Factors for determinining speed
    public float maxVelocity;
    public float accelerationFactor;
    public float maxXSpeed;
    public float maxYSpeed;

    public float grabDistance;

    public LayerMask moveableLayers;

    //Reference to the attached surface
    private Transform attachedSurface;


    //Realated to going around corners
    private Vector3 lastNormal;
    private Vector3 lastPoint;
    private Vector3 cornerPoint;
    private Vector3 lastUp;

    //States for movement
    private bool attached = false;
    private bool firstLerp = false;
    private bool secondLerp = false;

    //Points for linar interpolation
    private Vector3 firstLerpStart;
    private Vector3 firstLerpEnd;
    private Vector3 secondLerpEnd;
    private Vector3 searchVector;

    //Time tracking for lerping
    public float totalLerpTime = .5f;
    private float currentLerpTime = 0f;

    //Time tracking for shift debounce
    public float shiftDebounce = .5f;
    private float debounceTime = .5f;

    public void moveCharacter(Vector2 inputVector)
    {

        //Debouncing for cornering controls
        if(debounceTime < shiftDebounce)
        {
            debounceTime += Time.fixedDeltaTime;
        }
        if (firstLerp || secondLerp)
        {
            lerp();
            return;
        }
        //Vector2 inputVector = GetInput();
        DebugHUD.setValue("last normal", lastNormal);
        float movementSpeed = character.velocity.sqrMagnitude;
        float desiredSpeed = calcupateDesiredSpeed(inputVector);
        Vector3 worldSpaceVector = pCamera.transform.TransformDirection(inputVector.x, inputVector.y, 0);

        //Zero movement on axis if necessary
        //New velocity cancelling code.
        zeroMovement(inputVector);
        Vector3 moveFromLast = Vector3.ProjectOnPlane(worldSpaceVector, lastNormal);
        if (movementSpeed < maxVelocity * maxVelocity)
        {
            RaycastHit hit;
            //Ok so we agree this works, this is the on a surface without extreme angles
            if (Physics.SphereCast(character.position + ((character.velocity * Time.fixedDeltaTime) + (desiredSpeed * moveFromLast * Time.deltaTime)), .1f, -lastNormal, out hit,
                grabDistance + character.GetComponent<SphereCollider>().radius, moveableLayers))
            {
                Vector3 moveVector = Vector3.ProjectOnPlane(worldSpaceVector, hit.normal);
                if (Vector3.Dot(hit.normal, lastNormal) >= Mathf.Cos(5 * Mathf.Deg2Rad))
                {
                    character.AddForce(moveVector * desiredSpeed * Time.fixedDeltaTime * (accelerationFactor * (1- (character.velocity.magnitude / maxVelocity))), ForceMode.Impulse);
                }
                else
                {
                   // Debug.Log("different normal from surface");
                    character.AddForce(-character.velocity + Quaternion.AngleAxis(Vector3.Angle(lastNormal, hit.normal),
                        Vector3.Cross(lastNormal, hit.normal)) * character.velocity, ForceMode.Impulse);
                    character.AddForce(moveVector * desiredSpeed * Time.fixedDeltaTime * accelerationFactor, ForceMode.Impulse);
                }
                if (Vector3.Dot(hit.normal, lastNormal) < Mathf.Cos(90 * Mathf.Deg2Rad))
                {
                    lastNormal = hit.normal;
                    lastPoint = hit.point;
                }

            }

            //This would handle transitions between surfaces and not allowing the player to go over the edge
            else
            {
               // Debug.Log("Lost surface");

                //First handle not going off the edge, easy, just move in opposite of desired direction.
                Vector3 moveVector = Vector3.ProjectOnPlane(lastPoint - character.position, lastNormal);
                character.AddForce(character.velocity.magnitude * (moveVector), ForceMode.Impulse);

                //Now deal with rounding corners. We're going to try a press button to move around corner approach.
                //TODO: Use only the actual input system.
                //if (Input.GetKey(KeyCode.LeftShift) && debounceTime >= shiftDebounce)
                //{
                //    shiftDebounce = 0;
                //    roundCorner(inputVector, desiredSpeed);
                //}
            }
        }

    }

    private void roundCorner(Vector2 inputVector, float desiredSpeed)
    {
        RaycastHit hit;
        Debug.Log("Hit shift");
        //Now look at what direction the user is trying to go in, have left and right win over up and down
        //Determines direction to look for surface if moving left or right
        if (Mathf.Abs(inputVector.x) >= Mathf.Abs(inputVector.y))
        {
            //Try to find a surface in the wanted direction
            Vector3 xInputWorld = pCamera.transform.TransformDirection(inputVector.x, 0, 0);
            searchVector = Vector3.ProjectOnPlane(xInputWorld, lastNormal) * desiredSpeed * Time.deltaTime;
            lastUp = pCamera.up;

        }
        //Determines drirection to look for surface if going up or down. 
        else
        {
            Vector3 yInputWorld = pCamera.transform.TransformDirection(0, inputVector.y, 0);
            searchVector = Vector3.ProjectOnPlane(yInputWorld, lastNormal) * desiredSpeed * Time.deltaTime;
            //Direction for orienting player on new surface
            if (inputVector.y >= 0)
            {
                lastUp = lastNormal;
            } else
            {
                lastUp = lastNormal;
            }
        }


        float distanceToLastPoint = (lastPoint - character.position).magnitude;
        //Tries to hit a new surface in the desired direction
        if (Physics.Raycast(character.position + searchVector + -lastNormal.normalized * distanceToLastPoint * 1.1f, -searchVector, out hit, 
            grabDistance + .5f + character.GetComponent<SphereCollider>().radius, moveableLayers))
        {
            Debug.Log("hit something");
            Debug.Log(hit.normal);
            //Now somehow lerp between them
            //Ok, first pass, just sort of square between them
            firstLerpStart = character.position;
            firstLerpEnd = character.position + (searchVector.normalized * character.GetComponent<SphereCollider>().radius);
            secondLerpEnd = firstLerpEnd + (hit.point - firstLerpEnd);
            firstLerp = true;
            secondLerp = false;

            cornerPoint = hit.point;
        }
    }

    //Stop character motion in each direction independently
    private void zeroMovement(Vector2 inputVector)
    {
        if (Mathf.Abs(inputVector.x) < Mathf.Epsilon)
        {
            Vector3 xToWorld = pCamera.TransformDirection(1, 0, 0);
            Vector3 xOnPlane = Vector3.ProjectOnPlane(xToWorld, lastNormal);
            character.AddForce(Vector3.Project(-character.velocity, xOnPlane), ForceMode.Impulse);
        }

        if (Mathf.Abs(inputVector.y) < Mathf.Epsilon)
        {
            Vector3 yToWorld = pCamera.TransformDirection(0, 1, 0);
            Vector3 yOnPlane = Vector3.ProjectOnPlane(yToWorld, lastNormal);
            character.AddForce(Vector3.Project(-character.velocity, yOnPlane), ForceMode.Impulse);
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

    internal void lerp()
    {
        //The moving out part of going around a corner
        if (firstLerp)
        {
            currentLerpTime += Time.deltaTime;
            character.position = Vector3.Lerp(firstLerpStart, firstLerpEnd, currentLerpTime / totalLerpTime);
            if (currentLerpTime > totalLerpTime)
            {
                firstLerp = false;
                secondLerp = true;
                currentLerpTime = 0f;
                firstLerpEnd = character.position;
            }

            return;
        }

        //The moving in part of going around a corner. Also handles looking at the new surface.
        if (secondLerp)
        {
            currentLerpTime += Time.deltaTime;
            character.position = Vector3.Lerp(firstLerpEnd, secondLerpEnd, currentLerpTime / totalLerpTime);
            //pCamera.rotation = Quaternion.Lerp(pCamera.rotation, Quaternion.LookRotation(cornerPoint - character.position), currentLerpTime / totalLerpTime);
            if (currentLerpTime > totalLerpTime)
            {
                firstLerp = false;
                secondLerp = false;
                currentLerpTime = 0f;
                //pCamera.LookAt(cornerPoint);
                Debug.Log("lerping done");
                RaycastHit hit;
                if (Physics.Raycast(character.position, -searchVector, out hit, grabDistance + .5f))
                {
                    lastNormal = hit.normal;
                    lastPoint = hit.point;
                    character.AddForce((hit.point - character.position).magnitude * -hit.normal * character.mass, ForceMode.Impulse);

                }
                else
                {
                    Debug.Log("Something went wrong after lerping");
                }
            }

            return;
        }
    }

    //Method that is called when first contact with a surface occurs
    public void attachToSurface(RaycastHit hit)
    {
        attached = true;
        attachedSurface = hit.transform;
        character.MovePosition(((hit.point - character.position) * .3f * Time.deltaTime) + character.position);
        character.velocity = new Vector3(0, 0, 0);
        lastNormal = hit.normal;
        lastPoint = hit.point;
    }

    //Method that is called when a surface is detached from
    public void detachFromSurface()
    {
        attached = false;
        attachedSurface = null;
    }

    //Provides input from axes in one vector
    private Vector2 GetInput()
    {

        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };

        return input;
    }
}
