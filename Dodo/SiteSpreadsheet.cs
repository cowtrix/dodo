﻿using Google.Apis.Sheets.v4;
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
		private static Dictionary<WorkingGroup, string> m_shortCodes = new Dictionary<WorkingGroup, string>();
		public readonly string Name;
		public readonly string Mandate;
		public readonly EParentGroup ParentGroup;
		public readonly int SiteCode;
		[JsonIgnore]
		public string ShortCode { get { return m_shortCodes[this]; } }

		public WorkingGroup(string workingGroup, EParentGroup parentGroup, string mandate, int sitecode)
		{
			Name = workingGroup;
			ParentGroup = parentGroup;
			Mandate = mandate;
			SiteCode = sitecode;

			if(!m_shortCodes.ContainsKey(this))
			{
				var code = "";
				int tries = 0;
				do
				{
					code = Utility.RandomString(3, Name + tries.ToString()).ToUpperInvariant();
					tries++;
				} while (m_shortCodes.ContainsValue(code));
				m_shortCodes[this] = code;
			}
		}
	}

	public class SpreadsheetException : Exception
	{
		public readonly int Row;
		public readonly int Column;
		public SpreadsheetException(int row, int column, string message) : base($"Col: {column} | {message}")
		{
			Row = row;
			Column = column;
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
			Console.WriteLine("Loading spreadsheet for site " + siteName);
			SiteCode = siteCode;
			SiteName = siteName;
			var spreadSheet = GSheets.GetSheetRange(m_spreadSheetID, "A:ZZZ");
			for(var rowIndex = 0; rowIndex < spreadSheet.Values.Count; ++rowIndex)
			{
				try
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
						var email = spreadSheet.Values.Skip(rowIndex).First(x => (x.First() as string).Trim() == "Email").ElementAtOrDefault(column) as string ?? "";
						email = email.Trim();
						var number = spreadSheet.Values.Skip(rowIndex).First(x => (x.First() as string).Trim() == "Number").ElementAtOrDefault(column) as string ?? "";
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
							throw new SpreadsheetException(rowIndex, column, $"Value wasn't a valid UK Mobile number: " + number);
						}
						if(!string.IsNullOrEmpty(email) && !ValidationExtensions.EmailIsValid(email))
						{
							Status.Errors.Add(new SpreadsheetError()
							{
								Row = $"{rowIndex}",
								Message = $"Value wasn't a valid email address: {email}",
								Value = spreadSheet.Values[rowIndex].Cast<string>().Aggregate("", (current, next) =>
									current + (!string.IsNullOrEmpty(current) ? "," : "") + next)
							});
							email = null;
						}
						var workingGroup = new WorkingGroup(workingGroupName, parentGroup, mandate, siteCode);
						if(!WorkingGroups.Contains(workingGroup))
						{
							WorkingGroups.Add(workingGroup);
						}
						// ADD BACK IN
						continue;
						var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(number);
						user.Name = name;
						user.PhoneNumber = number;
						user.Email = email;
						user.CoordinatorRoles.Add(workingGroup);
;					}
				}
				catch(Exception e)
				{
					try
					{
						Status.Errors.Add(new SpreadsheetError()
						{
							Row = $"{rowIndex}",
							Message = e.Message,
							Value = spreadSheet.Values[rowIndex].Cast<string>().Aggregate("", (current, next) =>
								current + (!string.IsNullOrEmpty(current) ? "," : "") + next)
						});
					}
					catch { }
					Console.WriteLine($"{e.Message}\n{e.StackTrace}");
				}
			}
		}
	}
}
