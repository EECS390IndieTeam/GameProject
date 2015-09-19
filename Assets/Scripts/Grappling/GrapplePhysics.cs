using UnityEngine;
using System.Collections;

public static class GrapplePhysics {

	public static Vector3 anchor;

	// Length of the grapple line
	public static float Length { set { length = value; sqrLength = length * length; } get { return length; } }
	private static float length;
	private static float sqrLength;             // for comparing with sqrMagnitude

	// The base speed with which the grapple pulls in the player in meters^2 per second
	public static float reelSpeed = 15;

	public static int reelMultiplier = 0;

	private static Vector3 lastAddedVelocity = Vector3.zero;

	// Makes the player zero in on the anchor
	public static float stabilizer = 1.1f;


	public static Vector3 calculateVelocity(Vector3 position, Vector3 velocity) {
		Vector3 targetVelocity = velocity;

		Vector3 grappleVector = anchor - position;
		float sqrDistance = grappleVector.sqrMagnitude;

		// The player's velocity along the direction of the grapple line
		Vector3 projectedVelocity = Math.project(velocity, grappleVector);

		// Make sure the player is not moving away from the anchor
		if (Math.isAntiparallel (projectedVelocity, grappleVector) && sqrDistance >= sqrLength) {
			targetVelocity -= projectedVelocity;
		}

		if (reelMultiplier > 0) {
			Vector3 errantVelocity = velocity - projectedVelocity;
			targetVelocity -= errantVelocity;

			Vector3 reelVelocity = grappleVector.normalized * reelSpeed * reelMultiplier;

			if (Math.vector3LessThan(projectedVelocity, new Vector3(Mathf.Abs(reelVelocity.x), Mathf.Abs(reelVelocity.y), Mathf.Abs(reelVelocity.z))) || 
			    Math.isAntiparallel(projectedVelocity, reelVelocity)) {
				targetVelocity -= lastAddedVelocity;
				targetVelocity = reelVelocity;
				lastAddedVelocity = reelVelocity;
			}

			// Apply stabilizers
			if (reelMultiplier > 1) {
				errantVelocity /= stabilizer;
			}
			targetVelocity += errantVelocity;
			sqrLength = sqrDistance;
		}
		
		return targetVelocity;
	}
}
