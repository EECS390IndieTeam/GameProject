using UnityEngine;
using System.Collections;

public static class Math {

	// Project vector onto the direction defined by directionVector
	public static Vector3 project(Vector3 vector, Vector3 directionVector) {
		return Vector3.Dot(vector, directionVector) / directionVector.sqrMagnitude * directionVector;
	}

	// Check to see if two vectors are pointing in exact opposite directions
	public static bool isAntiparallel(Vector3 vector1, Vector3 vector2) {
		return (Mathf.Sign(vector1.x) != Mathf.Sign(vector2.x) && 
		        Mathf.Sign(vector1.y) != Mathf.Sign(vector2.y) && 
		        Mathf.Sign(vector1.z) != Mathf.Sign(vector2.z));
	}

	public static bool vector3LessThan(Vector3 vector1, Vector3 vector2) {
		return (vector1.x < vector2.x && vector1.y < vector2.y && vector1.z < vector2.z);
	}

	public static bool vector3GreaterThan(Vector3 vector1, Vector3 vector2) {
		return (vector1.x > vector2.x && vector1.y > vector2.y && vector1.z > vector2.z);
	}
}
