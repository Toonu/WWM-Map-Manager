using UnityEngine;
using UnityEditor;
using TMPro;

public class SetPlayerVersion : EditorWindow {
	private string newVersion = "0.0.0";
	private TMP_Text versionLabel;

	[MenuItem("Version/Set Version")]
	public static void ShowWindow() {
		GetWindow<SetPlayerVersion>("Set Version");
	}

	private void OnGUI() {
		GUILayout.Label("Set Version", EditorStyles.boldLabel);

		newVersion = EditorGUILayout.TextField("New Version", newVersion);

		if (GUILayout.Button("Set Version")) {
			PlayerSettings.bundleVersion = newVersion;

			// Find the TMP Text label object by name
			GameObject labelObject = GameObject.Find("VersionLabel");
			if (labelObject != null) {
				versionLabel = labelObject.GetComponent<TMP_Text>();
				if (versionLabel != null) {
					versionLabel.text = $"World War Mode Manager v{newVersion}\nby Toonu ";
				}
			}
		}
	}
}
