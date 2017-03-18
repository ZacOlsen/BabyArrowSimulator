using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour {

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			other.transform.root.GetComponent<BabyController> ().Die ();
		}
	}

	private void OnCollisionEnter (Collision other) {

		if (other.gameObject.CompareTag ("Baby")) {
			other.transform.GetComponent<BabyController> ().Die ();
		}
	}
}
