using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

public class SheetReader : MonoBehaviour {
	static private string spreadsheetId;
	static private string serviceAccountID;
	static private string private_key;
	static private SheetsService service;

	void Awake() {
		Initiate();
		ServiceAccountCredential.Initializer initializer = new(serviceAccountID);
		ServiceAccountCredential credential = new(
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
			ApplicationController.generalPopup.PopUp("Fatal Error! Could not connect to the server! " + e, 30);
		}
	}

	public async Task<IList<IList<object>>> GetSheetRangeAsync(string sheetNameAndRange) {
		GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);

		// Await the ExecuteAsync method to retrieve the sheet data
		ValueRange response = await request.ExecuteAsync();

		return response.Values;
	}

	public void SetSheetRange(IList<IList<object>> dataArray, string range) {
		service.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadsheetId, range).ExecuteAsync().ContinueWith(task => {
			// Build the update request with the new data
			ValueRange convertedData = new() {
				Values = dataArray,
				Range = range
			};

			UpdateRequest request = service.Spreadsheets.Values.Update(convertedData, spreadsheetId, range);

			request.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
			request.Execute();
			Debug.Log("Saved");
		});
	}
}

[System.Serializable]
public class SheetData {
	public string private_key;
	public string client_email;
	public string spreadsheet_id;
}