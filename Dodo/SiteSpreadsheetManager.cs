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
		private static string m_statusID;
		private string backupPath = "Backups\\SiteBackup.json";
		Dictionary<int, SiteSpreadsheet> Sites = new Dictionary<int, SiteSpreadsheet>();

		public SiteSpreadsheetManager(string configPath)
		{
			try
			{
				LoadFromGSheets(configPath);
			}
			catch(Exception e)
			{
				Logger.Exception(e);
				//LoadFromBackUp(backupPath);
			}
			var backupTask = new Task(() =>
			{
				while(true)
				{
					try
					{
						SaveToBackup(backupPath);
					}
					catch(Exception e)
					{
						Logger.Exception(e);
					}
					System.Threading.Thread.Sleep(5 * 60 * 1000);
				}
			});
			backupTask.Start();
			UpdateErrorReport();
		}

		public SiteSpreadsheet GetSite(int siteCode)
		{
			return Sites.Single(x => x.Key == siteCode).Value;
		}

		public WorkingGroup GetWorkingGroup(string code)
		{
			return Sites.First(x => x.Value.WorkingGroups.Any(y => y.ShortCode == code))
				.Value.WorkingGroups.Single(x => x.ShortCode == code);
		}

		private void SaveToBackup(string path)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(Sites));
		}

		private void LoadFromBackUp(string path)
		{
			if(!File.Exists(path))
			{
				Sites = Sites ?? new Dictionary<int, SiteSpreadsheet>();
				return;
			}
			Sites = JsonConvert.DeserializeObject<Dictionary<int, SiteSpreadsheet>>(File.ReadAllText(path));
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
				"Error Count",
				"Row",
				"Column",
				"Error Message",
				"Row Value",
			});
			foreach (var site in Sites)
			{
				var errorCount = site.Value.Status.Errors.Count();
				var rowList = new List<string>(){
					site.Key.ToString(),
					site.Value.SiteName,
					site.Value.URL,
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
								"", "", "", "",
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
			return Sites.Any(x => x.Value.WorkingGroups.Contains(workingGroup));
		}

		public bool IsValidWorkingGroup(string shortCode)
		{
			return Sites.Any(x => x.Value.WorkingGroups.Any(y => y.ShortCode == shortCode));
		}

		public bool IsValidSiteCode(int siteCode)
		{
			return Sites.ContainsKey(siteCode);
		}

		void LoadFromGSheets(string configPath)
		{
			var configs = File.ReadAllLines(configPath);
			m_statusID = configs.First();
			foreach (var config in configs.Skip(1))
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
					Sites.Add(sitecode, new SiteSpreadsheet(sitecode, cols[1], cols[2]));
				}
				catch
				{
					Logger.Error($"Could not load spreadsheet {cols[2]} ({cols[1]})");
					throw;
				}
			}
			Logger.Debug("Finished loading");
		}

		public List<SiteSpreadsheet> GetSites()
		{
			return Sites.Values.ToList();
		}
	}
}
