using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

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
	private float powerDecrease;

	private float launchX = 0;

	/**
	 * current launch speed of the baby based on the charge
	 */
	private float launchSpeed;

	/**
	 * this gameobjects rigidbody
	 */
	protected Rigidbody rb;

	/**
	 * the collider of this gameobject
	 */
	protected BoxCollider col;

	/**
	 * the charging part of the chargebar
	 */
	private Image chargeBar;

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

	protected BabyManager bm;
	private static GameObject previousBaby;

	private bool current = true;
	protected bool aiming;
	private bool onTreadmill;
	protected bool grounded;
	protected bool hitGround;
	private bool velIsZero;
	protected bool dying;

	protected int velCounter;

	protected Transform babyModel;
	[SerializeField] private GameObject fractalizedBaby = null;
	[SerializeField] private float timeTilFractalizedDestroyed = 5f;
	[SerializeField] private float timeTilSwitchBack = 3f;

	[SerializeField] private Vector3 camRotation = new Vector3();

	private List<GameObject> objectsInWay;

	protected Animator anim;

	[SerializeField] protected SkinnedMeshRenderer bow = null;
	[SerializeField] protected SkinnedMeshRenderer gun = null;
	[SerializeField] private Transform spine = null;

	[SerializeField] protected AudioClip[] wallHitSounds = null;
	[SerializeField] private AudioClip bowReleaseSound = null;
	[SerializeField] private AudioClip splatterSound = null;
	protected AudioSource audioPlayer;

	protected void Awake () {

		rb = GetComponent<Rigidbody> ();

		babyModel = transform.FindChild ("Baby Model");
		col = transform.GetComponentInChildren<BoxCollider> ();

		bm = GameObject.Find ("Baby Manager").GetComponent<BabyManager> ();
	}
		
	protected void Start () {

		rb.freezeRotation = true;
		anim = babyModel.GetComponentInChildren<Animator> ();

		Camera.main.transform.localEulerAngles = camRotation;

		chargeBar = GameObject.Find ("Charge Foreground").GetComponent<Image> ();

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

		audioPlayer = GetComponent<AudioSource> ();

		objectsInWay = new List<GameObject> ();
	}
		
	protected void Update () {

		if (Time.timeScale > 0) {
			UpdateLooking ();

			if (grounded) {

				if (!dying) {
					gun.enabled = bm.NextIsSoldierBaby ();
					bow.enabled = !gun.enabled;
				}

				Aim ();
			}
		}
	}

	void LateUpdate () {
		
		if (grounded && !dying) {
			spine.localRotation = Quaternion.Euler (0, 0, Mathf.Clamp (-rotationY, minRotationY, maxRotationY));
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
				}

				if (rb.velocity.magnitude < .5f) {
					velCounter++;
				} else {
					velCounter = 0;
				}
			}
		}


		if (Camera.main.transform.IsChildOf (vertRotation)) {

			Vector3 direction = transform.position - Camera.main.transform.position;
			float length = direction.magnitude;

			Ray rayMid = new Ray (Camera.main.transform.position, direction);
			RaycastHit[] hitsMid = Physics.RaycastAll (rayMid, length, ~(1 << 8));//~LayerMask.NameToLayer ("BabyL"));

			ResetObjectsInWay ();
			//Debug.DrawLine (ray.origin, transform.position, Color.red);

			Vector3 pointTL = Camera.main.ViewportPointToRay (Vector3.up).GetPoint (length);
			Ray rayTL = new Ray (Camera.main.transform.position, pointTL - Camera.main.transform.position);
			RaycastHit[] hitsTL = Physics.RaycastAll (rayTL, length, ~(1 << 8));

			Vector3 pointBL = Camera.main.ViewportPointToRay (Vector3.zero).GetPoint (length);
			Ray rayBL = new Ray (Camera.main.transform.position, pointBL - Camera.main.transform.position);
			RaycastHit[] hitsBL = Physics.RaycastAll (rayBL, length, ~(1 << 8));

			Vector3 pointTR = Camera.main.ViewportPointToRay (new Vector3 (1, 1, 0)).GetPoint (length);
			Ray rayTR = new Ray (Camera.main.transform.position, pointTR - Camera.main.transform.position);
			RaycastHit[] hitsTR = Physics.RaycastAll (rayTR, length, ~(1 << 8));

			Vector3 pointBR = Camera.main.ViewportPointToRay (Vector3.right).GetPoint (length);
			Ray rayBR = new Ray (Camera.main.transform.position, pointBR - Camera.main.transform.position);
			RaycastHit[] hitsBR = Physics.RaycastAll (rayBR, length, ~(1 << 8));

			RaycastHit[] hits = new RaycastHit[hitsMid.Length + hitsTL.Length + hitsBL.Length + hitsBR.Length + hitsTR.Length];

			Array.Copy (hitsMid, 0, hits, 0, hitsMid.Length);
			Array.Copy (hitsTL, 0, hits, hitsMid.Length, hitsTL.Length);
			Array.Copy (hitsBL, 0, hits, hitsTL.Length, hitsBL.Length);
			Array.Copy (hitsTR, 0, hits, hitsBL.Length, hitsTR.Length);
			Array.Copy (hitsBR, 0, hits, hitsTR.Length, hitsBR.Length);

			Debug.Log (hits.Length);
			for (int i = 0; i < hits.Length; i++) {

				if (hits[i].collider != null && !objectsInWay.Contains (hits [i].collider.gameObject)) {
				
					//	Debug.Log (objectsInWay[i]);
					objectsInWay.Add (hits [i].collider.gameObject);
					MaterialSwapper mesh = hits [i].collider.GetComponent<MaterialSwapper> ();
					if(mesh != null){
						mesh.enabled = false;
						mesh.SwapToTransparent();
					}
				}
			}
		}
	}

	protected void ResetObjectsInWay () {

		for (int i = 0; i < objectsInWay.Count; i++) {

			MaterialSwapper mesh = objectsInWay [i].GetComponent<MaterialSwapper> ();
			if(mesh != null){
				mesh.enabled = true;
				mesh.SwapToOpaque ();
			}
		}

		objectsInWay.Clear ();
	}

	protected void UpdateLooking () {

		float x = 0;
		x = Input.GetAxis ("Mouse X") * sensitivity;

		rotationY -= Input.GetAxis ("Mouse Y") * sensitivity;

		if (grounded) {
			
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y + x, 0f);
			vertRotation.transform.localRotation = Quaternion.Euler (Mathf.Clamp (rotationY, minRotationY, maxRotationY), 0, 0);

			if (!dying) {
				anim.SetBool ("shuffle", x != 0 && !bm.NextIsSoldierBaby ());
				spine.localRotation = Quaternion.Euler (0, 0, Mathf.Clamp (-rotationY, minRotationY, maxRotationY));
//				Debug.Log (spine.localEulerAngles);
//				anim.SetBool ("gun", bm.NextIsSoldierBaby ());
			}

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

		if (!dying) {
			anim.SetFloat ("blendPB", 0);
		}

		//uncommenting this line will make the aim arc invis on launch and right click
		DisableAimArc();
	}

	protected void ShootBaby (GameObject baby, bool soldier = false) {

		if (baby != null && !dying) {

			bm.ChangeUI (false);

			baby = ((GameObject) Instantiate (baby, new Vector3(launchStartPos.position.x, launchStartPos.position.y - .3f, launchStartPos.position.z), transform.rotation));
			baby.transform.FindChild ("Vertical Rotation").transform.localRotation = vertRotation.localRotation;

			if (col != null) {
				Physics.IgnoreCollision (col, baby.GetComponentInChildren<Collider> ());
			}

			Rigidbody rig = baby.GetComponent<Rigidbody> ();

			Vector3 vel = vertRotation.TransformDirection (new Vector3 (0, 0, launchSpeed * (1 - powerDecrease)));
		
			if (soldier) {
				baby.GetComponent<SoldierBaby> ().SetStats (launchSpeed / maxLaunchSpeed, vel);

			} else {
				audioPlayer.PlayOneShot (bowReleaseSound, MenuController.GetFXLevel ());
				rig.velocity = vel;
			}

			baby.GetComponent<BabyController> ().rotationY = rotationY;

			Destroy (GetComponentInChildren<Camera>().gameObject);
			launchSpeed = 0;

			this.enabled = false;

			current = false;
			ResetShootState ();

			anim.SetBool ("shuffle", false);

			ResetObjectsInWay ();
		}
	}

	/**
	 * Updates the chargeBar based on how much it is currently charged.
	 */
	private void UpdateChargeBar () {

		float percent = launchSpeed / maxLaunchSpeed;
		chargeBar.fillAmount = percent;
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

		//neo baby stuff
		if (!dying) {
			anim.SetFloat ("blendPB", Mathf.Abs (launchX));
		}

		UpdateChargeBar ();

		if (bm.NextIsSoldierBaby () && !regularArc) {
			SoldierBabyArc ();
			return;
		}

		float launchPower = launchSpeed * (1 - powerDecrease);

		float initialYVel = launchStartPos.TransformDirection (new Vector3(0, 0, launchPower)).y;

		float time = 2 * (-initialYVel / Physics.gravity.y) + (-initialYVel + Mathf.Sqrt(Mathf.Pow(initialYVel, 2) - 
			4 * -Physics.gravity.y / 2 * -(transform.position.y))) / -Physics.gravity.y;

		float initialXVel = launchPower * Mathf.Cos (vertRotation.localEulerAngles.x * Mathf.Deg2Rad);

		float distance = 0;
		for (float i = .1f; i < time; i += .01f) {
			
			distance = Mathf.Sqrt (Mathf.Pow(initialXVel * i, 2) + Mathf.Pow(.5f * -Physics.gravity.y * i * i + initialYVel * i + transform.position.y, 2));
			if (distance > maxArcDistance) {
				time = i;
			}
		}
			
		Vector3 landPos = transform.TransformDirection (new Vector3 (0, 0, distance)) + launchStartPos.position;
		Vector3 previousPoint = transform.position;

		int counter = 0;
		float increment = time / (float) arcPieces.Length;
		for (float i = increment; i <= time + .01f; i += increment, counter++) {

			if (counter >= arcPieces.Length || dying) {
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

	public void SetPowerDecrease (float decrease) {
		powerDecrease = decrease;
	}

	public void EndMotion () {

		Time.timeScale = 1f;

		if (!grounded && !dying) {

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

			anim.SetInteger ("animState", 1);

			rb.isKinematic = true;
			Destroy (col);
		}
	}

	public void Die () {

		Time.timeScale = 1f;
		DisableAimArc ();

		audioPlayer.PlayOneShot (splatterSound);
		dying = true;

		if (current) {
			rb.isKinematic = true;

			if (bm.OutOfBabies ()) {
				enabled = false;
			} else {
				Invoke ("SwitchToPreviousBaby", timeTilSwitchBack);
			}
		}
			
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

		ResetObjectsInWay ();
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
			
		audioPlayer.PlayOneShot (wallHitSounds[UnityEngine.Random.Range(0, wallHitSounds.Length - 1)], MenuController.GetFXLevel ());

		//if the player has hit the ground
		if (other.collider.CompareTag ("Floor")) {
			hitGround = true;

		} else if (other.collider.CompareTag ("Wall")) {

		} else if (other.collider.CompareTag ("Trampoline")) {
			
			rb.velocity = Vector3.Reflect (-other.relativeVelocity, other.contacts [0].normal)
				* other.collider.GetComponent<Trampoline> ().GetEnergyConserved ();

			if (rb.velocity.y < 1) {
				hitGround = true;
			}
		}

		if (other.contacts[0].point.y < transform.position.y && !other.collider.CompareTag ("Wall")) {
			hitGround = true;
		}
	}

	protected void OnCollisionStay (Collision other) {

		if (velCounter >= 8 && !grounded) {

			RaycastHit hit;

			Matrix4x4 mat = babyModel.GetChild (0).localToWorldMatrix;
			Vector3 pointUR = mat.MultiplyPoint3x4 (col.center + col.size / 2f);
			Vector3 pointUL = mat.MultiplyPoint3x4 (col.center + new Vector3(-col.size.x, col.size.y, col.size.z) / 2f);
			Vector3 pointBR = mat.MultiplyPoint3x4 (col.center + new Vector3(col.size.x, -col.size.y, col.size.z) / 2f);
			Vector3 pointBL = mat.MultiplyPoint3x4 (col.center + new Vector3(-col.size.x, -col.size.y, col.size.z) / 2f);

			bool succeed = false;

			if (Physics.Raycast (transform.position, -Vector3.up, out hit, col.size.z / 2f + .1f, ~(1 << 8))) {
				succeed = true;
			} else if (Physics.Raycast (pointBR, -Vector3.up, out hit, .1f, ~(1 << 8))) {
				succeed = true;
			} else if (Physics.Raycast (pointBL, -Vector3.up, out hit, .1f, ~(1 << 8))) {
				succeed = true;
			} else if (Physics.Raycast (pointUR, -Vector3.up, out hit, .1f, ~(1 << 8))) {
				succeed = true;
			} else if (Physics.Raycast (pointUL, -Vector3.up, out hit, .1f, ~(1 << 8))) {
				succeed = true;
			}

			Vector3 point = Vector3.zero;

			if (succeed) {
				point = hit.point;
		
			} else {
				
				for (int i = 0; i < other.contacts.Length; i++) {
					point += other.contacts [i].point;
				}
				
				point /= other.contacts.Length;
			}

			transform.position = new Vector3 (point.x, point.y + col.size.y / 2f + .05f, point.z);
			babyModel.transform.localRotation = Quaternion.identity;

			EndMotion ();
		}
	}

	protected void OnCollisionExit (Collision other) {

		if (!enabled) {
			return;
		}

		if (other.collider.CompareTag ("Floor")) {
	//		hitGround = false;
		}
	}
}

