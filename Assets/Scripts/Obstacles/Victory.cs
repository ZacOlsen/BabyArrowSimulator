using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Victory : MonoBehaviour {

	[SerializeField] private float timeTilEndShown = 3f;
	[SerializeField] private GameObject brokenJar = null;

	private void Win (Collider other) {

		BabyManager bm = GameObject.Find ("Baby Manager").GetComponent<BabyManager> ();

		if (!bm.GetLevelOver ()) {
			BabyController baby = other.transform.root.GetComponent<BabyController> ();

			if (baby.IsCurrent ()) {

				Destroy(Instantiate (brokenJar, transform.position, transform.rotation), 5f);
				Destroy (GetComponent<BoxCollider> ());
				Destroy (GetComponent<MeshRenderer> ());
				Destroy (transform.GetChild(0).GetComponent<MeshRenderer> ());

				//bm.ShowVictory ();
				Invoke("ShowWin", timeTilEndShown);
				Destroy (baby);
			}
		}
	}

	void ShowWin () {
		GameObject.Find ("Baby Manager").GetComponent<BabyManager> ().ShowVictory ();
		Destroy (gameObject);
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
