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
	public GameObject password;
	public SheetSync server;
	public string Username { private get; set; }
	public string Password { private get; set; }
	internal bool loggedIn = false;
	internal bool admin = false;
	internal bool sideB = false;
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

			password.transform.Find("Username/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = Username;
			password.transform.Find("Password/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "********";
			password.transform.Find("Sticky").GetComponent<Toggle>().isOn = true;
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
		transform.Find("UI/Points").GetComponent<TextMeshProUGUI>().text = $"A:{server.pointsA}pts B:{server.pointsB}pts";
		server.LoadSheet();
		transform.Find("UI/Loading").gameObject.SetActive(false);
	}

	private void Update() {
		if (deletingMenus && Input.GetKeyUp(KeyCode.Mouse0)) {
			foreach (GameObject child in GameObject.FindGameObjectsWithTag("ContextMenus")) {
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
		bool sideChange = false;
		if (Username == "A" && Password == server.passwordA) {
			if (sideB) {
				sideChange = true;
			}
			loggedIn = true;
			sideB = false;
			admin = false;
		} else if (Username == "B" && Password == server.passwordB) {
			if (!sideB) {
				sideChange = true;
			}
			loggedIn = true;
			sideB = true;
			admin = false;
		} else if (Username == "Admin" && Password == server.passwordAdmin) {
			if (sideB) {
				sideChange = true;
			}
			loggedIn = true;
			sideB = false;
			admin = true;
		}
		//Saving credentials to registry
		if (loggedIn && PlayerPrefs.GetInt("KeepLogin") == 1) {
			PlayerPrefs.SetString("username", Username);
			PlayerPrefs.SetString("password", Password);
			LoadSettings();
		}
		//Do when user logs in
		if (loggedIn) {
			generalPopup.PopUp("Logged In!");
			login.SetActive(false);
			if (sideChange) {
				UnitManager.Instance.SwitchSide(sideB);
			}
		} else {
			generalPopup.PopUp("Error!");
		}
	}

	public void ExitApplication() {
		Application.Quit();
		//EditorApplication.ExitPlaymode();
	}
}
