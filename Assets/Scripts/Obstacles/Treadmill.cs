using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour {

	[SerializeField] private float speed = 3f;

	private void OnCollisionStay (Collision other) {

		if (other.collider.CompareTag ("Baby")) {

			other.transform.GetComponent<Rigidbody> ().velocity = 
				transform.TransformDirection (new Vector3 (0, speed, 0));

			other.transform.GetComponent<BabyController> ().SetOnTreadmill (true);
		}
	}

	private void OnCollisionExit (Collision other) {

		if (other.collider.CompareTag ("Baby")) {
			other.transform.GetComponent<BabyController> ().SetOnTreadmill (false);
		}
	}
}
