using UnityEngine;
using System.Collections;

public class StickyBaby : BabyController {

	protected new void OnCollisionEnter (Collision other) {

		if (!enabled) {
			return;
		}

		transform.parent = other.collider.transform;
		EndMotion ();	
	}
}
