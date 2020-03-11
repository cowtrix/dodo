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
			return GetRandomSchema(typeof(T), context);
		}

		public static ResourceSchemaBase GetRandomSchema(Type type, AccessContext context)
		{
			if (m_mappings.TryGetValue(type, out var schemaFunc))
			{
				return schemaFunc.Invoke(context);
			}
			if (type.BaseType != null)
			{
				return GetRandomSchema(type.BaseType, context);
			}
			throw new Exception($"No schema generator found for type {type}");
		}

		private static Random m_random = new Random();
		public static string SampleMarkdown => File.ReadAllText(@"resources\SampleMarkdown.md");
		public static GeoLocation RandomLocation => new GeoLocation(m_random.NextDouble() * 90, m_random.NextDouble() * 90);
		private static DateTime RandomDate => DateTime.Now + TimeSpan.FromDays(m_random.NextDouble() * 365);
		private static string RandomName => StringExtensions.RandomString(32);

		private static Dictionary<Type, Func<AccessContext, ResourceSchemaBase>> m_mappings =
			new Dictionary<Type, Func<AccessContext, ResourceSchemaBase>>()
		{
			{ typeof(User), GetRandomUser },
			{ typeof(Rebellion), GetRandomRebellion },
			{ typeof(WorkingGroup), wg => GetRandomWorkinGroup(wg) },
			{ typeof(LocalGroup), lg => GetRandomLocalGroup(lg) },
			{ typeof(Role), r => GetRandomRole(r) },
			{ typeof(EventSite), s => GetRandomSite<EventSite>(s) },
			{ typeof(MarchSite), s => GetRandomSite<MarchSite>(s) },
			{ typeof(PermanentSite), s => GetRandomSite<PermanentSite>(s) },

		};

		public static RoleSchema GetRandomRole(AccessContext context, WorkingGroup wg = null)
		{
			wg = wg ?? ResourceUtility.GetFactory<WorkingGroup>().CreateTypedObject(
				context,
				GetRandomWorkinGroup(context));
			return new RoleSchema(RandomName,
				SampleMarkdown,
				wg.GUID);
		}

		public static SiteSchema GetRandomSite<T>(AccessContext context, Rebellion rb = null) where T:Site
		{
			rb = rb ?? ResourceUtility.GetFactory<Rebellion>().CreateTypedObject(
				context,
				GetRandomRebellion(context));
			return new SiteSchema(RandomName, typeof(T).FullName, rb.GUID, RandomLocation, SampleMarkdown);
		}

		public static UserSchema GetRandomUser(AccessContext context = default)
		{
			return new UserSchema(RandomName,
				RandomName.ToLowerInvariant(),
				ValidationExtensions.GenerateStrongPassword(),
				$"{StringExtensions.RandomString(16)}@{StringExtensions.RandomString(16)}.com"
			);
		}

		public static RebellionSchema GetRandomRebellion(AccessContext context)
		{
			var startDate = RandomDate;
			return new RebellionSchema(RandomName,
				SampleMarkdown,
				RandomLocation,
				startDate,
				startDate + TimeSpan.FromDays(m_random.NextDouble() * 14)
			);
		}

		public static WorkingGroupSchema GetRandomWorkinGroup(AccessContext context, GroupResource rsc = null)
		{
			rsc = rsc ?? ResourceUtility.GetFactory<Rebellion>().CreateTypedObject(
				context,
				GetRandomRebellion(context));
			return new WorkingGroupSchema(RandomName,
				SampleMarkdown,
				rsc.GUID
			);
		}

		public static LocalGroupSchema GetRandomLocalGroup(AccessContext context, GroupResource rsc = null)
		{
			rsc = rsc ?? ResourceUtility.GetFactory<Rebellion>().CreateTypedObject(
				context,
				GetRandomRebellion(context));
			return new LocalGroupSchema(RandomName,
				SampleMarkdown,
				RandomLocation
			);
		}
	}
}
