using UnityEngine;
using UnityEditor;
using TMPro;

public class SetPlayerVersion : EditorWindow {
	private static string newVersion = "0.0.0";
	private TMP_Text versionLabel;

	[MenuItem("Version/Set Version")]
	public static void ShowWindow() {
		GetWindow<SetPlayerVersion>("Set Version");
		newVersion = EditorPrefs.GetString("ProgramVersion");
	}

	private void OnGUI() {
		GUILayout.Label("Set Version", EditorStyles.boldLabel);

		newVersion = EditorGUILayout.TextField("New Version", newVersion);

		if (GUILayout.Button("Set Version")) {
			PlayerSettings.bundleVersion = newVersion;

			// Find the TMP Text label object by name
			GameObject labelObject = GameObject.Find("VersionLabel");
			if (labelObject != null) {
				if (labelObject.TryGetComponent(out versionLabel)) {
					versionLabel.text = $"World War Mode Manager v{newVersion}\nby Toonu";
				}
			}
			labelObject = GameObject.Find("World War Map Manager");
			if (labelObject != null) {
				if (labelObject.TryGetComponent(out versionLabel)) {
					versionLabel.text = $"World War Mode\r\nMap Manager\r\nv{newVersion}";
				}
			}

			EditorPrefs.SetString("ProgramVersion", newVersion);
			Debug.Log($"Set application version to {newVersion}");
		}
	}
}
