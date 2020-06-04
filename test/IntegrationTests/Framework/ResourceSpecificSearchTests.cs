using Dodo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;
using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using System.Collections.Generic;
using Dodo.Resources;
using Dodo.SharedTest;
using Dodo.Rebellions;
using DodoTest.Framework.Postman;
using Dodo.WorkingGroups;
using Dodo.Sites;
using Dodo.LocalGroups;

namespace RESTTests.Search
{
	[TestClass]
	public class RebellionResourceSpecificSearchTests : ResourceSpecificSearchTests<Rebellion>
	{
	}

	[TestClass]
	public class WorkingGroupResourceSpecificSearchTests : ResourceSpecificSearchTests<WorkingGroup>
	{
	}

	[TestClass]
	public class EventSiteResourceSpecificSearchTests : ResourceSpecificSearchTests<EventSite>
	{
		public override string ResourceRoot => "site";
		public override string PostmanCategory => $"{typeof(Site).Name}s";
	}

	[TestClass]
	public class PermanentSiteResourceSpecificSearchTests : ResourceSpecificSearchTests<PermanentSite>
	{
		public override string ResourceRoot => "site";
		public override string PostmanCategory => $"{typeof(Site).Name}s";
	}

	[TestClass]
	public class MarchSiteResourceSpecificSearchTests : ResourceSpecificSearchTests<MarchSite>
	{
		public override string ResourceRoot => "site";
		public override string PostmanCategory => $"{typeof(Site).Name}s";
	}

	[TestClass]
	public class LocalGroupResourceSpecificSearchTests : ResourceSpecificSearchTests<LocalGroup>
	{
	}

	public abstract class ResourceSpecificSearchTests<T> : IntegrationTestBase where T : IRESTResource
	{
		public virtual string ResourceRoot => typeof(T).Name;
		public virtual string PostmanCategory => $"{typeof(T).Name}s";
		protected virtual ResourceSchemaBase GetSchema(AccessContext context) => SchemaGenerator.GetRandomSchema<T>(context);

		[TestMethod]
		public async virtual Task CanSearchWithLocationFilter()
		{
			GetRandomUser(out _, out var context);
			var factory = ResourceUtility.GetFactory<T>();
			var resources = new List<T>();
			for (var i = 0; i < 5; ++i)
			{
				resources.Add(CreateObject<T>(context, SchemaGenerator.GetRandomSchema<T>(context)));
			}
			var resource = resources.Random() as ILocationalResource;
			if (resource == null)
			{
				Assert.Inconclusive();
			}
			var list = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.GET,
				parameters: new[]
				{
					(nameof(DistanceFilter.LatLong), $"{resource.Location.Latitude}+{resource.Location.Longitude}"),
					(nameof(DistanceFilter.Distance), "20.6"),
				});
			var guids = list.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			Assert.IsTrue(guids.Contains(resource.Guid.ToString()));
		}

		[TestMethod]
		public async virtual Task CanSearchWithDateFilter()
		{
			if(!typeof(ITimeBoundResource).IsAssignableFrom(typeof(T)))
			{
				Assert.Inconclusive();
			}
			GetRandomUser(out _, out var context);
			var resources = new List<T>();
			for (var i = 0; i < 1; ++i)
			{
				resources.Add(CreateObject<T>(context, SchemaGenerator.GetRandomSchema<T>(context)));
			}
			var resource = resources.Random() as ITimeBoundResource;
			var list = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.GET,
				parameters: new[]
				{
					(nameof(DateFilter.StartDate), $"{resource.StartDate}"),
					(nameof(DateFilter.EndDate), $"{resource.EndDate}")
				});
			var guids = list.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			Assert.IsTrue(guids.Contains(resource.Guid.ToString()));
		}

		[TestMethod]
		public async virtual Task CanSearchWithParentFilter()
		{
			if (!typeof(IOwnedResource).IsAssignableFrom(typeof(T)))
			{
				Assert.Inconclusive();
			}
			GetRandomUser(out _, out var context);
			var resources = new List<T>();
			for (var i = 0; i < 1; ++i)
			{
				resources.Add(CreateObject<T>(context, SchemaGenerator.GetRandomSchema<T>(context)));
			}
			var resource = resources.Random() as IOwnedResource;
			var list = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.GET,
				parameters: new[]
				{
					(nameof(ParentFilter.Parent), resource.Parent.Guid.ToString() ),
				});
			var guids = list.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			Assert.IsTrue(guids.Contains(resource.Guid.ToString()));
		}

		[TestMethod]
		public async virtual Task CanSearchWithStringFilter()
		{
			GetRandomUser(out _, out var context);
			var resources = new List<T>();
			for (var i = 0; i < 1; ++i)
			{
				resources.Add(CreateObject<T>(context, SchemaGenerator.GetRandomSchema<T>(context)));
			}
			var resource = resources.Random();
			var list = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.GET,
				parameters: new[]
				{
					(nameof(StringFilter.Search), resource.Name ),
				});
			var guids = list.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			Assert.IsTrue(guids.Contains(resource.Guid.ToString()));
		}

		[TestMethod]
		public async virtual Task CanSearch()
		{
			GetRandomUser(out _, out var context);
			var sites = new List<T>();
			for (var i = 0; i < 5; ++i)
			{
				sites.Add(CreateObject<T>(context, GetSchema(context)));
			}
			var list = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.GET);
			var guids = list.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			Assert.IsFalse(sites.Any(x => !guids.Contains(x.Guid.ToString())));
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"List all {PostmanCategory}" },
				LastRequest);
		}
	}
}
