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

	void Start () {

		bbc = GameObject.FindGameObjectWithTag ("Baby").GetComponent<BabyController> ();
		bbc.EndMotion ();
		bbc.enabled = false;

		StartLevelPreview ();
	}
	
	void FixedUpdate () {
				
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

	public void StopLevelPreview () {

		transform.localPosition = offsets;
		transform.localEulerAngles = offsetRotation;

		bbc.enabled = true;
	//	bbc.EndMotion ();
		Destroy (this);
	}
}
