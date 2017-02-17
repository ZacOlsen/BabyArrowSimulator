using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBaby : BabyController {

	[SerializeField] private float gunSpeed = 20;
	private Vector3 fallSpeed;
	private float launchTime = 0;
	private float straightDuration = 0;
	private bool falling = false;

	protected new void Start () {

		base.Start ();

		launchTime = Time.time;
		rb.useGravity = false;
	}

	protected new void FixedUpdate () {

		base.FixedUpdate ();

		if (!falling && Time.time - launchTime > straightDuration) {
			
			rb.velocity = fallSpeed;
			rb.useGravity = true;
			falling = true;
		}
	}

	public void SetStats (float straightDuration, Vector3 fallSpeed) {

		this.straightDuration = straightDuration;
		this.fallSpeed = fallSpeed;

		rb.velocity = fallSpeed.normalized * gunSpeed;
	}
}
