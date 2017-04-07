using UnityEngine;
using System.Collections;

public class StickyBaby : BabyController {

	protected new void OnCollisionEnter (Collision other) {

		if (!enabled) {
			return;
		}

		//transform.parent = other.collider.transform;

		if (other.gameObject.GetComponent<Death> () == null) {

			if (!Physics.Raycast (transform.position, -Vector3.up, ((BoxCollider)col).size.y / 2f, ~LayerMask.NameToLayer ("Baby"))) {

				float shift = Vector3.Distance (other.contacts [0].point, transform.position) - Mathf.Sqrt ((Mathf.Pow (((BoxCollider)col).size.x / 2f, 2) + Mathf.Pow (((BoxCollider)col).size.z / 2f, 2)));
				transform.position = Vector3.Lerp (other.contacts [0].point, transform.position, shift);
			}

			EndMotion ();	
		}
	}
}
