using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class SiteSpreadsheetManager
	{
		const string StatusRange = "A1:ZZZ";
		private string m_statusID;
		private string m_wgData;
		private string m_backupPath;

		public class SiteData
		{
			public Dictionary<int, SiteSpreadsheet> Sites = new Dictionary<int, SiteSpreadsheet>();
			public Dictionary<string, WorkingGroup> WorkingGroups = new Dictionary<string, WorkingGroup>();
		}
		public SiteData Data = new SiteData();

		public SiteSpreadsheetManager(string configPath)
		{
			m_backupPath = Path.Combine("Backups", "sites.json");
			try
			{
				LoadFromGSheets(configPath);
			}
			catch(Exception e)
			{
				Logger.Exception(e);
				LoadFromBackUp();
			}
			UpdateErrorReport();
			var saveTask = new Task(() =>
			{
				while(true)
				{
					System.Threading.Thread.Sleep(60 * 1000);
					try
					{
						Logger.Debug($"Saved site data to {m_backupPath}");
						File.WriteAllText(m_backupPath, JsonConvert.SerializeObject(Data.Sites));
					}
					catch (Exception e)
					{
						Logger.Exception(e);
					}
				}
			});
			saveTask.Start();
		}

		public SiteSpreadsheet GetSite(int siteCode)
		{
			return Data.Sites.Single(x => x.Key == siteCode).Value;
		}

		public WorkingGroup GetWorkingGroup(string code)
		{
			if(!IsValidWorkingGroup(code))
			{
				throw new Exception("Invalid working group shortcode: " + code);
			}
			return Data.WorkingGroups[code];
		}

		private void LoadFromBackUp()
		{
			if(!File.Exists(m_backupPath))
			{
				Data = Data ?? new SiteData();
				return;
			}
			Data = JsonConvert.DeserializeObject<SiteData>(File.ReadAllText(m_backupPath));
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
				/*var missingCoords = WorkingGroups.Where(x => !siteCoords.Any(y => y.CoordinatorRoles.Any(z => z.WorkingGroup.ShortCode == x.Value.ShortCode)));
				foreach(var missing in missingCoords)
				{
					site.Value.Status.Errors.Add(new SpreadsheetError()
					{
						Column = 0,
						Row = 0,
						Message = $"No coordinators for {missing.Value.Name} found",
					});
				}*/

				var errorCount = site.Value.Status.Errors.Count();
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
			return Data.WorkingGroups.ContainsValue(workingGroup);
		}

		public bool IsValidWorkingGroup(string shortCode)
		{
			return Data.WorkingGroups.ContainsKey(shortCode);
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

		void LoadFromGSheets(string configPath)
		{
			var configs = File.ReadAllLines(configPath);
			m_statusID = configs.First();
			m_wgData = configs.ElementAt(1);
			LoadWorkingGroups();
			if(DodoServer.Dummy)
			{
				LoadFromBackUp();
				return;
			}
			foreach (var config in configs.Skip(2))
			{
				var cols = config.Split('\t');
				try
				{
					if (cols.Length != 3)
					{
						Logger.Error($"Failed to read config at line: {config}");
						continue;
					}
					if (!int.TryParse(cols[0], out var sitecode))
					{
						Logger.Error($"Failed to parse sitecode at line: {config}");
						continue;
					}
					Data.Sites.Add(sitecode, new SiteSpreadsheet(sitecode, cols[1], cols[2], this));
				}
				catch
				{
					Logger.Error($"Could not load spreadsheet {cols[2]} ({cols[1]})");
					throw;
				}
			}
			Logger.Debug("Finished loading");
		}

		private void LoadWorkingGroups()
		{
			var wgData = GSheets.GetSheetRange(m_wgData, "A:D");
			var parentGroup = EParentGroup.ActionSupport;
			foreach(var row in wgData.Values.Skip(3))
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
					Data.WorkingGroups.Add(wg.ShortCode, wg);
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
			var shortCode = GenerateShortCode(name);
			return new WorkingGroup(name, parentGroup, mandate, shortCode);
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
			int tries = 0;
			while (shortcode.Length < 2 || Data.WorkingGroups.ContainsKey(shortcode))
			{
				shortcode = name.Substring(tries, 2).ToUpperInvariant();
				tries++;
			}
			return shortcode;
		}

		public List<SiteSpreadsheet> GetSites()
		{
			return Data.Sites.Values.ToList();
		}
	}
}
