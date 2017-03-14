using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSoundEmitter : MonoBehaviour {

	[SerializeField] private AudioClip backgroundSound = null;
	private AudioSource source;

	void Start () {
		
		source = GetComponent<AudioSource> ();
		source.loop = true;
		source.clip = backgroundSound;
		source.Play ();
	}

	void FixedUpdate () {
		source.volume = MenuController.GetAudioLevel ();
	}
}
