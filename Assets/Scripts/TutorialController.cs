using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialController : MonoBehaviour {

	[SerializeField] private string[] messages = null;
	[SerializeField] private Sprite[] images = null;

	private int index;

	private Text textBox;
	private RawImage image;

	void Start () {
	
		textBox = GetComponentInChildren<Text> ();
		image = transform.FindChild ("Image").GetComponent<RawImage> ();

		textBox.text = messages [index];
		image.texture = images [index].texture;

		Time.timeScale = 0;
	}
	
	void Update () {
	
		if (Input.GetMouseButtonDown(0)) {

			index++;

			if (index >= messages.Length) {
				
				gameObject.SetActive (false);
				Time.timeScale = 1f;
				Destroy (this);
				return;
			}

			textBox.text = messages [index];
			image.texture = images [index].texture;
		}
	}
}
