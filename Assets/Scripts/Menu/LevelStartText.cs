using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartText : MonoBehaviour {

	[TextArea(3,10)][SerializeField] private string[] texts = null;

	private Text text;
	private int index;

	private BabyController bbc;

	public static float timeFromStart;

	void Start () {

		text = GetComponentInChildren<Text> ();
		text.text = texts [index];

		bbc = GameObject.FindWithTag ("Baby").GetComponent<BabyController> ();
	}
	
	void Update () {

		timeFromStart = Time.timeSinceLevelLoad;

		if (Input.GetMouseButtonDown (0)) {

			index++;
			if (index < texts.Length) {
				text.text = texts [index];
				return;
			}

			bbc.enabled = true;

			timeFromStart = Time.timeSinceLevelLoad;
			GameObject.Find ("Baby Manager").GetComponent<BabyManager> ().ChangeUI (true);

			gameObject.SetActive (false);
		}
	}
}
