using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialController : MonoBehaviour {

	[SerializeField] private string[] messages = null;
	[SerializeField] private Sprite[] images = null;

	[SerializeField] private GameObject tempWall = null;
	[SerializeField] private bool defaultlyActive = false;

	private int index;

	private Text textBox;
	private RawImage image;

	private void Start () {
	
		if (defaultlyActive) {
			ShowTutorial ();
		}
	}

	private void ShowTutorial () {

		textBox = GameObject.Find ("Text").GetComponent<Text> ();
		image = GameObject.Find ("Image").GetComponent<RawImage> ();
		
		textBox.text = messages [index];
		image.texture = images [index].texture;
		
		Time.timeScale = 0;
		defaultlyActive = true;
	}
	
	private void Update () {
	
		if (defaultlyActive && Input.GetMouseButtonDown(0)) {

			index++;

			if (index >= messages.Length) {
				
				gameObject.SetActive (false);
				Time.timeScale = 1f;

				if (tempWall != null) {
					Destroy (tempWall);
				}

				Destroy (this);
				return;
			}

			textBox.text = messages [index];
			image.texture = images [index].texture;
		}
	}

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			ShowTutorial ();
		}
	}
}
