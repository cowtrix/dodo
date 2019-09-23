using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Common;

namespace XR.Dodo
{
	public struct SpreadsheetError
	{
		public int Row;
		public int Column;
		public string Value;
		public string Message;
	}
	public static class GSheets
	{
		static string[] Scopes = { SheetsService.Scope.Spreadsheets };
		static string ApplicationName = "XR Dodo Automated Service";
		static SheetsService m_service;

		static GSheets()
		{
			UserCredential credential;

			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Logger.Debug("Credential file saved to: " + credPath);
			}

			// Create Google Sheets API service.
			m_service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});
			/*
			// Define request parameters.
			String spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
			String range = "Class Data!A2:E";
			SpreadsheetsResource.ValuesResource.GetRequest request =
					service.Spreadsheets.Values.Get(spreadsheetId, range);

			// Prints the names and majors of students in a sample spreadsheet:
			// https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
			ValueRange response = request.Execute();
			IList<IList<Object>> values = response.Values;
			if (values != null && values.Count > 0)
			{
				Logger.Debug("Name, Major");
				foreach (var row in values)
				{
					// Print columns A and E, which correspond to indices 0 and 4.
					Logger.Debug("{0}, {1}", row[0], row[4]);
				}
			}
			else
			{
				Logger.Debug("No data found.");
			}*/
		}

		public static void ClearSheet(string spreadSheetID, string range)
		{
			var requestBody = new ClearValuesRequest();
			var request = m_service.Spreadsheets.Values.Clear(requestBody, spreadSheetID, range);
			var response = request.Execute();
		}

		public static void WriteSheet<T>(string spreadsheetID, List<List<T>> data, string range)
		{
			var obj = new List<IList<object>>();
			foreach(var entry in data)
			{
				var objList = new List<object>();
				foreach(var val in entry)
				{
					objList.Add(val);
				}
				obj.Add(objList);
			}
			WriteSheetData(spreadsheetID, obj, range);
		}

		public static void WriteSheetData(string spreadsheetID, IList<IList<object>> data, string range)
		{
			m_service.Spreadsheets.Values.BatchClear(new BatchClearValuesRequest(), spreadsheetID);
			var values = new ValueRange
			{
				Values = data
			};
			var request = m_service.Spreadsheets.Values.Update(values, spreadsheetID, range);
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
			request.Execute();
		}

		public static ValueRange GetSheetRange(string spreadsheetID, string range)
		{
			var request = m_service.Spreadsheets.Values.Get(spreadsheetID, range);
			return request.Execute();
		}

		public static Spreadsheet GetSheet(string spreadsheetID)
		{
			var request = m_service.Spreadsheets.Get(spreadsheetID);
			return request.Execute();
		}
	}
}
