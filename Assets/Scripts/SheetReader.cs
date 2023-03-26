using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;

namespace Assets.Scripts {
	public class SheetReader : MonoBehaviour {
		private static string spreadsheetId;
		private static string serviceAccountID;
		private static string private_key;
		private static SheetsService service;

		private void Awake() {
			Initiate();
			var initializer =
				new ServiceAccountCredential.Initializer(serviceAccountID);
			var credential = new ServiceAccountCredential(
				initializer.FromPrivateKey(private_key)
			);

			service = new SheetsService(
				new BaseClientService.Initializer
				{
					HttpClientInitializer = credential
				}
			);
		}

		public IList<IList<object>> GetSheetRange(string sheetNameAndRange) {
			var request =
				service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);

			var response = request.Execute();
			var values = response.Values;
			if (values != null && values.Count > 0) {
				return values;
			}

			Debug.Log("No data found.");
			return null;
		}

		public void SetSheetRange(IList<IList<object>> dataArray, string range) {
			var convertedData = new ValueRange();
			convertedData.Values = dataArray;
			convertedData.Range = range;

			var request =
				service.Spreadsheets.Values.Update(convertedData, spreadsheetId, range);

			request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
			var response = request.Execute();
		}

		public void Initiate() {
			var path = Application.dataPath + "/sheetData.json";
			var jsonString = File.ReadAllText(path);
			var data = JsonUtility.FromJson<SheetData>(jsonString);

			serviceAccountID = data.client_email;
			spreadsheetId = data.spreadsheet_id;
			private_key = data.private_key;
		}
	}

	[Serializable]
	public class SheetData {
		public string client_email;
		public string private_key;
		public string spreadsheet_id;
	}
}