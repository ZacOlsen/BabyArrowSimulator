using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartText : MonoBehaviour {

	private static bool shown;

	void Start () {

		if (!shown) {
			Time.timeScale = 0f;
		} else {
			gameObject.SetActive (false);
		}
	}
	
	void Update () {

		if (Input.GetMouseButtonDown (0)) {

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
