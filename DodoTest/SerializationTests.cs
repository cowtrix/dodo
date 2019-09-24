using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XR.Dodo;
using DodoTest;
using Newtonsoft.Json;

[TestClass]
public class SerializationTests : TestBase
{
	[TestMethod]
	public void CheckSiteSerialization()
	{
		var data = new SiteSpreadsheetManager.SiteData();
		data.Sites.TryAdd(3, new SiteSpreadsheet(3, "Test", "", null));
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
