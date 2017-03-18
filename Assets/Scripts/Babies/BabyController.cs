using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BabyController : MonoBehaviour {

	/**
	 *  max angle the player can look up 
	 */
	private const float maxRotationY = 89f;

	/**
	 * min the player can look down
	 */
	private const float minRotationY = -89f;

	/**
	 * the current y rotation of vertical rotation, DO NOT TOUCH
	 */
	private float rotationY;

	/**
	 * sensitivity of the mouse
	 */
	[SerializeField] private float sensitivity = 2f;

	/**
	 * used to store the vertical rotation so the model doesn't rotate
	 */
	protected Transform vertRotation;

	/**
	 * the initial position of the baby being launched
	 */
	private Transform launchStartPos;

	/**
	 * max launch speed of the new baby
	 */
	[SerializeField] private float maxLaunchSpeed = 10f;

	[SerializeField] private float launchIterationSize = .02f;

	[SerializeField][Range(0, 1)] private float initialCharge = .5f;

	private float launchX = 0;

	/**
	 * current launch speed of the baby based on the charge
	 */
	private float launchSpeed;

	[SerializeField] protected float slideTime = 2.5f;

	/**
	 * this gameobjects rigidbody
	 */
	protected Rigidbody rb;

	/**
	 * the collider of this gameobject
	 */
	private Collider col;

	/**
	 * the charging part of the chargebar
	 */
	private RectTransform chargeBar;

	/**
	 * the initial elevation on the screen of the charge bar
	 */
	private float chargeBarIniX;

	[SerializeField] private float maxArcDistance = 30f;

	/**
	 * the gameobject used to create the aiming arc
	 */
	[SerializeField] private GameObject aimArc = null;

	/**
	 * the array of gameobjects used to create the aiming arc
	 */
	private static GameObject[] arcPieces;

	/**
	 * the number of gameobjects the aiming arc is made of
	 */
	private const int NUMBER_OF_ARC_PIECES = 30;

	protected bool grounded;

	private bool current = true;

	protected bool aiming;

	protected BabyManager bm;

	private static GameObject previousBaby;

	private Transform babyModel;

	private bool onTreadmill;

	protected bool hitGround;

	[SerializeField] private GameObject fractalizedBaby = null;

	[SerializeField] private float timeTilFractalizedDestroyed = 5f;

	[SerializeField] private float timeTilSwitchBack = 3f;

	[SerializeField] private Vector3 camRotation = new Vector3();

	protected Animator anim;

	[SerializeField] private SkinnedMeshRenderer bow = null;
	[SerializeField] private SkinnedMeshRenderer gun = null;

	protected void Awake () {

		rb = GetComponent<Rigidbody> ();

		babyModel = transform.FindChild ("Baby Model");
		col = transform.GetComponentInChildren<Collider> ();

		bm = GameObject.Find ("Baby Manager").GetComponent<BabyManager> ();
	}
		
	protected void Start () {

		rb.freezeRotation = true;
		anim = babyModel.GetComponentInChildren<Animator> ();

		Camera.main.transform.localEulerAngles = camRotation;

		chargeBar = ((RectTransform) GameObject.Find ("Charge Foreground").transform);
		chargeBarIniX = 0;

		vertRotation = transform.FindChild ("Vertical Rotation");
		launchStartPos = vertRotation.FindChild ("Launch Start Pos");

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		//set the initial chargeBar state to no charge
		UpdateChargeBar ();

		//create the arcPieces if they havent been created before
		if (arcPieces == null) {
			arcPieces = new GameObject [NUMBER_OF_ARC_PIECES];
		}

		for (int i = 0; i < arcPieces.Length; i++) {
		
			if (arcPieces [i] == null) {
				arcPieces [i] = Instantiate (aimArc);
				arcPieces [i].SetActive (false);
			}
		}

		anim.SetInteger ("animState", 0);
		bow.enabled = false;
	}
		
	protected void Update () {

		if (Time.timeScale > 0) {
			UpdateLooking ();

			if (grounded) {

				gun.enabled = bm.NextIsSoldierBaby ();
				bow.enabled = !gun.enabled;

				Aim ();
			}
		}
	}

	protected void FixedUpdate () {

		if (babyModel != null) {
			
			if (!grounded) {

				if (!onTreadmill) {
					babyModel.localEulerAngles = new Vector3 (Mathf.Rad2Deg * Mathf.Atan2 (Mathf.Sqrt (Mathf.Pow (rb.velocity.x, 2) +
					Mathf.Pow (rb.velocity.z, 2)), rb.velocity.y), 0, 0);

					Quaternion babyRot = babyModel.rotation;
					Quaternion camRot = vertRotation.rotation;

					if (!aiming) {
						transform.eulerAngles = new Vector3 (0, Mathf.Rad2Deg * Mathf.Atan2 (rb.velocity.x, rb.velocity.z), 0);
					}

					babyModel.rotation = babyRot;
					vertRotation.rotation = camRot;
				}

				if (hitGround) {
					babyModel.localEulerAngles = new Vector3 (90, 0, 0);

					if (rb.velocity.magnitude < .01f) {
						EndMotion ();
					}
				}
			}

//			if (rb.velocity.magnitude < .01f) {
//				EndMotion ();
//			}
		}
	}

	protected void UpdateLooking () {

		float x = 0;
		x = Input.GetAxis ("Mouse X") * sensitivity;

		rotationY -= Input.GetAxis ("Mouse Y") * sensitivity;

		if (grounded) {
			
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y + x, 0f);
			vertRotation.transform.localRotation = Quaternion.Euler (Mathf.Clamp (rotationY, minRotationY, maxRotationY), 0, 0);

			anim.SetBool ("shuffle", x != 0 && !bm.NextIsSoldierBaby ());
			anim.SetBool ("gun", bm.NextIsSoldierBaby ());
		
		} else if (aiming) {

			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y + x, 0f);
			vertRotation.transform.localRotation = Quaternion.Euler (Mathf.Clamp (rotationY, minRotationY, maxRotationY), 0, 0);

		} else {
			vertRotation.transform.localRotation = Quaternion.Euler (Mathf.Clamp (rotationY, minRotationY, maxRotationY), 
				vertRotation.transform.localEulerAngles.y + x, 0);
		}
	}

	protected void Aim () {

		if (Input.GetMouseButtonDown (0)) {
			aiming = true;
			launchX = initialCharge;
		}

		if (Input.GetMouseButtonDown (1)) {
			ResetShootState ();
		}

		if (aiming) {
			UpdateAimArc ();
		}

		if (Input.GetMouseButtonUp (0) && aiming) {

			bool soldier = bm.NextIsSoldierBaby ();
			ShootBaby (bm.GetNextBaby (), soldier);
		}
	}

	protected void ResetShootState () {

		aiming = false;
		launchSpeed = 0;
		UpdateChargeBar ();
		launchX = initialCharge;
		anim.SetFloat ("blendPB", 0);

		//uncommenting this line will make the aim arc invis on launch and right click
//		DisableAimArc();
	}

	protected void ShootBaby (GameObject baby, bool soldier = false) {

		if (baby != null) {

			bm.ChangeUI (false);

			baby = ((GameObject) Instantiate (baby, launchStartPos.position, transform.rotation));
			baby.transform.FindChild ("Vertical Rotation").transform.localRotation = vertRotation.localRotation;

			if (col != null) {
				Physics.IgnoreCollision (col, baby.GetComponentInChildren<Collider> ());
			}

			Rigidbody rig = baby.GetComponent<Rigidbody> ();

			Vector3 vel = vertRotation.TransformDirection (new Vector3 (0, 0, launchSpeed));
		
			if (soldier) {
				baby.GetComponent<SoldierBaby> ().SetStats (launchSpeed / maxLaunchSpeed, vel);

			} else {
				rig.velocity = vel;
			}

			baby.GetComponent<BabyController> ().rotationY = rotationY;

			chargeBar.localPosition = new Vector3 (chargeBarIniX, chargeBar.localPosition.y, chargeBar.localPosition.z);
			chargeBar.localScale = Vector3.one;

			Destroy (GetComponentInChildren<Camera>().gameObject);
			launchSpeed = 0;

			this.enabled = false;

			current = false;
			ResetShootState ();
		}
	}

	/**
	 * Updates the chargeBar based on how much it is currently charged.
	 */
	private void UpdateChargeBar () {

		float percent = launchSpeed / maxLaunchSpeed;

		chargeBar.localScale = new Vector3 (percent, 1, 1);
		chargeBar.localPosition = new Vector3 (chargeBarIniX - ((chargeBar.rect.width - chargeBar.rect.width * percent) / 2f), 
			chargeBar.localPosition.y, chargeBar.localPosition.z);
	}
		
	/**
	 * Updates the aiming arc based on the amount charged and the angle the player is looking at.
	 */
	protected void UpdateAimArc (bool regularArc = false) {

		launchX = Mathf.Clamp(launchX + launchIterationSize, -1, 1);
		launchSpeed = maxLaunchSpeed * Mathf.Pow (launchX, 2); 

		if (launchX >= 1) {
			launchX = -1;
		}

		anim.SetFloat ("blendPB", Mathf.Abs (launchX));
	
		UpdateChargeBar ();

		if (bm.NextIsSoldierBaby () && !regularArc) {
			SoldierBabyArc ();
			return;
		}

		float initialYVel = launchStartPos.TransformDirection (new Vector3(0, 0, launchSpeed)).y;

		float time = 2 * (-initialYVel / Physics.gravity.y) + (-initialYVel + Mathf.Sqrt(Mathf.Pow(initialYVel, 2) - 
			4 * -Physics.gravity.y / 2 * -(transform.position.y))) / -Physics.gravity.y;

		float initialXVel = launchSpeed * Mathf.Cos (vertRotation.localEulerAngles.x * Mathf.Deg2Rad);

		float distance = 0;
		for (float i = .1f; i < time; i += .01f) {
			
			distance = Mathf.Sqrt (Mathf.Pow(initialXVel * i, 2) + Mathf.Pow(.5f * -Physics.gravity.y * i * i + initialYVel * i + transform.position.y, 2));
			if (distance > maxArcDistance) {
				time = i;
			}
		}
			
		Vector3 landPos = transform.TransformDirection (new Vector3 (0, 0, distance)) + launchStartPos.position;

/**	Debug Code creates straight line aiming
 		float angle = Mathf.Atan2 (landPos.x - transform.position.x, landPos.z - transform.position.z) * Mathf.Rad2Deg;
		Transform arc = ((GameObject) Instantiate (aimArc, (landPos + transform.position) / 2, 
			Quaternion.Euler(new Vector3(90, angle, 0)))).transform;
		arc.localScale = new Vector3 (arc.localScale.x, distance / 2, arc.localScale.z); */

		Vector3 previousPoint = transform.position;
		int counter = 0;
		float increment = time / (float) arcPieces.Length;
		for (float i = increment; i <= time + .01f; i += increment, counter++) {

			if (counter >= arcPieces.Length) {
				break;
			}

			Vector3 point = transform.TransformDirection(new Vector3(0, initialYVel * i + 
				.5f * Physics.gravity.y * Mathf.Pow(i, 2), initialXVel * i));
			point += transform.position;

			float angleHor = Mathf.Atan2 (landPos.x - transform.position.x, landPos.z - transform.position.z) * Mathf.Rad2Deg;
			float angleVert = 90;
							
			angleVert -= Mathf.Atan2 (point.y - previousPoint.y, Vector2.Distance (new Vector2 (point.x, point.z),
				new Vector2 (previousPoint.x, previousPoint.z))) * Mathf.Rad2Deg;

			arcPieces [counter].SetActive (true);
			arcPieces [counter].transform.position = (point + previousPoint) / 2;
			arcPieces [counter].transform.rotation = Quaternion.Euler(new Vector3(angleVert, angleHor, 0));
			arcPieces [counter].transform.localScale = new Vector3(arcPieces[counter].transform.localScale.x, 
				Vector3.Distance(point, previousPoint) / 2, arcPieces[counter].transform.localScale.z);

			previousPoint = point;
		}
	}

	private void SoldierBabyArc () {

		Vector3 physicsPos = launchStartPos.TransformDirection (new Vector3(0, 0, launchSpeed)) + transform.position;

		float angleHor = Mathf.Atan2 (physicsPos.x - transform.position.x, physicsPos.z - transform.position.z) * Mathf.Rad2Deg;
		float angleVert = 90;

		angleVert -= Mathf.Atan2 (physicsPos.y - transform.position.y, Vector2.Distance (new Vector2 (physicsPos.x, physicsPos.z),
			new Vector2 (transform.position.x, transform.position.z))) * Mathf.Rad2Deg;

		arcPieces [0].SetActive (true);
		arcPieces [0].transform.position = (transform.position + physicsPos) / 2;
		arcPieces [0].transform.rotation = Quaternion.Euler (new Vector3 (angleVert, angleHor, 0));
		arcPieces [0].transform.localScale = new Vector3 (arcPieces[0].transform.localScale.x, 
			Vector3.Distance (transform.position, physicsPos) / 2, arcPieces[0].transform.localScale.z);

		for (int i = 1; i < NUMBER_OF_ARC_PIECES; i++) {
			arcPieces [i].SetActive (false);
		}
	}

	protected void DisableAimArc () {

		for (int i = 0; i < arcPieces.Length; i++) {
			arcPieces [i].SetActive (false);
		}
	}

	protected void EndMotion () {

		if (!grounded) {

			Debug.Log (name);

			bm.ChangeUI (true);

			rb.velocity = Vector3.zero;
			grounded = true;

			babyModel.transform.localRotation = Quaternion.identity;

			if (vertRotation != null) {
				transform.eulerAngles = new Vector3 (0, vertRotation.eulerAngles.y, 0);
				vertRotation.localEulerAngles = new Vector3 (vertRotation.localEulerAngles.x, 0, 0);
			}

			if (current) {
				previousBaby = gameObject;
			}

			if (bm.OutOfBabies ()) {
				this.enabled = false;
			}

			StartCoroutine ("FixCollision");
			anim.SetInteger ("animState", 1);
		}
	}

	private IEnumerator FixCollision () {

		yield return new WaitForFixedUpdate ();
		yield return new WaitForFixedUpdate ();
		yield return new WaitForFixedUpdate ();
		yield return new WaitForFixedUpdate ();
		rb.isKinematic = true;
		Destroy (col);
	}

	public void Die () {

		if (current) {
			rb.isKinematic = true;
			Invoke ("SwitchToPreviousBaby", timeTilSwitchBack);
		}
			
		bm.OutOfBabies ();
		Destroy (babyModel.gameObject);

		Destroy (Instantiate (fractalizedBaby, transform.position, babyModel.rotation), timeTilFractalizedDestroyed);
	}

	private void SwitchToPreviousBaby () {

		bm.ChangeUI (true);

		Vector3 camOffsets = Camera.main.transform.localPosition;

		Camera.main.transform.parent = previousBaby.transform.FindChild ("Vertical Rotation").transform;
		Camera.main.transform.localPosition = camOffsets;
		Camera.main.transform.localEulerAngles = camRotation;

		BabyController bc = previousBaby.GetComponent<BabyController> ();
		if (bc is NeoBaby) {
			bc.enabled = false;
		}

		bc.enabled = true;
		Destroy (gameObject);
//		this.enabled = false;
	}

	public bool IsCurrent () {
		return current;
	}

	public void SetOnTreadmill (bool treadmill) {
		onTreadmill = treadmill;
	}

	protected void OnCollisionEnter (Collision other) {
		
		if (!enabled) {
			return;
		}
		
		//if the player has hit the ground
		if (other.collider.CompareTag ("Floor")) {
			//stop their movement and delete their collider
//			Invoke ("EndMotion", slideTime);
			hitGround = true;

			//make the aiming arc invisible

		} else if (other.collider.CompareTag ("Wall")) {

//			rb.velocity = new Vector3 (0, -other.relativeVelocity.y, 0);
	
		} else if (other.collider.CompareTag ("Trampoline")) {
			
			rb.velocity = Vector3.Reflect (-other.relativeVelocity, other.contacts [0].normal)
				* other.collider.GetComponent<Trampoline> ().GetEnergyConserved ();

			if (rb.velocity.y < 1) {
				Invoke ("EndMotion", slideTime);
				hitGround = true;
			}
		}
	}

	protected void OnCollisionExit (Collision other) {

		if (!enabled) {
			return;
		}

		if (other.collider.CompareTag ("Floor")) {
			hitGround = false;
		}
	}
}

