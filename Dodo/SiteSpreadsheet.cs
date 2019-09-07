using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XR.Dodo
{
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
		public enum EWorkingGroup
		{
			ActionSupport,
			ArresteeSupport,
			WorldBuildingProd,
			MediaAndMessaging,
			MovementSupport,
		}

		public class Coordinator : User
		{
			public string Email;
			public string Role;
			public EWorkingGroup WorkingGroup;

			public override string ToString()
			{
				return $"{Name}: {PhoneNumber}";
			}
		}

		public readonly int SiteCode;
		public readonly string SiteName;
		public List<Coordinator> Coordinators = new List<Coordinator>();
		private readonly string m_spreadSheetID;

		public string URL
		{
			get
			{
				return @"https://docs.google.com/spreadsheets/d/" + m_spreadSheetID;
			}
		}

		private static EWorkingGroup StringToWorkingGroup(string str)
		{
			str = str.ToUpperInvariant();
			if (str.Contains("ACTION SUPPORT"))
				return EWorkingGroup.ActionSupport;
			if (str.Contains("ARRESTEE SUPPORT"))
				return EWorkingGroup.ArresteeSupport;
			if (str.Contains("WORLD BUILDING AND PRODUCTION"))
				return EWorkingGroup.WorldBuildingProd;
			if (str.Contains("MEDIA AND MESSAGING"))
				return EWorkingGroup.MediaAndMessaging;
			if (str.Contains("MOVEMENT SUPPORT"))
				return EWorkingGroup.MovementSupport;
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
					string workingGroup = null;
					var workingGroupEnum = EWorkingGroup.ActionSupport;
					for (var column = 1; column < row.Count; ++column)
					{
						workingGroup = spreadSheet.Values.ElementAt(2).ElementAtOrDefault(column) as string ?? workingGroup;
						try
						{
							workingGroupEnum = StringToWorkingGroup(workingGroup);
						}
						catch { }
						var name = rowString.ElementAt(column) as string;
						if (string.IsNullOrEmpty(name))
						{
							continue;
						}
						var email = spreadSheet.Values.Skip(rowIndex).First(x => (x.First() as string).Trim() == "Email").ElementAtOrDefault(column) as string ?? "";
						var number = spreadSheet.Values.Skip(rowIndex).First(x => (x.First() as string).Trim() == "Number").ElementAtOrDefault(column) as string ?? "";
						var role = spreadSheet.Values.ElementAt(4).ElementAtOrDefault(column) as string ?? "";
						
						if(!PhoneExtensions.ValidateNumber(ref number))
						{
							throw new SpreadsheetException(rowIndex, column, $"Value wasn't a valid UK Mobile number: " + number);
						}

						Coordinators.Add(new Coordinator()
						{
							Name = name,
							Email = email,
							PhoneNumber = number,
							Role = role,
							WorkingGroup = workingGroupEnum,
						});
					}
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
						}); ;
					}
					catch { }
					Console.WriteLine($"{e.Message}\n{e.StackTrace}");
				}
			}
		}
	}
}
