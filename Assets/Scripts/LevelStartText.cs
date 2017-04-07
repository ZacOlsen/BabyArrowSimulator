using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartText : MonoBehaviour {

	[TextArea(3,10)][SerializeField] private string[] texts = null;

	private Text text;
	private static bool shown;
	private int index;

	void Start () {

		if (!shown) {
			Time.timeScale = 0f;
		} else {
			gameObject.SetActive (false);
		}

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

			shown = true;
			Time.timeScale = 1f;
			gameObject.SetActive (false);
		}
	}

	public static void ChangeLevel () {
		shown = false;
	}

	public static bool GetShown () {
		return shown;
	}
}
