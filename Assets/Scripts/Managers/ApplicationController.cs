using System;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationController : MonoBehaviour {
	public static UIPopup generalPopup;
	public SheetSync server;
	public string Username { private get; set; }
	public string Password { private get; set; }
	#region Static methods and attributes.
	internal static bool isLoggedIn = false;
	internal static bool isDeletingMenus = false;
	internal static bool isSideB = false;
	internal static bool isAdmin = false;
	internal static bool isDebug = true;
	internal static string applicationVersion = "v0.0.7";
	internal static CultureInfo culture = new("en-GB");
	internal static ApplicationController Instance { get { return _instance; } }
	private static ApplicationController _instance;

	//Used by the UI calls. Exist the application and also play editor mode.
	public static void ExitApplication() {
		Debug.Log("Exiting application.");
		Application.Quit();
		#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
		#endif
	}
	#endregion

	private void Awake() {
		_instance = GetComponent<ApplicationController>();
		generalPopup = transform.Find("UI/GeneralPopup").GetComponent<UIPopup>();
		mainCamera = Camera.main.GetComponent<CameraController>();
		isAdmin = false;
		Debug.unityLogger.filterLogType = LogType.Log;
		LoadSettings();
	}
	private async void Start() {
		//Loads basic UI elements and starts server syncing.
		await server.LoadSheet();
		transform.Find("UI/Loading").gameObject.SetActive(false);
		
	}
	private void Update() {
		//Checks for context menus so they can be deleted when any is open.
		if (isDeletingMenus && Input.GetKeyUp(KeyCode.Mouse0)) {
			foreach (GameObject child in GameObject.FindGameObjectsWithTag("ContextMenus")) {
				if (child.name == "ContextMenu(Clone)") {
					Destroy(child);
				}
			}
			isDeletingMenus = false;
		}
	}


	#region Settings
	public Slider cameraSpeedSlider;
	public UITextFloatAppender cameraSpeedSliderText;
	private CameraController mainCamera;

	/// <summary>
	/// Loads settings from the registry if they exist. Also loads the credentials.
	/// </summary>
	public void LoadSettings() {
		if (PlayerPrefs.HasKey("CameraSpeed")) {
			float newSpeed = PlayerPrefs.GetFloat("CameraSpeed");
			cameraSpeedSlider.value = newSpeed;
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

		if (PlayerPrefs.HasKey("Fullscreen")) {
			bool fullscreen = EnumUtil.ConvertIntToBool(PlayerPrefs.GetInt("Fullscreen"));
			if (fullscreen) {
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
			} else {
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.MaximizedWindow);
			}
			transform.Find("UI/Settings/Fullscreen").GetComponent<Toggle>().SetIsOnWithoutNotify(fullscreen);
		}

		if (PlayerPrefs.HasKey("Debug")) {
			isDebug = EnumUtil.ConvertIntToBool(PlayerPrefs.GetInt("Debug"));
			transform.Find("UI/Settings/Debug").GetComponent<Toggle>().SetIsOnWithoutNotify(isDebug);
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

	public void SetFullscreen(bool fullscreen) {
		if (fullscreen) {
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
		} else {
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.MaximizedWindow);
		}
		PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
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
		cameraSpeedSliderText.UpdateText(newSpeed);
	}

	#endregion

	//Used by buttons in the UI calls. Cannot be put to the Pw setter since that would hash already hashed passwords.
	public void HashAndSetPassword(string password) => Password = PasswordManager.HashPassword(password);

	/// <summary>
	/// Logs in the user based on input fields in the settings.
	/// </summary>
	public void Login() {
		//Sidechange swaps the unit icons if the isSideB changes.
		bool sideChange = false;
		//Login logic based on three user approach
		if (Username == "A" && Password == SheetSync.passwordA) {
			if (isSideB) {
				sideChange = true;
			}
			isLoggedIn = true;
			isSideB = false;
			isAdmin = false;
			transform.Find("UI/Settings/Debug").gameObject.SetActive(false);
		} else if (Username == "B" && Password == SheetSync.passwordB) {
			if (!isSideB) {
				sideChange = true;
			}
			isLoggedIn = true;
			isSideB = true;
			isAdmin = false;
			transform.Find("UI/Settings/Debug").gameObject.SetActive(false);
		} else if (Username == "Admin" && Password == SheetSync.passwordAdmin) {
			if (isSideB) {
				sideChange = true;
			}
			isLoggedIn = true;
			isSideB = false;
			isAdmin = true;
			transform.Find("UI/Settings/Debug").gameObject.SetActive(true);
		}
		//Saving credentials to registry
		if (isLoggedIn && PlayerPrefs.GetInt("KeepLogin") == 1) {
			PlayerPrefs.SetString("username", Username);
			PlayerPrefs.SetString("password", Password);
			LoadSettings();
		}
		//Do when user logs in
		if (isLoggedIn) {
			generalPopup.PopUp("Logged In!");
			transform.Find("UI/Login").gameObject.SetActive(false);
			if (sideChange) {
				UnitManager.Instance.SwitchSide();
			}
		} else {
			generalPopup.PopUp("Wrong credentials!", 3);
		}
	}
}
