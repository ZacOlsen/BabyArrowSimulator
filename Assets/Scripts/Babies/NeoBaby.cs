using UnityEngine;
using System.Collections;

public class NeoBaby : BabyController {

	[SerializeField] private GameObject regularBaby = null;
	[SerializeField] private float timeAimSpeed = .5f;
	private float timeSpeedSlowAmount;

	private bool launched;

	private const float startingTimeScale = 1f;
	private const float startingfixedTime = .02f;

	protected new void Start () {

		base.Start();

		timeSpeedSlowAmount = timeAimSpeed * Time.fixedDeltaTime;
	}

	protected new void Update () {

		if (Time.timeScale > 0) {
			
			if (!launched) {
				UpdateLooking ();
				Aim ();
			}

			if (Input.GetMouseButtonDown (0)) {
				Time.fixedDeltaTime = timeSpeedSlowAmount;
				Time.timeScale = timeAimSpeed;
			}

			if (Input.GetMouseButtonUp (0)) {
				Time.timeScale = startingTimeScale;
				Time.fixedDeltaTime = startingfixedTime;
				launched = true;
			}
		}
	}

	protected new void Aim () {

		if (Input.GetMouseButtonDown (0)) {
			aiming = true;
		}

		if (Input.GetMouseButtonDown (1)) {
			aiming = false;
			ResetShootState ();
		}

		if (aiming) {
			UpdateAimArc ();
		}

		if (Input.GetMouseButtonUp (0) && aiming) {

			if (grounded) {
				bool soldier = bm.NextIsSoldierBaby ();
				ShootBaby (bm.GetNextBaby (), soldier);
			} else {
				ShootBaby (regularBaby);
			}
		}
	}

	private void OnDisable () {

		Time.timeScale = startingTimeScale;
		Time.fixedDeltaTime = startingfixedTime;
	}

	private void OnEnable () {
		launched = false;
	}
}
