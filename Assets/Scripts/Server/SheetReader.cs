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

	static private string spreadsheetId;		//Sheet ID
	static private string serviceAccountID;		//Account ID
	static private string private_key;			//PK
	static private SheetsService service;		//Service

	/// <summary>
	/// Initializes the service and the credentials.
	/// </summary>
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

	/// <summary>
	/// Reads the sheetData.json file and decrypts the data.
	/// </summary>
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

	/// <summary>
	/// Method gets data from the sheet.
	/// </summary>
	/// <param name="sheetNameAndRange">string AX:BZ coordinates</param>
	/// <returns></returns>
	public async Task<IList<IList<object>>> GetSheetRangeAsync(string sheetNameAndRange) {
		GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);

		// Await the ExecuteAsync method to retrieve the sheet data
		ValueRange response = await request.ExecuteAsync();

		return response.Values;
	}

	/// <summary>
	/// Method sets data to the sheet.
	/// </summary>
	/// <param name="dataArray">Data to save to the sheet in 2D List<List<Object>> structure.</param>
	/// <param name="range">string AX:BZ coordinates</param>
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
			if (ApplicationController.isDebug) Debug.Log("Saved");
		});
	}
}

[Serializable]
public class SheetData {
	public string private_key;
	public string client_email;
	public string spreadsheet_id;
}