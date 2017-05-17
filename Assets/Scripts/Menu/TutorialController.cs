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
	private RawImage backgroundImage;
	private bool babyInArea;

	private Rigidbody rb;
	private BabyManager bm;
	private bool paused;

	private void Start () {
		
		backgroundImage = background.GetComponent<RawImage> ();
		textBox = GameObject.Find ("BText").GetComponent<Text> ();
		image = GameObject.Find ("Commander").GetComponent<RawImage> ();

		//background.SetActive (false);
		ChangeText (false);

		bm = GameObject.Find ("Baby Manager").GetComponent<BabyManager> ();
	}

	private void ShowTutorial () {
		
		//background.SetActive (true);
		ChangeText (true);
		textBox.text = messages [index];
		image.texture = images [index].texture;

		bm.ChangeUI (false);

		WaitOneFrame ();
		Time.timeScale = 0;
	}

	//cancer being worked on
	private IEnumerable WaitOneFrame(){
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		Time.timeScale = 0;
	}

	private void ChangeText (bool shown) {

		backgroundImage.enabled = shown;
		textBox.enabled = shown;
		image.enabled = shown;
	}

	private void Update () {
	
		if (babyInArea && Input.GetKeyDown (KeyCode.Escape)) {

			backgroundImage.enabled = paused;
			textBox.enabled = paused;
			image.enabled = paused;

			paused = !paused;
		}

		if (!paused && babyInArea && Input.GetMouseButtonDown(0)) {

			index++;

			if (index >= messages.Length) {
				
				gameObject.SetActive (false);
				Time.timeScale = 1f;

				if (tempWall != null) {
					Destroy (tempWall);
				}

				//background.SetActive (false);
				ChangeText (false);
				bm.ChangeUI (true);
				rb.GetComponent<BabyController> ().enabled = true;
				Destroy (this);
				return;
			}

			textBox.text = messages [index];
			image.texture = images [index].texture;
		}
	}

	private void FixedUpdate () {

		if (rb != null && rb.isKinematic) {
			rb.GetComponent<BabyController> ().enabled = false;
			ShowTutorial ();
		}
	}

	private void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Baby")) {
			babyInArea = true;
			rb = other.attachedRigidbody;
		}
	}
}
