using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour {

	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float distanceUp = 5f;
	[SerializeField] private float distanceDown = 3f;
	[SerializeField] private float errorRange = .1f;

	private Vector3 upLocation;
	private Vector3 downLocation;

	private Vector3 currentDestination;

	private void Start () {

		upLocation = new Vector3 (transform.position.x, transform.position.y + distanceUp, transform.position.z);
		downLocation = new Vector3 (transform.position.x, transform.position.y - distanceDown, transform.position.z);

		currentDestination = upLocation;
	}
	
	private void FixedUpdate () {

		if (Vector3.Distance(transform.position, currentDestination) < errorRange) {

			if (currentDestination.y == upLocation.y) {
				currentDestination = downLocation;

			} else {
				currentDestination = upLocation;
			}
		}

		transform.position = Vector3.Lerp (transform.position, currentDestination, 
			moveSpeed / Vector3.Distance(transform.position, currentDestination));
	}
}
