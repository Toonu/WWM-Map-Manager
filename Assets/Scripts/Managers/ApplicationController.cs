using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationController : MonoBehaviour {
	public static UIPopup generalPopup;
	public SheetSync server;
	public string Username { private get; set; }
	public string Password { private get; set; }
	internal bool loggedIn = false;
	internal static bool deletingMenus = false;
	internal static bool sideB = false;
	internal static bool admin = false;
	internal static string applicationVersion = "v0.0.7";

	private void Awake() {
		generalPopup = transform.Find("UI/GeneralPopup").GetComponent<UIPopup>();
		mainCamera = Camera.main.GetComponent<CameraController>();
		admin = false;
		Debug.unityLogger.filterLogType = LogType.Log;
		//Loads registry items for the settings.
		LoadSettings();
	}
	private async void Start() {
		//Loads basic UI elements and starts server syncing.
		await server.LoadSheet();
		transform.Find("UI/Loading").gameObject.SetActive(false);
	}
	private void Update() {
		//Checks for context menus so they can be deleted when any is open.
		if (deletingMenus && Input.GetKeyUp(KeyCode.Mouse0)) {
			foreach (GameObject child in GameObject.FindGameObjectsWithTag("ContextMenus")) {
				if (child.name == "ContextMenu(Clone)") {
					Destroy(child);
				}
			}
			deletingMenus = false;
		}
	}


	#region Settings
	public Slider cameraSpeedSlider;
	public TextMeshProUGUI cameraSpeedSliderText;
	private CameraController mainCamera;

	/// <summary>
	/// Loads settings from the registry if they exist. Also loads the credentials.
	/// </summary>
	public void LoadSettings() {
		if (PlayerPrefs.HasKey("CameraSpeed")) {
			float newSpeed = PlayerPrefs.GetFloat("CameraSpeed");
			cameraSpeedSlider.value = newSpeed;
			cameraSpeedSliderText.text = $"Camera Speed: {newSpeed}";
			mainCamera.speed = newSpeed;
		}
		if (PlayerPrefs.HasKey("KeepLogin") && PlayerPrefs.GetInt("KeepLogin") == 1) {
			Username = PlayerPrefs.GetString("username");
			Password = PlayerPrefs.GetString("password");

			transform.Find("UI/Login/Items/Username/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = Username;
			transform.Find("UI/Login/Items/Password/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "********";
			transform.Find("UI/Login/Items/Sticky").GetComponent<Toggle>().isOn = true;

			transform.Find("UI/BottomPanel/Username/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = Username;
			transform.Find("UI/BottomPanel/Password/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "********";
			transform.Find("UI/BottomPanel/Sticky").GetComponent<Toggle>().isOn = true;
		}
	}

	/// <summary>
	/// Saves the setting into registry.
	/// </summary>
	/// <param name="stick">If sticky</param>
	public void SetStickyCredentials(bool stick) {
		PlayerPrefs.SetInt("KeepLogin", Convert.ToInt16(stick));
		PlayerPrefs.Save();
	}

	/// <summary>
	/// Set the camera zooming and moving speed.
	/// </summary>
	/// <param name="newSpeed">New camera speed float</param>
	public void SetCameraSpeed(float newSpeed) {
		mainCamera.speed = newSpeed;
		PlayerPrefs.SetFloat("CameraSpeed", newSpeed);
		PlayerPrefs.Save();
		cameraSpeedSliderText.GetComponent<UITextFloatAppender>().UpdateText(newSpeed);
	}

	#endregion

	//Used by buttons in the UI calls. Cannot be put to the Pw setter since that would hash already hashed passwords.
	public void HashAndSetPassword(string password) => Password = PasswordManager.HashPassword(password);

	/// <summary>
	/// Logs in the user based on input fields in the settings.
	/// </summary>
	public void Login() {
		//Sidechange swaps the unit icons if the side changes.
		bool sideChange = false;
		//Login logic based on three user approach
		if (Username == "A" && Password == SheetSync.passwordA) {
			if (sideB) {
				sideChange = true;
			}
			loggedIn = true;
			sideB = false;
			admin = false;
		} else if (Username == "B" && Password == SheetSync.passwordB) {
			if (!sideB) {
				sideChange = true;
			}
			loggedIn = true;
			sideB = true;
			admin = false;
		} else if (Username == "Admin" && Password == SheetSync.passwordAdmin) {
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
			transform.Find("UI/Login").gameObject.SetActive(false);
			if (sideChange) {
				UnitManager.Instance.SwitchSide();
			}
		} else {
			generalPopup.PopUp("Wrong credentials!", 3);
		}
	}

	//Used by the UI calls. Exist the application and also play editor mode.
	public static void ExitApplication() {
		Debug.Log("Exiting application.");
		Application.Quit();
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#endif
	}
}
