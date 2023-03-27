using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationController : MonoBehaviour {
	#region Settings
	public Slider cameraSpeedSlider;
	public TextMeshProUGUI cameraSpeedSliderText;
	public CameraController mainCamera;
	public Popup generalPopup;
	public GameObject login;
	public SheetSync server;
	public string Username { private get; set; }
	public string Password { private get; set; }
	internal bool loggedIn = false;
	internal bool admin = false;
	private bool sideEnemy = false;
	internal bool deletingMenus = false;

	public void LoadSettings() {
		if (PlayerPrefs.HasKey("CameraSpeed")) {
			float newSpeed = PlayerPrefs.GetFloat("CameraSpeed");
			cameraSpeedSlider.value = newSpeed;
			cameraSpeedSliderText.text = $"Camera Speed: {newSpeed}";
			mainCamera.speed = newSpeed;
		}
		if (PlayerPrefs.HasKey("KeepLogin") && PlayerPrefs.GetInt("KeepLogin") == 1) {
			Username = PlayerPrefs.GetString("username");

			login.transform.Find("Username/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = Username;
			login.transform.Find("Password/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "********";
			Password = PlayerPrefs.GetString("password");
			login.transform.Find("Sticky").GetComponent<Toggle>().isOn = true;
		}
	}

	public void SetStickyCredentials(bool stick) {
		PlayerPrefs.SetInt("KeepLogin", Convert.ToInt16(stick));
		PlayerPrefs.Save();
	}

	public void SetCameraSpeed(float newSpeed) {
		mainCamera.speed = newSpeed;
		PlayerPrefs.SetFloat("CameraSpeed", newSpeed);
		PlayerPrefs.Save();
		cameraSpeedSliderText.GetComponent<TextFloatAppender>().UpdateText(newSpeed);
	}

	#endregion

	private void Awake() {
		admin = false;
		Debug.unityLogger.filterLogType = LogType.Log;
		LoadSettings();
	}
	private void Start() {
		server.LoadSheet();
		transform.Find("UI/Points").GetComponent<TextMeshProUGUI>().text = $"A:{server.pointsA}pts B:{server.pointsB}pts";
	}

	private void Update() {
		if (deletingMenus && Input.GetKeyUp(KeyCode.Mouse0)) {
			Transform ui = transform.GetChild(0);

			for (int i = 0; i < ui.childCount; i++) {
				Transform child = ui.GetChild(i);
				if (child.name == "ContextMenu(Clone)") {
					Destroy(child.gameObject);
				}
			}
			deletingMenus = false;
		}
	}


	public void SetPassword(string password) {
		Password = PasswordManager.HashPassword(password);
	}

	/// <summary>
	/// Logs in the user based on input fields in the settings.
	/// </summary>
	public void Login() {
		Debug.Log("u"+Username);
		Debug.Log("p"+Password);
		Debug.Log("s"+server.passwordAdmin);
		if (Username == "A" && Password == server.passwordA) {
			loggedIn = true;
			sideEnemy = false;
			admin = false;
		} else if (Username == "B" && Password == server.passwordB) {
			loggedIn = true;
			sideEnemy = true;
			admin = false;
		} else if (Username == "Admin" && Password == server.passwordAdmin) {
			loggedIn = true;
			sideEnemy = false;
			admin = true;
		}
		//Saving credentials to registry
		if (loggedIn && PlayerPrefs.GetInt("KeepLogin") == 1) {
			PlayerPrefs.SetString("username", Username);
			PlayerPrefs.SetString("password", Password);
		}
		//Do when user logs in
		if (loggedIn) {
			generalPopup.PopUp("Logged In!");
			UnitManager.Instance.SwitchSide(sideEnemy);
		} else {
			generalPopup.PopUp("Error!");
		}
	}

	public void ExitApplication() {
		Application.Quit();
		//EditorApplication.ExitPlaymode();
	}
}
