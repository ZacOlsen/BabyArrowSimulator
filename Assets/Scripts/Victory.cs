using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Victory : MonoBehaviour {

	private void Win (Collider other) {

		BabyController baby = other.transform.root.GetComponent<BabyController> ();

		if (baby.IsCurrent ()) {
			GameObject.Find ("Baby Manager").GetComponent<BabyManager> ().ShowVictory ();
			Destroy (baby);
		}
	}

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			Win (other);
		}
	}

	private void OnCollisionEnter (Collision other) {

		if (other.gameObject.CompareTag ("Baby")) {
			Win (other.collider);
		}
	}
}
