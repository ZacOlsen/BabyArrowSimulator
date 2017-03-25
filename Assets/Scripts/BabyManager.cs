using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BabyManager : MonoBehaviour {

	[SerializeField] private GameObject[] babies = null;
	[SerializeField] private Sprite[] babyImages = null;
	[SerializeField] private int[] babiesTable = null;
	private int[] babiesUsedNum;
	private int currentIndex;

//	[SerializeField] private Color highlighted = Color.white;
//	[SerializeField] private Color unhighlighted = Color.black;

	[SerializeField] private string mainMenuSceneName = "";
	[SerializeField] private string nextLevelSceneName = "";
	private GameObject menuBackground;

	private RawImage currentBabySelected;
	private Text currentBabyNumbers;
	private Text totalBabyNumbers;

	[SerializeField] private GameObject babyMenuTemplate = null;
	private RawImage[] babyMenuImages;
	private Text[] babyMenuNumbers;

	private Text levelTime;
	private bool levelOver;

	private bool isOnSoldierBaby;

	private GameObject nextLevelButton;
	private GameObject optionsMenu;
	private Slider audioLevel;
	private GameObject optionsButton;
	private GameObject babiesUsed;

	private void Start () {

		babiesUsedNum = new int [babiesTable.Length];

		currentBabySelected = GameObject.Find ("Current Baby").GetComponent<RawImage> ();
		currentBabyNumbers = GameObject.Find ("Current Numbers").GetComponent<Text> ();
		totalBabyNumbers = GameObject.Find ("Total Numbers").GetComponent<Text> ();

		currentBabySelected.texture = babyImages [0].texture;

		babyMenuImages = new RawImage [babies.Length];
		babyMenuNumbers = new Text [babies.Length];
		UpdateNumberDisplay (currentIndex);

		babiesUsed = GameObject.Find ("Babies Used");
		for (int i = 0; i < babyMenuImages.Length; i++) {

			GameObject go = (GameObject)Instantiate (babyMenuTemplate);
			go.transform.SetParent (babiesUsed.transform);
			go.transform.localPosition = new Vector3 (i * 100f - (200f * (babies.Length - 1f) / 4f), 0, 0);

			babyMenuImages [i] = go.GetComponent<RawImage> ();
			babyMenuImages [i].texture = babyImages [i].texture;

			babyMenuNumbers [i] = go.GetComponentInChildren<Text> ();
			babyMenuNumbers [i].text = "x" + babiesUsedNum [i];
		}
			
		levelTime = GameObject.Find ("Level Time").GetComponent<Text> ();

		nextLevelButton = GameObject.Find ("Next Level");
		optionsMenu = GameObject.Find ("Options Menu");
		audioLevel = GameObject.Find ("Audio Slider").GetComponent<Slider> ();
		optionsButton = GameObject.Find ("Options");

		optionsMenu.SetActive (false);
		nextLevelButton.SetActive (false);

		menuBackground = GameObject.Find ("Menu Background");
		menuBackground.SetActive (false);
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

		currentBabySelected.texture = babyImages [currentIndex].texture;
		currentBabyNumbers.text = "x" + babiesTable [currentIndex];

		isOnSoldierBaby = babies[currentIndex].GetComponent<SoldierBaby>() != null;
	}

	private void UpdateNumberDisplay (int index) {

		currentBabyNumbers.text = "x" + babiesTable [index];

		int sum = 0;
		for (int i = 0; i < babiesTable.Length; i++) {
			sum += babiesTable [i];
		}

		totalBabyNumbers.text = "x" + sum;
	}

	public void RetryLevel () {
		
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		Time.timeScale = 1;
	}

	public void GotToMainMenu () {
	
		LevelStartText.ChangeLevel ();
		SceneManager.LoadScene (mainMenuSceneName);
	}

	public void GoToNextLevel () {
	
		LevelStartText.ChangeLevel ();
		SceneManager.LoadScene (nextLevelSceneName);
	}

	public void ShowOptions () {

		optionsMenu.SetActive (true);
		audioLevel.value = MenuController.GetAudioLevel ();
		babiesUsed.SetActive (false);
	}

	public void HideOptions () {

		optionsMenu.SetActive (false);
		babiesUsed.SetActive (true);
	}

	public void SetAudioLevel (float al) {
		MenuController.SetAudioLevel (al);
	}

	public void ToggleShowTimer (bool shown) {
		levelTime.color = new Color(levelTime.color.r, levelTime.color.g, levelTime.color.b, shown ? 255 : 0);
	}

	private void HideMenu () {

		if (optionsMenu.activeSelf) {
			HideOptions ();
		}

		menuBackground.SetActive (false);

		levelTime.rectTransform.SetParent (GameObject.Find ("Canvas").transform);

		levelTime.rectTransform.anchorMax = new Vector2 (1f, 1f);
		levelTime.rectTransform.anchorMin = new Vector2 (1f, 1f);

		levelTime.rectTransform.anchoredPosition = new Vector2 (-90, -15);
		levelTime.alignment = TextAnchor.MiddleRight;

		currentBabySelected.enabled = true;
		currentBabyNumbers.enabled = true;
		totalBabyNumbers.enabled = true;

		GameObject.Find ("Canvas").transform.FindChild ("Charge Background").gameObject.SetActive (true);
	}

	private void ShowMenu () {
	
		if (levelOver) {
			return;
		}

		currentBabySelected.enabled = false;
		currentBabyNumbers.enabled = false;
		totalBabyNumbers.enabled = false;
			
		menuBackground.SetActive (true);

		GameObject babiesUsed = GameObject.Find ("Babies Used");
		for (int i = 0; i < babiesUsedNum.Length; i++) {
			babiesUsed.transform.GetChild (i).GetComponentInChildren<Text> ().text = "x" + babiesUsedNum [i];
		}

		levelTime.rectTransform.SetParent (menuBackground.transform);

		levelTime.rectTransform.anchorMax = new Vector2 (.5f, 1f);
		levelTime.rectTransform.anchorMin = new Vector2 (.5f, 1f);

		levelTime.rectTransform.anchoredPosition = new Vector2 (0, -50);
		levelTime.alignment = TextAnchor.MiddleCenter;

		GameObject.Find ("Charge Background").SetActive (false);

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void ShowVictory () {

		ShowMenu ();

		optionsButton.SetActive (false);
		nextLevelButton.SetActive (true);

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

		currentBabySelected.enabled = shown;
		currentBabyNumbers.enabled = shown;
		totalBabyNumbers.enabled = shown;
	}
}
