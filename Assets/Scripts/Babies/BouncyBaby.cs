using UnityEngine;
using System.Collections;

public class BouncyBaby : BabyController {

	[SerializeField] private int numberOfBounces = 2;
	[SerializeField] [Range(0, 1)] private float energyConserved = .66f;

	protected new void OnCollisionEnter (Collision other) {

		if (!enabled) {
			return;
		}

		if (other.collider.CompareTag ("Trampoline")) {

			rb.velocity = Vector3.Reflect (-other.relativeVelocity, other.contacts [0].normal) * energyConserved;

			if (rb.velocity.y < 1) {
				hitGround = true;
			}
		
		} else 	if (numberOfBounces > 0) {

			rb.velocity = Vector3.Reflect (-other.relativeVelocity, other.contacts [0].normal) * energyConserved;
			numberOfBounces--;

		} else if (other.collider.CompareTag ("Floor")) {
			hitGround = true;
		}
	}

	protected new void OnCollisionStay (Collision other) {

		if (numberOfBounces <= 0 && velCounter >= 8 && !grounded) {

			RaycastHit hit;

			Matrix4x4 mat = babyModel.GetChild (0).localToWorldMatrix;
			Vector3 pointUR = mat.MultiplyPoint3x4 (col.center + col.size / 2f);
			Vector3 pointUL = mat.MultiplyPoint3x4 (col.center + new Vector3(-col.size.x, col.size.y, col.size.z) / 2f);
			Vector3 pointBR = mat.MultiplyPoint3x4 (col.center + new Vector3(col.size.x, -col.size.y, col.size.z) / 2f);
			Vector3 pointBL = mat.MultiplyPoint3x4 (col.center + new Vector3(-col.size.x, -col.size.y, col.size.z) / 2f);

			bool succeed = false;

			if (Physics.Raycast (transform.position, -Vector3.up, out hit, col.size.z / 2f + .1f, ~(1 << 8))) {
				succeed = true;
			} else if (Physics.Raycast (pointBR, -Vector3.up, out hit, .1f, ~(1 << 8))) {
				succeed = true;
			} else if (Physics.Raycast (pointBL, -Vector3.up, out hit, .1f, ~(1 << 8))) {
				succeed = true;
			} else if (Physics.Raycast (pointUR, -Vector3.up, out hit, .1f, ~(1 << 8))) {
				succeed = true;
			} else if (Physics.Raycast (pointUL, -Vector3.up, out hit, .1f, ~(1 << 8))) {
				succeed = true;
			}

			if (!succeed) {
				Debug.LogError ("point detection failure");
			} else {

				transform.position = new Vector3 (hit.point.x, hit.point.y + col.size.y / 2f + .05f, hit.point.z);
				babyModel.transform.localRotation = Quaternion.identity;

				EndMotion ();
			}
		}
	}
}
