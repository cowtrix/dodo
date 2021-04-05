using Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using System;
using System.Text.RegularExpressions;

namespace Dodo.SeleniumTests
{
	

	
	public abstract class ResourceTests<T> : SeleniumTestBase where T: IPublicResource
	{
		[TestMethod]
		public void CanViewResource()
		{
			var obj = CreateObject<T>();
			Driver.Url = $"{URL}/{typeof(T).Name.ToLowerInvariant()}/{obj.Slug}";

			var rsc = new ResourcePageHandler(Driver);
			Assert.AreEqual(obj.Name, rsc.Title.Text);
			Assert.IsFalse(string.IsNullOrEmpty(rsc.Description.Text));

			if(obj is ITimeBoundResource tb)
			{
				// Check date display is correct
				string dtFormat(DateTime dt, bool time) =>
					dt.ToString("dd MMMM yyyy") + (time ? dt.ToString(", HH:mm") : "");
				bool sameDay = tb.StartDate.Date == tb.EndDate.Date;
				var expectedDate = $"{dtFormat(tb.StartDate.ToLocalTime(), sameDay)} - ";
				if (sameDay)
				{
					expectedDate += tb.EndDate.ToLocalTime().ToString("HH:mm");
				}
				else
				{
					expectedDate += dtFormat(tb.EndDate.ToLocalTime(), sameDay);
				}
				Assert.AreEqual(expectedDate, rsc.Date.Text);

				
				if (tb.StartDate > DateTime.UtcNow)
				{
					Assert.IsNotNull(rsc.CountdownContainer);
					var countdownText = rsc.CountdownContainer.Text
						.Replace(Environment.NewLine, " ");

					var delta = tb.StartDate - DateTime.UtcNow;
					var rgxExtractor = $@"{obj.Name} begins in\s+(\d+) days?,\s+(\d+) hours?,\s+(\d+) minutes?,\s+(\d+) seconds?";
					var rgx = new Regex(rgxExtractor);
					var match = rgx.Match(countdownText);
					Assert.IsTrue(match.Success, $"Regex failed: <{rgxExtractor}> <{countdownText}>");
					var dateRead = new TimeSpan(
						int.Parse(match.Groups[1].Value),
						int.Parse(match.Groups[2].Value),
						int.Parse(match.Groups[3].Value),
						int.Parse(match.Groups[4].Value));
					Assert.IsTrue(Math.Abs((delta - dateRead).TotalMinutes) < 2);
				}
				else
				{
					Assert.IsNull(rsc.CountdownContainer);
				}
			}

			if(obj is ILocationalResource loc)
			{
				// Check address display is correct
				Assert.AreEqual(loc.Location.Address, rsc.Address.Text);
			}
		}
	}

	[TestClass]
	public class RebellionTests : ResourceTests<Rebellions.Rebellion> { }

	[TestClass]
	public class LocalGroupTests : ResourceTests<LocalGroups.LocalGroup> { }

	[TestClass]
	public class EventTests : ResourceTests<LocationResources.Event> { }

	[TestClass]
	public class SiteTests : ResourceTests<LocationResources.Site> { }
}
