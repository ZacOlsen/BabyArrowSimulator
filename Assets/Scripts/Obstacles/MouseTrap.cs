using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrap : MonoBehaviour {

	[SerializeField] private float rotateAngleSpeed = 10f;
	[SerializeField] private float finalRotation = 355f;
	private Quaternion startingRotation;

	private bool snap;

	private Transform wireKiller;

	void Start () {

		wireKiller = transform.FindChild ("wired_square");
		startingRotation = wireKiller.rotation;
	}
	
	void FixedUpdate () {

		if (snap) {

			wireKiller.RotateAround (wireKiller.position, Vector3.right, -rotateAngleSpeed);

			if (wireKiller.localEulerAngles.x < finalRotation && wireKiller.localEulerAngles.x > 180) {
				snap = false;
				Invoke ("MoveBackToStart", 3);
			}
		}
	}

	void MoveBackToStart () {
		wireKiller.rotation = startingRotation;
	}

	void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			snap = true;
		}
	}
}
