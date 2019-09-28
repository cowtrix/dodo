﻿using Google;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace XR.Dodo
{
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
		[JsonIgnore]
		public SpreadsheetStatus Status { get; private set; }
		public int SiteCode;
		public string SiteName;
		public string SpreadSheetID;
		public HashSet<string> WorkingGroups = new HashSet<string>();

		public string URL
		{
			get
			{
				return @"https://docs.google.com/spreadsheets/d/" + SpreadSheetID;
			}
		}


		public SiteSpreadsheet()
		{
		}

		public SiteSpreadsheet(int siteCode, string siteName, string spreadSheetID)
		{
			SpreadSheetID = spreadSheetID;
			SiteCode = siteCode;
			SiteName = siteName;
			Status = new SpreadsheetStatus();
			Logger.Debug("Loading spreadsheet for site " + siteName);
		}

		public void LoadFromGSheets(SiteSpreadsheetManager manager)
		{
			try
			{
				ValueRange spreadSheet = null;
				spreadSheet = GSheets.GetSheetRange(SpreadSheetID, "A:ZZZ");
				var parentGroupRow = spreadSheet.Values.First(x => x.Any() && (x.First() as string).ToUpperInvariant().Contains("PARENT GROUP"));
				var workingGroupRow = spreadSheet.Values.First(x => x.Any() && (x.First() as string).ToUpperInvariant().Contains("WORKING GROUP"));
				for (var rowIndex = 0; rowIndex < spreadSheet.Values.Count; ++rowIndex)
				{
					var row = spreadSheet.Values[rowIndex];
					var rowString = row.Cast<string>();
					var firstCell = rowString.FirstOrDefault();
					if (firstCell == null || !(firstCell.Contains("Point Person") || firstCell.Contains("Role Holder")))
					{
						continue;
					}
					var parentGroup = EParentGroup.ActionSupport;
					for (var column = 1; column < row.Count; ++column)
					{
						try
						{
							var parentGroupString = parentGroupRow.ElementAtOrDefault(column) as string ?? "";
							if(SiteSpreadsheetManager.TryStringToParentGroup(parentGroupString, out var newGroup))
							{
								parentGroup = newGroup;
							}

							var name = (rowString.ElementAt(column) as string).Trim();
							if (string.IsNullOrEmpty(name))
							{
								continue;
							}
							var nextEmailRowIndex = spreadSheet.Values.Skip(rowIndex).First(x => (x.First() as string).Trim() == "Email");
							var email = nextEmailRowIndex.ElementAtOrDefault(column) as string ?? "";
							email = email.Trim();
							var nextNumberRowIndex = spreadSheet.Values.Skip(rowIndex).First(x => (x.First() as string).Trim() == "Number");
							var number = nextNumberRowIndex.ElementAtOrDefault(column) as string ?? "";
							var workingGroupName = workingGroupRow.ElementAtOrDefault(column) as string ?? "";
							var roleName = spreadSheet.Values
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
							if (string.IsNullOrEmpty(number))
							{
								continue;
							}
							if (!ValidationExtensions.ValidateNumber(ref number))
							{
								var phoneIndex = spreadSheet.Values.IndexOf(nextNumberRowIndex);
								throw new SpreadsheetException(phoneIndex, column,
									"Value wasn't a valid phone number", number);
							}
							if (!string.IsNullOrEmpty(email) && !ValidationExtensions.EmailIsValid(email))
							{
								var emailIndex = spreadSheet.Values.IndexOf(nextEmailRowIndex);
								Status.Errors.Add(new SpreadsheetError()
								{
									Row = emailIndex,
									Column = column,
									Message = $"Value wasn't a valid email address",
									Value = email,
								});
								email = null;
							}

							if (!manager.GetWorkingGroupFromName(workingGroupName, out WorkingGroup wg))
							{
								Logger.Debug($"Site {SiteCode} had custom Working Group {workingGroupName}");
								wg = manager.GenerateWorkingGroup(workingGroupName, parentGroup, mandate);
								manager.Data.WorkingGroups.TryAdd(wg.ShortCode, wg);
							}
							var role = new Role(wg, roleName, SiteCode);
							var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(number);
							var session = DodoServer.SessionManager.GetOrCreateSession(user);
							if (session.Inbox.Count > 0 && session.Workflow.CurrentTask is IntroductionTask)
							{
								DodoServer.DefaultGateway.SendMessage(new ServerMessage($"You're now recognised as a {user.Name}, a Coordinator for {role.WorkingGroup}"), session);
								session.Workflow.CurrentTask.ExitTask(session);
							}

							user.Name = name;
							user.Email = email;
							user.GDPR = true;
							user.CoordinatorRoles.Add(role);
							WorkingGroups.Add(wg.ShortCode);
						}
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
							Logger.Exception(e, nolog:true);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Exception(e, $"Failed to load site spreadsheet for {SiteName}");
				Status.Errors.Add(new SpreadsheetError()
				{
					Message = "CRITICAL ERROR: Could not load spreadsheet",
					Value = e.Message,
				});
				return;
			}
		}
	}
}
