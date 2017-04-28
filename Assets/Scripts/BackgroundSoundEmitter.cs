using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSoundEmitter : MonoBehaviour {

	[SerializeField] private AudioClip backgroundSound = null;
	private AudioSource source;
	private static float timeInClip = 0f;

	void Start () {
		
		source = GetComponent<AudioSource> ();
		source.loop = true;
		source.clip = backgroundSound;

		source.Play ();
		source.time = timeInClip;
	}

	void Update () {
		source.volume = MenuController.GetMusicLevel ();
		timeInClip = source.time;
	}
}
