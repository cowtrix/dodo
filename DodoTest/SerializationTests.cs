using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dodo.Dodo;
using DodoTest;
using Newtonsoft.Json;

[TestClass]
public class SerializationTests : TestBase
{
	[TestMethod]
	public void ValidateNumbers()
	{
		string invalid = "test";
		Assert.IsFalse(ValidationExtensions.ValidateNumber(ref invalid));

		string valid = "+44 132 510 3992";
		Assert.IsTrue(ValidationExtensions.ValidateNumber(ref valid));
	}

	[TestMethod]
	public void CheckSiteSerialization()
	{
		var data = new SiteSpreadsheetManager.SiteData();
		data.Sites.TryAdd(3, new SiteSpreadsheet(3, "Test", ""));
		data.WorkingGroups.TryAdd("TE", new WorkingGroup("Test", EParentGroup.ActionSupport, "test", "TE"));

		var str = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto
		});

		var des = JsonConvert.DeserializeObject<SiteSpreadsheetManager.SiteData>(str, new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto
		});
		Assert.IsTrue(des.Sites.ContainsKey(3));
		Assert.IsTrue(des.WorkingGroups.ContainsKey("TE"));
		Assert.IsTrue(des.WorkingGroups["TE"].ShortCode == "TE");
	}
}
