using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BabyManager : MonoBehaviour {

	//0 = default
	//1 = bouncy
	//2 = sticky
	//3 = neo
	[SerializeField] private GameObject[] babies = null;
	[SerializeField] private int[] babiesTable = null;
	private int[] babiesUsedNum;
	private int currentIndex;

	[SerializeField] private Color highlighted = Color.white;
	[SerializeField] private Color unhighlighted = Color.black;

	[SerializeField] private string mainMenuSceneName = "";
	[SerializeField] private string nextLevelSceneName = "";
	private GameObject menuBackground;

	private GameObject babySelector;
	private RawImage[] babyImages;
	private Text[] numbersToDisplay;

	private Text levelTime;
	private bool levelOver;

	private bool isOnSoldierBaby;

	private void Start () {

		babiesUsedNum = new int [babiesTable.Length];

		menuBackground = GameObject.Find ("Menu Background");
		menuBackground.SetActive (false);

		babySelector = GameObject.Find ("Baby Selector");
		babyImages = new RawImage[babies.Length];
		numbersToDisplay = new Text[babies.Length];

		for (int i = 0; i < babies.Length; i++) {

			babyImages[i] = babySelector.transform.GetChild (i).GetComponent<RawImage> ();
			numbersToDisplay [i] = babySelector.transform.GetChild (i).GetComponentInChildren<Text> ();

			babyImages [i].color = i == 0 ? highlighted : unhighlighted;
			UpdateNumberDisplay (i);
		}

		levelTime = GameObject.Find ("Level Time").GetComponent<Text> ();
	}

	private void Update () {

		//mouse lock and unlock mouse on pause
		if (Input.GetKeyDown(KeyCode.Escape)) {

			if (Cursor.visible == false) {
				ShowMenu ();
				Time.timeScale = 0;
			} else {
				HideMenu ();
				Time.timeScale = 1;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		if (!levelOver) {

			levelTime.text = string.Format ("{0:0.00}", Time.timeSinceLevelLoad);

			if (Input.GetAxisRaw ("Mouse ScrollWheel") != 0) {
				UpdateSelection (Input.GetAxisRaw ("Mouse ScrollWheel") > 0 ? -1 : 1);
			}
		}
	}

	private void UpdateSelection (int increment) {

		int previous = currentIndex;
		currentIndex += increment;

		if (currentIndex >= babies.Length) {
			currentIndex = 0;
		}

		if (currentIndex < 0) {
			currentIndex = babies.Length - 1;
		}

		do {
			
			if (babiesTable [currentIndex] == 0) {
				currentIndex += increment;	
			} else {
				break;
			}

			if (currentIndex >= babies.Length) {
				currentIndex = 0;
			}
			
			if (currentIndex < 0) {
				currentIndex = babies.Length - 1;
			}
		
		} while (currentIndex != previous);

		if (currentIndex >= babies.Length) {
			currentIndex = 0;
		}

		babyImages [previous].color = unhighlighted;
		babyImages [currentIndex].color = highlighted;

		isOnSoldierBaby = babies[currentIndex].GetComponent<SoldierBaby>() != null;
	}

	private void UpdateNumberDisplay (int index) {

		numbersToDisplay [index].text = "x" + babiesTable [index];
	}

	public void RetryLevel () {

		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void GotToMainMenu () {

		SceneManager.LoadScene (mainMenuSceneName);
	}

	public void GoToNextLevel () {

		SceneManager.LoadScene (nextLevelSceneName);
	}

	private void HideMenu () {

		menuBackground.SetActive (false);

		levelTime.rectTransform.SetParent (GameObject.Find ("Canvas").transform);

		levelTime.rectTransform.anchorMax = new Vector2 (.5f, 1f);
		levelTime.rectTransform.anchorMin = new Vector2 (.5f, 1f);

		levelTime.rectTransform.anchoredPosition = new Vector2 (0, -15);

		for (int i = 0; i < babies.Length; i++) {
			babyImages [i].enabled = true;
			numbersToDisplay [i].enabled = true;
		}

		GameObject.Find ("Canvas").transform.FindChild ("Charge Background").gameObject.SetActive (true);
	}

	private void ShowMenu () {
	
		if (levelOver) {
			return;
		}

		for (int i = 0; i < babies.Length; i++) {
			babyImages [i].enabled = false;
			numbersToDisplay [i].enabled = false;
		}
			
		menuBackground.SetActive (true);

		GameObject babiesUsed = GameObject.Find ("Babies Used");
		for (int i = 0; i < babiesUsedNum.Length; i++) {
			babiesUsed.transform.GetChild (i).GetComponentInChildren<Text> ().text = "x" + babiesUsedNum [i];
		}

		levelTime.rectTransform.SetParent (menuBackground.transform);

		levelTime.rectTransform.anchorMax = new Vector2 (.5f, 1f);
		levelTime.rectTransform.anchorMin = new Vector2 (.5f, 1f);

		levelTime.rectTransform.anchoredPosition = new Vector2 (0, -50);

		GameObject.Find ("Charge Background").SetActive (false);

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void ShowVictory () {

		ShowMenu ();
		levelOver = true;
		GameObject.Find ("Level Name").GetComponent<Text> ().text = SceneManager.GetActiveScene().name + " Completed";
	}

	private void ShowDefeat () {

		ShowMenu ();
		levelOver = true;
		GameObject.Find ("Level Name").GetComponent<Text> ().text = SceneManager.GetActiveScene().name + " Failed";
	}

	public bool OutOfBabies () {

		for (int i = 0; i < babiesTable.Length; i++) {

			if (babiesTable[i] != 0) {
				return false;
			}
		}

		ShowDefeat ();
		return true;
	}

	public GameObject GetNextBaby () {

		if (babiesTable [currentIndex] > 0) {

			GameObject baby = babies [currentIndex];

			babiesUsedNum [currentIndex]++;
			babiesTable [currentIndex]--;
			UpdateNumberDisplay (currentIndex);

			if (babiesTable [currentIndex] == 0) {
				UpdateSelection (1);
			}

			return baby;
		}

		return null;
	}

	public bool NextIsSoldierBaby () {
		return isOnSoldierBaby;
	}

	public void ChangeUI (bool shown) {

		for (int i = 0; i < babies.Length; i++) {
			babyImages [i].enabled = shown;
			numbersToDisplay [i].enabled = shown;
		}
	}
}
