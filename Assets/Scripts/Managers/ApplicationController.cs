using System;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationController : MonoBehaviour {
	internal static UIPopup generalPopup;
	internal SheetSync server;
	public string Username { private get; set; }
	public string Password { private get; set; }
	public static bool isController = false;
	#region Static methods and attributes.
	internal static bool isLoggedIn = false;
	internal static bool isDeletingMenus = false;
	internal static bool isSideB = false;
	internal static bool isAdmin = false;
	internal static bool isDebug = true;
	private static bool isStarted = false;
	internal static string applicationVersion = "v0.0.9";
	internal static CultureInfo culture = new("en-GB");
	internal static ApplicationController Instance { get { return _instance; } }
	private static ApplicationController _instance;
	#endregion

	/// <summary>
	/// Used by the UI calls. Exits the application and also exits in the editor mode.
	/// </summary>
	public async static Task<bool> ExitApplication() {
		//Handle controller assignment if no controler is already active.
		if (isController && isLoggedIn && SheetSync.passwordA != null) {
			isController = false;
			//Giving time for the server to save the configuration.
			await Instance.server.SaveConfiguration();
		}
		Debug.Log("Exiting application.");
		Application.Quit();
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#endif

	return true;
	}
#if UNITY_EDITOR
	private void OnPlayModeStateChanged(PlayModeStateChange state) {
		if (state == PlayModeStateChange.ExitingPlayMode) {
			_ = ExitApplication();
		}
	}
#endif
	/// <summary>
	/// Method sets up Components on startup and loads settings.
	/// </summary>
	private void Awake() {
		_instance = GetComponent<ApplicationController>();
		generalPopup = transform.Find("UI/PopupWarningGeneral").GetComponent<UIPopup>();
		server = transform.GetChild(0).gameObject.GetComponent<SheetSync>();
		mainCamera = Camera.main.GetComponent<CameraController>();
		isAdmin = false;
		isController = false;
		Debug.unityLogger.filterLogType = LogType.Log;
		LoadSettings();
		Application.wantsToQuit += () => ExitApplication().Result;
#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
	}
	/// <summary>
	/// Starts server syncing.
	/// </summary>
	private async void Start() {
		await server.LoadSheetAsync();
		transform.Find("UI/Loading").gameObject.SetActive(false);
	}
	/// <summary>
	/// Checks for context menus so they can be deleted when any is open.
	/// </summary>
	private void Update() {
		if (isDeletingMenus && (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse2))) {
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
	public UILabelTextAppender cameraSpeedSliderText;
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

		if (PlayerPrefs.HasKey("Debug")) {
			isDebug = EnumUtil.ConvertIntToBool(PlayerPrefs.GetInt("Debug"));
			transform.Find("UI/Settings/Debug").GetComponent<Toggle>().SetIsOnWithoutNotify(isDebug);
		}

		if (PlayerPrefs.HasKey("Fullscreen")) {
			bool fullscreen = EnumUtil.ConvertIntToBool(PlayerPrefs.GetInt("Fullscreen"));
			SetFullscreen(fullscreen);
			transform.Find("UI/Settings/Fullscreen").GetComponent<Toggle>().SetIsOnWithoutNotify(fullscreen);
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
	/// Method sets the fullscreen mode and saves the setting.
	/// </summary>
	/// <param name="fullscreen"></param>
	public void SetFullscreen(bool fullscreen) {
		if (fullscreen) {
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
		} else {
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.Windowed);
		}
		PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
		PlayerPrefs.Save();
	}

	/// <summary>
	/// Method sets debug mode and saves the setting.
	/// </summary>
	/// <param name="debug"></param>
	public void SetDebug(bool debug) {
		isDebug = debug;
		Debug.Log("Debug set to " + debug + ".");
		PlayerPrefs.SetInt("Debug", debug ? 1 : 0);
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

	/// <summary>
	/// Used by buttons in the UI calls. Cannot be put to the Pw setter since that would hash already hashed passwords.
	/// </summary>
	/// <param name="password"></param>
	public void HashAndSetPassword(string password) => Password = PasswordManager.HashPassword(password);

	/// <summary>
	/// Logs in the user based on input fields in the settings.
	/// </summary>
	public async void Login() {
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
			transform.Find("UI/Login").gameObject.SetActive(false);
			if (!isStarted) {
				server.StartUpdateLoop();
				isStarted = true;
			}
			await server.CheckController();
			if (isController) {
				generalPopup.PopUp("Logged in! You are your teams controller!", 3);
			} else {
				generalPopup.PopUp("Logged In!");
			}
			if (sideChange) {
				UnitManager.Instance.SwitchSide();
			}

		} else {
			generalPopup.PopUp("Wrong credentials!", 3);
		}
	}
}
