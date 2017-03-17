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
				Aim ();
				UpdateLooking ();
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

			if (!grounded) {
				transform.eulerAngles = new Vector3 (0, vertRotation.eulerAngles.y, 0);
				vertRotation.localEulerAngles = Vector3.zero;
			}

			aiming = true;
		}

		if (Input.GetMouseButtonDown (1)) {
			aiming = false;
			ResetShootState ();
		}

		if (aiming) {
			UpdateAimArc (!grounded);
		}

		if (Input.GetMouseButtonUp (0) && aiming) {

			if (grounded) {
				ShootBaby (bm.GetNextBaby (), bm.NextIsSoldierBaby ());
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
