using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialController : MonoBehaviour {

	[TextArea(3, 10)][SerializeField] private string[] messages = null;
	[SerializeField] private Sprite[] images = null;

	[SerializeField] private GameObject tempWall = null;
	private int index;

	private Text textBox;
	private RawImage image;
	[SerializeField] private GameObject background = null;
	private bool babyInArea;

	private void Start () {
		background.SetActive (false);
	}

	private void ShowTutorial () {
		
		background.SetActive (true);
		textBox = GameObject.Find ("Text").GetComponent<Text> ();
		image = GameObject.Find ("Image").GetComponent<RawImage> ();
		
		textBox.text = messages [index];
		image.texture = images [index].texture;
		
		Time.timeScale = 0;
	}
	
	private void Update () {
	
		if (babyInArea && Input.GetMouseButtonDown(0)) {

			index++;

			if (index >= messages.Length) {
				
				gameObject.SetActive (false);
				Time.timeScale = 1f;

				if (tempWall != null) {
					Destroy (tempWall);
				}

				background.SetActive (false);
				Destroy (this);
				return;
			}

			textBox.text = messages [index];
			image.texture = images [index].texture;
		}
	}

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			babyInArea = true;
			ShowTutorial ();
		}
	}
}
