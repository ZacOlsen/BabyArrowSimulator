using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour {

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			other.transform.parent.GetComponent<BabyController> ().Die ();
		}
	}
}
