using UnityEngine;
using System.Collections;

public class CameraLevelPreview : MonoBehaviour {

	[SerializeField] private float speed = 10f;
	[SerializeField] private float errorRange = .5f;
	[SerializeField] private Transform[] viewPoints = null;

	private int index = 1;

	//vert rotation offsets
	[SerializeField] private Vector3 offsets = new Vector3 (1, .5f, -3);
	[SerializeField] private Vector3 offsetRotation = new Vector3();

	private BabyController bbc;
	private GameObject lst;

	private GameObject lpt;

	void Start () {

		bbc = GameObject.FindGameObjectWithTag ("Baby").GetComponent<BabyController> ();
		bbc.EndMotion ();
		bbc.enabled = false;

		lst = GameObject.Find ("Level Start Background");
		lst.SetActive (false);

		lpt = GameObject.Find ("Level Preview Text");
		//GameObject.Find ("Charge Background").SetActive (false);
		//GameObject.Find ("Current Baby").SetActive (false);

		GameObject.Find ("Baby Manager").GetComponent<BabyManager> ().ChangeUI (false);

		StartLevelPreview ();

		//hot fix for forcing bow showing at level start
		GameObject.Find ("Bow").GetComponent<SkinnedMeshRenderer> ().enabled = true;
	}

	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			StopLevelPreview ();
		}
	}

	void FixedUpdate () {

		LevelStartText.timeFromStart = Time.timeSinceLevelLoad;

		if (index < viewPoints.Length) {

			transform.position = Vector3.Lerp (transform.position, viewPoints [index].position, 
				Time.fixedDeltaTime * speed / Vector3.Distance (transform.position, viewPoints [index].position));
			
			transform.rotation = Quaternion.Slerp (transform.rotation, viewPoints [index].rotation, 
				Time.fixedDeltaTime * speed / Vector3.Distance (transform.position, viewPoints [index].position));

			if (Vector3.Distance (transform.position, viewPoints [index].position) < errorRange &&
			   Vector3.Distance (transform.eulerAngles, viewPoints [index].eulerAngles) < errorRange) {
				index++;
			}
		
		} else {
			StartLevelPreview ();
		}
	}

	void StartLevelPreview () {

		transform.position = viewPoints [0].position;
		transform.rotation = viewPoints [0].rotation;

		index = 1;
	}

	void StopLevelPreview () {

		transform.localPosition = offsets;
		transform.localEulerAngles = offsetRotation;

		lst.SetActive (true);
		lpt.SetActive (false);

		Destroy (this);
	}
}
