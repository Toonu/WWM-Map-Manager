using Google.Apis.Auth.OAuth2;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

public class SheetReader : MonoBehaviour {
	static private string spreadsheetId;
	static private string serviceAccountID;
	static private string private_key;
	static private SheetsService service;

	void Awake() {
		Initiate();
		ServiceAccountCredential.Initializer initializer = new ServiceAccountCredential.Initializer(serviceAccountID);
		ServiceAccountCredential credential = new ServiceAccountCredential(
			initializer.FromPrivateKey(private_key)
		);

		service = new SheetsService(
			new BaseClientService.Initializer() {
				HttpClientInitializer = credential,
			}
		);
	}

	public void Initiate() {
		try {
			string path = Application.dataPath + "/sheetData.json";
			string jsonString = File.ReadAllText(path);
			SheetData data = JsonUtility.FromJson<SheetData>(jsonString);

			spreadsheetId = data.spreadsheet_id;
			serviceAccountID = PasswordManager.Decrypt(data.client_email, "eafduhgrfsa86gr4g87aer4");
			private_key = PasswordManager.Decrypt(data.private_key, "874rg9a8rg4r4hgae4aeht8");
		} catch (Exception e) {
			GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().generalPopup.PopUp("Fatal Error! Could not connect to the server! " + e, 30);
		}
	}

	public IList<IList<object>> GetSheetRange(string sheetNameAndRange) {
		try {
			GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);

			ValueRange response = request.Execute();
			IList<IList<object>> values = response.Values;
			if (values != null && values.Count > 0) {
				return values;
			} else {
				Debug.Log("No data found.");
				return null;
			}
		} catch (Exception e) {
			GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().generalPopup.PopUp("Fatal Error! Could not connect to the server! " + e, 30);
			return null;
		}
	}

	public void SetSheetRange(IList<IList<object>> dataArray, string range) {
		service.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadsheetId, range).ExecuteAsync().ContinueWith(task => {
			// Build the update request with the new data
			ValueRange convertedData = new ValueRange {
				Values = dataArray,
				Range = range
			};

			UpdateRequest request = service.Spreadsheets.Values.Update(convertedData, spreadsheetId, range);

			request.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
			request.Execute();
			Debug.Log("Saved");
		});
	}
	/*
	public IEnumerator<YieldInstruction> ClearAndSetValues(string spreadsheetId, string range, IList<IList<object>> values) {
		
		// Build the update request with the new data
		ValueRange convertedData = new ValueRange {
			Values = values,
			Range = range
		};

		ClearRequest clear = service.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadsheetId, range.Substring(0, range.Length - 1));
		UpdateRequest update = service.Spreadsheets.Values.Update(convertedData, spreadsheetId, range);
		update.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;

		List<Request> requests = new List<Request>() { clear, update };

		// Add the requests to a BatchUpdateValuesRequest object.
		BatchRequest batchRequest = new BatchRequest(service);


		// To execute asynchronously in an async method, replace `request.Execute()` as shown:
		Data.BatchUpdateSpreadsheetResponse response = request.Execute();

		batchRequest.ExecuteAsync();
	}*/
}

[System.Serializable]
public class SheetData {
	public string private_key;
	public string client_email;
	public string spreadsheet_id;
}