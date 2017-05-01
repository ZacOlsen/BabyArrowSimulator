﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.IO;

public class MenuController : MonoBehaviour {

	[SerializeField] private GameObject missions = null;
	[SerializeField] private GameObject mainMenu = null;
	[SerializeField] private GameObject options = null;

	private bool freeplay;
	private static float musicLevel = .4f;
	private static float fxLevel = 1f;

	[SerializeField] private GameObject missionButtonObject = null;

	public void Play () {
		freeplay = true;
		Missions ();
	}

	public void Missions () {

		BinaryReader br = new BinaryReader (new FileStream("Assets\\Game File IO\\Mission_Unlocks.sav", FileMode.Open));
		int unlocks = br.ReadInt32 ();
		br.Close ();

		for (int i = 0; i < missionButtonObject.transform.childCount; i++) {
			missionButtonObject.transform.GetChild (i).GetComponent<Button> ().interactable = i < unlocks;
		}
		
		mainMenu.SetActive (false);
		missions.SetActive (true);
	}

	public void UnlockMissions (string missions) {

		int unlocks;
		if (!int.TryParse (missions, out unlocks)) {
			return;
		}
			
		UnlockMissions (unlocks);
		Missions ();
	}

	public static void UnlockMissions (int unlocks) {

		BinaryWriter bw = new BinaryWriter (new FileStream ("Assets\\Game File IO\\Mission_Unlocks.sav", FileMode.Create));
		bw.Write (unlocks);
		bw.Close ();
	}

	public void ResetMissions () {

		BinaryWriter bw = new BinaryWriter (new FileStream ("Assets\\Game File IO\\Mission_Unlocks.sav", FileMode.Create));
		bw.Write (1);
		bw.Close ();

		Missions ();
	}

	public void PlayMission (string name) {

		name = (freeplay ? "Freeplay " : "Mission ") + name;
		SceneManager.LoadScene (name);
	}

	public void Options () {

		mainMenu.SetActive (false);
		options.SetActive (true);
	}

	public void UpdateMusicLevel (float level) {
		musicLevel = level;
	}

	public void UpdateFXLevel (float level) {
		fxLevel = level;
	}

	public static void SetMusicLevel (float level) {
		musicLevel = level;
	}

	public static void SetFXLevel (float level) {
		fxLevel = level;
	}

	public void MainMenu () {

		options.SetActive (false);
		missions.SetActive (false);
		mainMenu.SetActive (true);

		freeplay = false;
	}

	public static float GetMusicLevel () {
		return musicLevel;
	}

	public static float GetFXLevel () {
		return fxLevel;
	}

	public void Quit () {
		Application.Quit ();
	}
}
