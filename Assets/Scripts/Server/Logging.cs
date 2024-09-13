using System.IO;
using UnityEditor;
using UnityEngine;

public class Logging : MonoBehaviour {
	public string logFileName = "output.log";
	private StreamWriter logFileWriter;

	void Start() {
		// Subscribe to the log message received event
		Application.logMessageReceived += HandleLog;

		// Open the log file for writing
		logFileWriter = File.CreateText(Path.Combine(Application.dataPath, logFileName));
	}

	private void HandleLog(string logString, string stackTrace, LogType type) {
		logFileWriter.WriteLine($"[{type}] {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}: {logString}");
		logFileWriter.Flush();
	}
}

#if UNITY_EDITOR
[InitializeOnLoad]
public class PlayModeExitListener {
	static PlayModeExitListener() {
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	private async static void OnPlayModeStateChanged(PlayModeStateChange state) {
		if (state == PlayModeStateChange.ExitingPlayMode && ApplicationController.Instance != null) {
			ApplicationController.isController = false;
			await ApplicationController.Instance.server.SaveConfiguration();
		}
	}
}
#endif
