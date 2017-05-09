using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour {

	[SerializeField] private bool burnDeath = false;

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			other.transform.root.GetComponent<BabyController> ().Die (burnDeath);
		}
	}

	private void OnCollisionEnter (Collision other) {

		if (other.gameObject.CompareTag ("Baby")) {
			other.transform.GetComponent<BabyController> ().Die (burnDeath);
		}
	}
}
