using UnityEngine;
using System.Collections;

public class StickyBaby : BabyController {

	protected new void OnCollisionEnter (Collision other) {

		if (!enabled) {
			return;
		}

		if (other.collider.CompareTag ("Floor") || other.collider.CompareTag ("Wall") || 
			other.collider.CompareTag ("Trampoline")) {

			EndMotion ();

		} else if (other.collider.CompareTag ("Balloon")) {

			transform.parent = other.collider.transform;
			EndMotion ();
		}
	}
}
