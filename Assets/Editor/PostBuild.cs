using System.IO;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PostBuild : IPostprocessBuildWithReport {
	public int callbackOrder => 0;

	public void OnPostprocessBuild(BuildReport report) {
		//if (report.summary.result == BuildResult.Succeeded) {
		PrintTargetOperatingSystem();
	}

	private static void MoveFiles(string path = "/Build/World War Mode Map Manager_Data") {
		string[] filesToMove = {
			"/sheetData.json",
			"/map.png",
		};

		string targetDir = Application.dataPath + "/../" + path;

		foreach (string file in filesToMove) File.Copy(Application.dataPath + file, targetDir + file, true);
		Debug.Log($"Files moved to the build folder!");
	}


	public static void PrintTargetOperatingSystem() {
		BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

		switch (target) {
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				Debug.Log("Target operating system: Windows");
				MoveFiles();
				break;
			case BuildTarget.StandaloneOSX:
				//In case of macOS, it renames the built file to the official name and puts the data into its application contents
				Debug.Log("Target operating system: macOS");
				if (File.Exists(Application.dataPath + "/../Build/World War Mode Map Manager.app")) {
					File.Delete(Application.dataPath + "/../Build/World War Mode Map Manager.app");
				}
				File.Move(Application.dataPath + "/../Build/Build.app", Application.dataPath + "/../Build/World War Mode Map Manager.app");
				
				MoveFiles("/Build/World War Mode Map Manager.app/Contents");
				break;
			case BuildTarget.StandaloneLinux64:
				Debug.Log("Target operating system: Linux");
				break;
			default:
				Debug.Log("Target operating system: Unknown");
				break;
		}
	}
}


