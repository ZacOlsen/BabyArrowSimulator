using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

	[SerializeField] private float rotationAngleSec = 360f;
	[SerializeField] RotationDirection direction = RotationDirection.Y;

	private enum RotationDirection {X, Y, Z};

	void FixedUpdate () {

		Vector3 vec = Vector3.forward;

		if (direction == RotationDirection.X) {
			vec = Vector3.right;
		} else if (direction == RotationDirection.Y) {
			vec = Vector3.up;
		}

		transform.RotateAround (transform.position, transform.parent.TransformDirection (vec),
			rotationAngleSec * Time.fixedDeltaTime);
	}
}
