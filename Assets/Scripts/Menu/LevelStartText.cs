using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartText : MonoBehaviour {

	[TextArea(3,10)][SerializeField] private string[] texts = null;

	private Text text;
	private RawImage background;
	private RawImage commander;

	private int index;

	private BabyController bbc;

	public static float timeFromStart;
	private bool paused;
	public static bool inProgress;

	void Start () {

		text = GetComponentInChildren<Text> ();
		text.text = texts [index];

		background = GetComponent<RawImage> ();
		commander = transform.GetChild (1).GetComponent<RawImage> ();

		bbc = GameObject.FindWithTag ("Baby").GetComponent<BabyController> ();

		inProgress = true;
	}
	
	void Update () {

		if (Input.GetKeyDown (KeyCode.Escape)) {

			text.enabled = paused;
			background.enabled = paused;
			commander.enabled = paused;

			paused = !paused;
		}

		timeFromStart = Time.timeSinceLevelLoad;

		if (!paused && Input.GetMouseButtonDown (0)) {

			index++;
			if (index < texts.Length) {
				text.text = texts [index];
				return;
			}

			bbc.enabled = true;

			inProgress = false;
			timeFromStart = Time.timeSinceLevelLoad;
			GameObject.Find ("Baby Manager").GetComponent<BabyManager> ().ChangeUI (true);

			gameObject.SetActive (false);
		}
	}
}
