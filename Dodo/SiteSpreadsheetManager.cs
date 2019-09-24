using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace XR.Dodo
{
	public class SiteSpreadsheetManager : IBackup
	{
		const string StatusRange = "A1:ZZZ";
		private string m_statusID;
		private string m_wgDataID;

		public class SiteData
		{
			public Dictionary<int, SiteSpreadsheet> Sites = new Dictionary<int, SiteSpreadsheet>();
			public Dictionary<string, WorkingGroup> WorkingGroups = new Dictionary<string, WorkingGroup>();
		}
		public SiteData Data;

		public SiteSpreadsheetManager(Configuration config)
		{
			m_statusID = config.SpreadsheetData.SiteSpreadsheetHealthReportID;
			m_wgDataID = config.SpreadsheetData.WorkingGroupDataID; ;

			try
			{
				LoadFromGSheets(config);
			}
			catch (Exception e)
			{
				Logger.Exception(e);
				LoadFromFile(config.BackupPath);
			}
			UpdateErrorReport();
		}

		public void GenerateErrorEmails(string outputPath)
		{
			outputPath = Path.GetFullPath(outputPath);
			foreach (var site in Data.Sites)
			{
				if (site.Value.Status.Errors.Count == 0)
				{
					continue;
				}
				var rotaCoordsWithEmails = DodoServer.SessionManager.GetUsers()
					.Where(user => user.IsRotaCoordinator
					&& user.CoordinatorRoles.Any(x => x.SiteCode == site.Key)
					&& ValidationExtensions.EmailIsValid(user.Email)).ToList();
				if (rotaCoordsWithEmails.Count() == 0)
				{
					Logger.Warning($"Errors exist on site spreadsheet {site.Value.SiteName} but there is no Rota manager with an email");
					continue;
				}

				var sb = new StringBuilder();
				for (int i = 0; i < rotaCoordsWithEmails.Count; i++)
				{
					var coord = rotaCoordsWithEmails[i];
					sb.Append(coord.Email + ", ");
				}
				sb.AppendLine();
				sb.AppendLine();

				sb.Append("Hello ");
				for (int i = 0; i < rotaCoordsWithEmails.Count; i++)
				{
					var coord = rotaCoordsWithEmails[i];
					sb.Append(coord.Name + ", ");
				}
				sb.AppendLine();
				sb.AppendLine();

				sb.AppendLine($"This is an email to inform you of some errors that have been detected on the Autumn Rebellion Site Spreadsheet for {site.Value.SiteName} (site {site.Key}). "
					+ $"You have been put on this spreadsheet as being in the Rota team, which is why you are receiving this email. This spreadsheet can be found here: {site.Value.URL}");
				sb.AppendLine();
				sb.AppendLine($"These errors usually relate to an improperly typed phone number, or putting more than one contact detail in the same cell. Please be aware that these "
					+ "spreadsheets are read by an automated system, so you should endeavor to not significantly change the format of these spreadsheets and put properly formatted "
					+ "data in the cells. If you have multiple contact details to input, you should put them in the same column for the working group, but one below the other.");
				sb.AppendLine();
				sb.AppendLine($"The errors for your site spreadsheet are:");

				foreach (var error in site.Value.Status.Errors)
				{
					sb.AppendLine($"{error.Message} at Row {error.Row + 1}, Column {error.Column + 1}. The value of this cell is '{error.Value}'");
				}
				sb.AppendLine();

				var siteCoords = DodoServer.SessionManager.GetUsers().Where(x => x.CoordinatorRoles.Any(y => y.SiteCode == site.Key));
				sb.AppendLine($"You currently have {siteCoords.Count()} coordinators registered properly at your site, out of {site.Value.WorkingGroups.Count} roles.");
				sb.AppendLine();

				sb.AppendLine($"This is an automatically generated email. " +
					"You can check the status of your spreadsheets at any time here: https://docs.google.com/spreadsheets/d/1ggsb9-ZjYcCK69f8vt0LQSWQbxFSgudGxHeA5r0-lc8");
				sb.AppendLine();

				sb.AppendLine("This spreadsheet will be updated regularly. When possible, please fix any errors shown. If you have any questions, don't hesitate to ask me.");
				sb.AppendLine();

				sb.AppendLine("Love and Rage,");
				sb.AppendLine("Sean");
				sb.AppendLine("RSO Rota Team");

				File.WriteAllText(outputPath + site.Value.SiteName + ".txt", sb.ToString());
			}
			Logger.Debug("Output error emails to: " + outputPath);
		}

		public SiteSpreadsheet GetSite(int siteCode)
		{
			return Data.Sites.SingleOrDefault(x => x.Key == siteCode).Value;
		}

		public WorkingGroup GetWorkingGroup(string code)
		{
			if(!IsValidWorkingGroup(code))
			{
				throw new Exception("Invalid working group shortcode: " + code);
			}
			return Data.WorkingGroups[code];
		}

		void UpdateErrorReport()
		{
			var errorReport = new List<List<string>>();
			errorReport.Add(new List<string>()
			{
				"Last Updated", "",
				DateTime.Now.ToString()
			});
			errorReport.Add(new List<string>()
			{
				"Site Code",
				"Site Name",
				"Spreadsheet URL",
				"Coordinators Found",
				"Error Count",
				"Row",
				"Column",
				"Error Message",
				"Row Value",
			});
			foreach (var site in Data.Sites)
			{
				var siteCoords = DodoServer.SessionManager.GetUsers().Where(x => x.CoordinatorRoles.Any(y => y.SiteCode == site.Key));
				var errorCount = site.Value.Status?.Errors.Count();
				var rowList = new List<string>(){
					site.Key.ToString(),
					site.Value.SiteName,
					site.Value.URL,
					siteCoords.Count().ToString(),
					errorCount.ToString()
				};
				if (errorCount > 0)
				{
					for (var i = 0; i < site.Value.Status.Errors.Count; i++)
					{
						var error = site.Value.Status.Errors[i];
						if (i > 0)
						{
							rowList.AddRange(new[]
							{
								"", "", "", "", "",
							});
						}
						rowList.AddRange(new[]
						{
							(error.Row + 1).ToString(), (error.Column + 1).ToString(), error.Message, error.Value
						});
						errorReport.Add(rowList.ToList());
						rowList.Clear();
					}
				}
				else
				{
					rowList.AddRange(new[]
					{
						"", "", "", ""
					});
					errorReport.Add(rowList.ToList());
					rowList.Clear();
				}
				rowList.AddRange(new[]
				{
					"",
				});
			}
			Logger.Debug("Updated Spreadsheet Health Report");
			if (!DodoServer.Dummy)
			{
				GSheets.ClearSheet(m_statusID, StatusRange);
				GSheets.WriteSheet(m_statusID, errorReport, StatusRange);
			}
		}

		public bool IsValidWorkingGroup(WorkingGroup workingGroup)
		{
			if(string.IsNullOrEmpty(workingGroup.ShortCode))
			{
				return false;
			}
			return Data.WorkingGroups.ContainsKey(workingGroup.ShortCode);
		}

		public bool IsValidWorkingGroup(string shortCode)
		{
			return Data.WorkingGroups.ContainsKey(shortCode);
		}

		public bool IsValidWorkingGroup(string shortCode, out WorkingGroup workingGroup)
		{
			return Data.WorkingGroups.TryGetValue(shortCode, out workingGroup);
		}

		public bool IsValidSiteCode(int siteCode)
		{
			return Data.Sites.ContainsKey(siteCode);
		}

		public static bool TryStringToParentGroup(string str, out EParentGroup group)
		{
			group = default;
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			str = str.ToUpperInvariant();
			if (str.Contains("ACTION SUPPORT"))
			{
				group = EParentGroup.ActionSupport;
				return true;
			}
			if (str.Contains("ARRESTEE SUPPORT"))
			{
				group = EParentGroup.ArresteeSupport;
				return true;
			}
			if (str.Contains("WORLD BUILDING AND PRODUCTION"))
			{
				group = EParentGroup.WorldBuildingProd;
				return true;
			}
			if (str.Contains("MEDIA AND MESSAGING"))
			{
				group = EParentGroup.MediaAndMessaging;
				return true;
			}
			if (str.Contains("MOVEMENT SUPPORT"))
			{
				group = EParentGroup.MovementSupport;
				return true;
			}
			if (str.Contains("RSO"))
			{
				group = EParentGroup.RSO;
				return true;
			}
			return false;
		}

		public bool GetWorkingGroupFromName(string workingGroupName, out WorkingGroup wg)
		{
			wg = Data.WorkingGroups.Values.FirstOrDefault(x => x.Name.Contains(workingGroupName) || workingGroupName.Contains(x.Name));
			return !string.IsNullOrEmpty(wg.ShortCode);
		}

		void LoadFromGSheets(Configuration config)
		{
			if(DodoServer.Dummy)
			{
				LoadFromFile(config.BackupPath);
				return;
			}

			Data = new SiteData();
			LoadWorkingGroups();
			foreach (var site in config.SpreadsheetData.SiteSpreadsheets)
			{
				var sheet = new SiteSpreadsheet(site.SiteNumber, site.SiteName, site.SheetID, this);
				if(sheet.WorkingGroups.Count == 0)
				{
					continue;
				}
				Data.Sites.Add(site.SiteNumber, sheet);
			}
			Logger.Debug("Finished loading");
		}

		private void LoadWorkingGroups()
		{
			var wgData = GSheets.GetSheetRange(m_wgDataID, "A:D");
			var parentGroup = EParentGroup.ActionSupport;
			foreach(var row in wgData.Values.Skip(1))
			{
				try
				{
					var rowStrings = row.Cast<string>();
					if (TryStringToParentGroup(rowStrings.ElementAtOrDefault(0), out var newGroup))
					{
						parentGroup = newGroup;
					}
					var wgName = rowStrings.ElementAtOrDefault(2);
					var mandate = rowStrings.ElementAtOrDefault(3);
					var wg = GenerateWorkingGroup(wgName, parentGroup, mandate);
				}
				catch(Exception e)
				{
					Logger.Exception(e, $"Failed to parse coordinator row at {wgData.Values.IndexOf(row)}");
				}
			}
			Logger.Debug($"Loaded {Data.WorkingGroups.Count} working groups.");
		}

		public WorkingGroup GenerateWorkingGroup(string name, EParentGroup parentGroup, string mandate)
		{
			if(Data.WorkingGroups.Any(x => x.Value.Name == name && x.Value.ParentGroup == parentGroup))
			{
				return Data.WorkingGroups.First(x => x.Value.Name == name && x.Value.ParentGroup == parentGroup).Value;
			}
			var shortCode = GenerateShortCode(name);
			var wg = new WorkingGroup(name, parentGroup, mandate, shortCode);
			Data.WorkingGroups.Add(wg.ShortCode, wg);
			return wg;
		}

		private string GenerateShortCode(string name)
		{
			var shortcode = name.Where(x => char.IsUpper(x)).Aggregate("", (current, next) => current + next);
			if(shortcode.Length >= 2)
			{
				shortcode = shortcode.Substring(0, 2);
				if(!Data.WorkingGroups.ContainsKey(shortcode))
				{
					return shortcode;
				}
			}
			int tries = 1;
			while (shortcode.Length < 2 || Data.WorkingGroups.ContainsKey(shortcode))
			{
				var nextChar = name.Substring(tries, 1);
				tries++;
				if(!char.IsWhiteSpace(nextChar[0]))
				{
					shortcode = (name.Substring(0, 1) + nextChar).ToString().ToUpperInvariant();
				}
			}
			return shortcode;
		}

		public List<SiteSpreadsheet> GetSites()
		{
			return Data.Sites.Values.ToList();
		}

		public void SaveToFile(string dataPath)
		{
			dataPath = Path.Combine(dataPath, "sites.json");
			Logger.Debug($"Saved site data to {dataPath}");
			File.WriteAllText(dataPath, JsonConvert.SerializeObject(Data, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
		}

		public void LoadFromFile(string backupPath)
		{
			backupPath = Path.Combine(backupPath, "sites.json");
			if (!File.Exists(backupPath))
			{
				Data = Data ?? new SiteData();
				return;
			}
			Data = JsonConvert.DeserializeObject<SiteData>(File.ReadAllText(backupPath), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Logger.Debug("Loaded site data from " + backupPath);
		}
	}
}
