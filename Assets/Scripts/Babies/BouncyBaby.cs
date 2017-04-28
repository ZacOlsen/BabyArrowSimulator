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
			audioPlayer.PlayOneShot (wallHitSounds[Random.Range(0, wallHitSounds.Length - 1)], MenuController.GetFXLevel ());

			if (rb.velocity.y < 1) {
				hitGround = true;
			}
		
		} else 	if (numberOfBounces > 0) {

			rb.velocity = Vector3.Reflect (-other.relativeVelocity, other.contacts [0].normal) * energyConserved;
			audioPlayer.PlayOneShot (wallHitSounds[Random.Range(0, wallHitSounds.Length - 1)], MenuController.GetFXLevel ());
			numberOfBounces--;

		} else if (other.collider.CompareTag ("Floor")) {
			hitGround = true;
		}
	}

	protected new void OnCollisionStay (Collision other) {

		if (numberOfBounces <= 0) {
			base.OnCollisionStay (other);
		}
	}
}
