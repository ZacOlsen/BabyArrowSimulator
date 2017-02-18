using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushInArea : MonoBehaviour {

	[SerializeField] private float pushForce = 5f;
	private Vector3 pushDirection;
	private Rigidbody babyRb;

	private void Start () {
		pushDirection = transform.TransformDirection (new Vector3 (0, 0, pushForce));
	}

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			babyRb = other.transform.parent.parent.GetComponent<Rigidbody> ();
		}
	}

	private void OnTriggerStay (Collider other) {

		if (other.CompareTag ("Baby")) {
			babyRb.AddForce (pushDirection);
		}
	}
}
