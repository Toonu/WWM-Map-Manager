using System;
using System.Globalization;
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
	public string Input { private get; set; }
	public string InputP { private get; set; }
	internal bool loggedIn = false;
	internal bool admin = false;
	#endregion

	#region Unit management
	public UnitTypesManager unitManager;
	private Unit[] units;
	#endregion

	private void Awake() {
		LoadSettings();
		units = unitManager.GetComponentsInChildren<Unit>();

	}
	private void Start() {
		server.LoadSheet();
		unitManager.PopulateUI();
	}

	

	public void Login() {
		InputP = PasswordManager.HashPassword(InputP);
		switch (Input) {
			case "A":
			if (InputP == server.passwordAdmin) {
				loggedIn= true;
			}
			break;
			case "B":
			if (InputP == server.passwordB) {
				loggedIn = true;
			}
			break;
			case "Admin":
			if (InputP == server.passwordAdmin) {
				loggedIn = true;
				admin = true;
			}
			break;
			default:
			break;
		}
		Debug.Log(loggedIn);
		if (loggedIn && PlayerPrefs.GetInt("KeepLogin") == 1) {
			PlayerPrefs.SetString("username", Input);
			PlayerPrefs.SetString("password", InputP);
		}
		if (loggedIn) {
			login.transform.Find("UserToolbar").gameObject.SetActive(true);
		}
	}

	public void LoadSettings() {
		if (PlayerPrefs.HasKey("CameraSpeed")) {
			float newSpeed = PlayerPrefs.GetFloat("CameraSpeed");
			cameraSpeedSlider.value = newSpeed;
			cameraSpeedSliderText.text = $"Camera Speed: {newSpeed}";
			mainCamera.speed = newSpeed;
		}
		if (PlayerPrefs.HasKey("KeepLogin") && PlayerPrefs.GetInt("KeepLogin") == 1) {
			Input  = PlayerPrefs.GetString("username");
			InputP = PlayerPrefs.GetString("password");

			string hiddenText = "";
			for (int i = 0; i < InputP.Length; i++) {
				hiddenText += '*';
			}
			

			login.transform.Find("Username/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = Input;
			login.transform.Find("Password/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = hiddenText;
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
	}

	public void ShowMissileRanges() {
		foreach (Unit unit in units) {
			if (unit.unitType == UnitType.SAM) {
				//unit.switchRange();
			}
		}
	}

	public void ExitApplication() {
		Application.Quit();
		//EditorApplication.ExitPlaymode();
	}
}
