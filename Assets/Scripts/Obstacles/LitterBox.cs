using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitterBox : MonoBehaviour {

	[SerializeField][Range(0, 1)] private float percentPowerDecrease = .5f;

	void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {

			other.transform.root.GetComponent<BabyController> ().SetPowerDecrease (percentPowerDecrease);
		}
	}
}
