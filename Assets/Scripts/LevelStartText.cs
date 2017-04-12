using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartText : MonoBehaviour {

	[TextArea(3,10)][SerializeField] private string[] texts = null;

	private Text text;
	private int index;

	void Start () {

		text = GetComponentInChildren<Text> ();
		text.text = texts [index];
	}
	
	void Update () {

		if (Input.GetMouseButtonDown (0)) {

			index++;
			if (index < texts.Length) {
				text.text = texts [index];
				return;
			}

			Camera.main.GetComponent<CameraLevelPreview> ().StopLevelPreview();
			gameObject.SetActive (false);
		}
	}
}
