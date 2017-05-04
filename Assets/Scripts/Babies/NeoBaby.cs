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

			if (grounded && !dying) {

				gun.enabled = bm.NextIsSoldierBaby ();
				bow.enabled = !gun.enabled;
			}

			if (Input.GetMouseButtonDown (0) && !dying) {

				if (!grounded) {
					Time.fixedDeltaTime = timeSpeedSlowAmount;
					Time.timeScale = timeAimSpeed;
				}

				aiming = true;
			}

			if (!launched) {
				Aim ();
				UpdateLooking ();
			}
			
			if (Input.GetMouseButtonUp (0)) {
				Time.timeScale = startingTimeScale;
				Time.fixedDeltaTime = startingfixedTime;

				if (aiming) {
					launched = true;
				}
			}

			if (Input.GetMouseButtonDown (1)) {
				Time.timeScale = startingTimeScale;
				Time.fixedDeltaTime = startingfixedTime;
				launched = false;
				aiming = false;

				if (!grounded && !dying) {
					anim.SetInteger ("animState", 0);
				}
			}
		}
	}

	protected new void Aim () {

		if (Input.GetMouseButtonDown (0)) {

			if (!grounded && !dying) {
				transform.eulerAngles = new Vector3 (0, vertRotation.eulerAngles.y, 0);
				vertRotation.localEulerAngles = Vector3.zero;
				anim.SetInteger ("animState", 2);
			}

	//		aiming = true;
		}

		if (Input.GetMouseButtonDown (1)) {
	//		aiming = false;
			ResetShootState ();
		}

		if (aiming) {
			UpdateAimArc (!grounded);
		}

		if (Input.GetMouseButtonUp (0) && aiming && !dying) {

			Time.timeScale = startingTimeScale;
			Time.fixedDeltaTime = startingfixedTime;

			if (grounded) {
				ShootBaby (bm.GetNextBaby (), bm.NextIsSoldierBaby ());
			} else {
				ShootBaby (regularBaby);
			}
				
			anim.SetInteger ("animState", 0);
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
