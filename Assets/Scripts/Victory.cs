using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Victory : MonoBehaviour {

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {

			BabyController baby = other.transform.parent.GetComponent<BabyController> ();

			if (baby.IsCurrent ()) {
				GameObject.Find ("Baby Manager").GetComponent<BabyManager> ().ShowVictory ();
				Destroy (baby);
			}
		}
	}
}
