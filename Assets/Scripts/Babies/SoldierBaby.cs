using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBaby : BabyController {

	private float launchTime = 0;
	private float straightDuration = 0;

	protected new void Start () {

		base.Start ();

		launchTime = Time.time;
		rb.useGravity = false;
	}

	protected new void FixedUpdate () {

		base.FixedUpdate ();

		if (Time.time - launchTime > straightDuration) {
			rb.useGravity = true;
		}
	}

	public void SetStraightDuration (float straightDuration) {
		this.straightDuration = straightDuration;
	}
}
