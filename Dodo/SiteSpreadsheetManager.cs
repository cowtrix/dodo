using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XR.Dodo
{
	public class SiteSpreadsheetManager
	{
		const string StatusRange = "A1:ZZZ";
		private static string m_statusID;
		Dictionary<int, SiteSpreadsheet> Sites = new Dictionary<int, SiteSpreadsheet>();
		public SiteSpreadsheetManager(string configPath)
		{
			LoadFromGSheets(configPath);
			UpdateErrorReport();
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
				"Error Row",
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
								"", "", "", ""
							});
						}
						rowList.AddRange(new[]
						{
							error.Row, error.Message, error.Value
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
			GSheets.ClearSheet(m_statusID, StatusRange);
			GSheets.WriteSheet(m_statusID, errorReport, StatusRange);
		}

		void LoadFromGSheets(string configPath)
		{
			var configs = File.ReadAllLines(configPath);
			m_statusID = configs.First();
			foreach (var config in configs.Skip(1))
			{
				var cols = config.Split('\t');
				if (cols.Length != 3)
				{
					Console.WriteLine($"Failed to read config at line: {config}");
					continue;
				}
				if (!int.TryParse(cols[0], out var sitecode))
				{
					Console.WriteLine($"Failed to parse sitecode at line: {config}");
					continue;
				}
				Sites.Add(sitecode, new SiteSpreadsheet(sitecode, cols[1], cols[2]));
			}
			Console.WriteLine("Finished loading");
		}

		public bool IsCoordinator(User user)
		{
			return Sites.Any(x => x.Value.Coordinators.Any(y => y.PhoneNumber == user.PhoneNumber));
		}

		public bool IsCoordinator(string phoneNumber)
		{
			return Sites.Any(x => x.Value.Coordinators.Any(y => y.PhoneNumber == phoneNumber));
		}

		public bool IsCoordinator(int telegramID)
		{
			return Sites.Any(x => x.Value.Coordinators.Any(y => y.TelegramUser == telegramID));
		}
	}
}
