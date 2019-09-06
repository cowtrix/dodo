using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XR.Dodo
{
	public class SiteSpreadsheet
	{
		public enum EWorkingGroup
		{
			ActionSupport,
			ArresteeSupport,
			WorldBuildingProd,
			MediaAndMessaging,
			MovementSupport,
		}

		public struct Coordinator
		{
			public string Name;
			public string Number;
			public string Email;
			public string Role;
			public EWorkingGroup WorkingGroup;

			public override string ToString()
			{
				return $"{Name}: {Number}";
			}
		}

		public readonly int SiteCode;
		public readonly string SiteName;
		public List<Coordinator> Coordinators = new List<Coordinator>();

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
			Console.WriteLine("Loading spreadsheet for site " + siteName);
			SiteCode = siteCode;
			SiteName = siteName;
			var spreadSheet = GSheets.GetSheetRange(spreadSheetID, "A:ZZ");
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
							Console.WriteLine($"Unable to parse phone number for {name}: {number} @ row {rowIndex}");
							continue;
						}

						Coordinators.Add(new Coordinator()
						{
							Name = name,
							Email = email,
							Number = number,
							Role = role,
							WorkingGroup = workingGroupEnum,
						});
					}
				}
				catch(Exception e)
				{
					Console.WriteLine($"Failed to parse row {rowIndex}");
					Console.WriteLine($"{e.Message}\n{e.StackTrace}");
				}
			}
		}
	}
}
