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

			//Vector3 direction = other.contacts[0].point - transform.position;
			//RaycastHit hit;
			//Physics.Raycast(transform.position, direction, out hit, direction.magnitude, ~(1<<8));

		//	Vector2 contactXZ = new Vector2 (hit.point.x, hit.point.z);
			Vector2 contactXZ = new Vector2 (other.contacts[0].point.x, other.contacts[0].point.z);
			Vector2 posXZ = new Vector2 (transform.position.x, transform.position.z);

			float shiftXZ = Vector3.Distance (contactXZ, posXZ) - 
				Mathf.Sqrt ((Mathf.Pow (col.size.x / 2f, 2) + Mathf.Pow (col.size.z / 2f, 2)));

			Vector2 newPos = Vector2.Lerp (contactXZ, posXZ, shiftXZ);
			transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);
			}

			transform.position = new Vector3 (transform.position.x, 
				transform.position.y + col.size.y / 2f - col.size.z, transform.position.z);
			EndMotion ();	
		}
	}
}
