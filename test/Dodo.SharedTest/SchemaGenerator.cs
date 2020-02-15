using Common;
using Common.Extensions;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Sites;
using Dodo.Users;
using Dodo.WorkingGroups;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dodo.SharedTest
{
	public static class SchemaGenerator
	{
		public static ResourceSchemaBase GetRandomSchema<T>(AccessContext context) where T : IRESTResource
		{
			return m_mappings[typeof(T)].Invoke(context);
		}

		private static Random m_random = new Random();
		public static string SampleMarkdown => File.ReadAllText("SampleMarkdown.md");
		public static GeoLocation RandomLocation => new GeoLocation(m_random.NextDouble() * 90, m_random.NextDouble() * 90);
		private static DateTime RandomDate => DateTime.Now + TimeSpan.FromDays(m_random.NextDouble() * 365);
		private static Dictionary<Type, Func<AccessContext, ResourceSchemaBase>> m_mappings =
			new Dictionary<Type, Func<AccessContext, ResourceSchemaBase>>()
		{
			{ typeof(User), GetRandomUser },
			{ typeof(Rebellion), GetRandomRebellion },
			{ typeof(WorkingGroup), wg => GetRandomWorkinGroup(wg) },
			{ typeof(LocalGroup), lg => GetRandomLocalGroup(lg) },
			{ typeof(Site), s => GetRandomSite(s) },
			{ typeof(Role), r => GetRandomRole(r) },
		};

		public static RoleSchema GetRandomRole(AccessContext context, WorkingGroup wg = null)
		{
			wg = wg ?? ResourceUtility.GetFactory<WorkingGroup>().CreateObject(
				context,
				GetRandomWorkinGroup(context));
			return new RoleSchema(StringExtensions.RandomString(32),
				SampleMarkdown,
				wg.GUID);
		}

		public static SiteSchema GetRandomSite(AccessContext context, WorkingGroup wg = null)
		{
			wg = wg ?? ResourceUtility.GetFactory<WorkingGroup>().CreateObject(
				context,
				GetRandomWorkinGroup(context));
			return new SiteSchema(StringExtensions.RandomString(32),
				ReflectionExtensions.GetConcreteClasses<Site>().Random().FullName,
				wg.GUID,
				RandomLocation,
				SampleMarkdown);
		}

		public static UserSchema GetRandomUser(AccessContext context = default)
		{
			return new UserSchema(StringExtensions.RandomString(32),
				StringExtensions.RandomString(32).ToLowerInvariant(),
				ValidationExtensions.GenerateStrongPassword(),
				$"{StringExtensions.RandomString(16)}@{StringExtensions.RandomString(16)}.com"
			);
		}

		public static RebellionSchema GetRandomRebellion(AccessContext context)
		{
			var startDate = RandomDate;
			return new RebellionSchema(StringExtensions.RandomString(32),
				SampleMarkdown,
				RandomLocation,
				startDate,
				startDate + TimeSpan.FromDays(m_random.NextDouble() * 14)
			);
		}

		public static WorkingGroupSchema GetRandomWorkinGroup(AccessContext context, GroupResource rsc = null)
		{
			rsc = rsc ?? ResourceUtility.GetFactory<Rebellion>().CreateObject(
				context,
				GetRandomRebellion(context));
			return new WorkingGroupSchema(StringExtensions.RandomString(32),
				SampleMarkdown,
				rsc.GUID
			);
		}

		public static LocalGroupSchema GetRandomLocalGroup(AccessContext context, GroupResource rsc = null)
		{
			rsc = rsc ?? ResourceUtility.GetFactory<Rebellion>().CreateObject(
				context,
				GetRandomRebellion(context));
			return new LocalGroupSchema(StringExtensions.RandomString(32),
				SampleMarkdown,
				RandomLocation
			);
		}
	}
}
