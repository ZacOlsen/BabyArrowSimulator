using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

	[SerializeField] private float rotationAngleSec = 360f;
	
	void FixedUpdate () {
		transform.RotateAround (transform.position, Vector3.up, rotationAngleSec * Time.fixedDeltaTime);
	}
}
