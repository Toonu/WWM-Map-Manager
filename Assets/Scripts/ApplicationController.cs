using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationController : MonoBehaviour {
	#region Settings
	public Slider cameraSpeedSlider;
	public TextMeshProUGUI cameraSpeedSliderText;
	public CameraController mainCamera;
	public static TextMeshProUGUI generalPopup;
	public GameObject login;
	public SheetSync server;
	public string username { private get; set; }
	public string password { private get; set; }
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
			username = PlayerPrefs.GetString("username");
			password = PlayerPrefs.GetString("password");

			login.transform.Find("Username/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = username;
			login.transform.Find("Password/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "********";
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
		LoadSettings();
	}
	private void Start() {
		server.LoadSheet();
		transform.Find("UI/Points").GetComponent<TextMeshProUGUI>().text = $"A:{server.pointsA}pts B:{server.pointsB}pts";
		//UnitManager.Instance.PopulateUI();
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

	/// <summary>
	/// Logs in the user based on input fields in the settings.
	/// </summary>
	public void Login() {
		password = PasswordManager.HashPassword(password);
		if (username == "A" && password == server.passwordA) {
			loggedIn = true;
			sideEnemy = false;
			admin = false;
		} else if (username == "B" && password == server.passwordB) {
			loggedIn = true;
			sideEnemy = true;
			admin = false;
		} else if (username == "Admin" && password == server.passwordAdmin) {
			loggedIn = true;
			sideEnemy = false;
			admin = true;
		}
		//Saving credentials to registry
		if (loggedIn && PlayerPrefs.GetInt("KeepLogin") == 1) {
			PlayerPrefs.SetString("username", username);
			PlayerPrefs.SetString("password", password);
		}
		//Do when user logs in
		if (loggedIn) {
			transform.Find("UI/LoginPopup").gameObject.GetComponent<Popup>().PopUp();
			UnitManager.Instance.SwitchSide(sideEnemy);
		} else {
			transform.Find("UI/ErrorPopup").gameObject.GetComponent<Popup>().PopUp();
		}
	}

	public void ExitApplication() {
		Application.Quit();
		//EditorApplication.ExitPlaymode();
	}
}
