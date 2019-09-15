using Google;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XR.Dodo
{
	public enum EParentGroup
	{
		ActionSupport,
		ArresteeSupport,
		WorldBuildingProd,
		MediaAndMessaging,
		MovementSupport,
	}

	public readonly struct WorkingGroup
	{
		private static Dictionary<string, string> m_shortCodes = new Dictionary<string, string>();
		public readonly string Name;
		public readonly string Mandate;
		public readonly EParentGroup ParentGroup;
		public readonly int SiteCode;
		[JsonIgnore]
		public string ShortCode { get { return m_shortCodes[Name]; } }

		public WorkingGroup(string workingGroup, EParentGroup parentGroup, string mandate, int sitecode)
		{
			Name = workingGroup.Trim();
			ParentGroup = parentGroup;
			Mandate = mandate;
			SiteCode = sitecode;

			if(!m_shortCodes.ContainsKey(Name))
			{
				var code = "";
				int tries = 0;
				do
				{
					code = Name.Substring(tries, 2).ToUpperInvariant();
					tries++;
				} while (m_shortCodes.ContainsValue(code));
				m_shortCodes[Name] = code;
			}
		}
	}

	public class SpreadsheetException : Exception
	{
		public readonly int Row;
		public readonly int Column;
		public readonly string Value;
		public SpreadsheetException(int row, int column, string message, string value) : base(message)
		{
			Row = row;
			Column = column;
			Value = value;
		}
	}

	public class SiteSpreadsheet
	{
		public class SpreadsheetStatus
		{
			public List<SpreadsheetError> Errors = new List<SpreadsheetError>();
		}
		public SpreadsheetStatus Status { get; private set; }
		public readonly int SiteCode;
		public readonly string SiteName;
		public List<WorkingGroup> WorkingGroups = new List<WorkingGroup>();
		private readonly string m_spreadSheetID;

		public string URL
		{
			get
			{
				return @"https://docs.google.com/spreadsheets/d/" + m_spreadSheetID;
			}
		}

		private static EParentGroup StringToParentGroup(string str)
		{
			str = str.ToUpperInvariant();
			if (str.Contains("ACTION SUPPORT"))
				return EParentGroup.ActionSupport;
			if (str.Contains("ARRESTEE SUPPORT"))
				return EParentGroup.ArresteeSupport;
			if (str.Contains("WORLD BUILDING AND PRODUCTION"))
				return EParentGroup.WorldBuildingProd;
			if (str.Contains("MEDIA AND MESSAGING"))
				return EParentGroup.MediaAndMessaging;
			if (str.Contains("MOVEMENT SUPPORT"))
				return EParentGroup.MovementSupport;
			throw new Exception($"Couldn't cast {str}");
		}

		public SiteSpreadsheet(int siteCode, string siteName, string spreadSheetID)
		{
			m_spreadSheetID = spreadSheetID;
			Status = new SpreadsheetStatus();
			Logger.Debug("Loading spreadsheet for site " + siteName);
			SiteCode = siteCode;
			SiteName = siteName;
			ValueRange spreadSheet = null;
			try
			{
				spreadSheet = GSheets.GetSheetRange(m_spreadSheetID, "A:ZZZ");
			}
			catch(GoogleApiException e)
			{
				Logger.Exception(e, $"Failed to load site spreadsheet for {SiteName}");
				Status.Errors.Add(new SpreadsheetError()
				{
					Message = "ERROR: Could not load spreadsheet",
					Value = e.Message,
				});
				return;
			}
			for(var rowIndex = 0; rowIndex < spreadSheet.Values.Count; ++rowIndex)
			{
				var row = spreadSheet.Values[rowIndex];
				var rowString = row.Cast<string>();
				var firstCell = rowString.FirstOrDefault();
				if (firstCell == null || !(firstCell.Contains("Point Person") || firstCell.Contains("Role Holder")))
				{
					continue;
				}
				string parentGroupStr = null;
				var parentGroup = EParentGroup.ActionSupport;
				
				for (var column = 1; column < row.Count; ++column)
				{
					try
					{
						parentGroupStr = spreadSheet.Values.ElementAt(2).ElementAtOrDefault(column) as string ?? parentGroupStr;
						try
						{
							parentGroup = StringToParentGroup(parentGroupStr);
						}
						catch { }
						var name = rowString.ElementAt(column) as string;
						if (string.IsNullOrEmpty(name))
						{
							continue;
						}
						var nextEmailRowIndex = spreadSheet.Values.Skip(rowIndex).First(x => (x.First() as string).Trim() == "Email");
						var email = nextEmailRowIndex.ElementAtOrDefault(column) as string ?? "";
						email = email.Trim();
						var nextNumberRowIndex = spreadSheet.Values.Skip(rowIndex).First(x => (x.First() as string).Trim() == "Number");
						var number = nextNumberRowIndex.ElementAtOrDefault(column) as string ?? "";
						var workingGroupName = spreadSheet.Values
							.First(x =>
							{
								var str = (x.FirstOrDefault() as string ?? "").ToLowerInvariant();
								return str.Contains("working group");
							})
							.ElementAtOrDefault(column) as string ?? "";
						var mandate = spreadSheet.Values
							.First(x =>
							{
								var str = (x.FirstOrDefault() as string ?? "").ToLowerInvariant();
								return str.Contains("mandate");
							})
							.ElementAtOrDefault(column) as string ?? "";

						if (!ValidationExtensions.ValidateNumber(ref number))
						{
							var phoneIndex = spreadSheet.Values.IndexOf(nextNumberRowIndex);
							throw new SpreadsheetException(phoneIndex, column, 
								"Value wasn't a valid UK Mobile number", number);
						}
						if(!string.IsNullOrEmpty(email) && !ValidationExtensions.EmailIsValid(email))
						{
							var emailIndex = spreadSheet.Values.IndexOf(nextEmailRowIndex);
							Status.Errors.Add(new SpreadsheetError()
							{
								Row = rowIndex,
								Column = emailIndex,
								Message = $"Value wasn't a valid email address",
								Value = email,
							});
							email = null;
						}
						var workingGroup = new WorkingGroup(workingGroupName, parentGroup, mandate, siteCode);
						if(!WorkingGroups.Contains(workingGroup))
						{
							WorkingGroups.Add(workingGroup);
						}
						var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(number);
						user.Name = name;
						user.PhoneNumber = number;
						user.Email = email;
						user.CoordinatorRoles.Add(workingGroup);
;					}
					catch (Exception e)
					{
						try
						{
							int r = rowIndex;
							int col = column;
							var val = spreadSheet.Values[rowIndex].Cast<string>().Aggregate("", (current, next) =>
									current + (!string.IsNullOrEmpty(current) ? "," : "") + next);
							if (e is SpreadsheetException)
							{
								var se = e as SpreadsheetException;
								r = se.Row;
								col = se.Column;
								val = se.Value;
							}
							Status.Errors.Add(new SpreadsheetError()
							{
								Row = r,
								Column = col,
								Message = e.Message,
								Value = val,
							});
						}
						catch { }
						Logger.Debug($"{e.Message}\n{e.StackTrace}");
					}
			}
				
			}
		}
	}
}
