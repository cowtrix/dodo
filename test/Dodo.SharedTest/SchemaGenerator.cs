using Common;
using Common.Extensions;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Sites;
using Dodo.Users;
using Dodo.WorkingGroups;
using REST;
using System;
using System.Collections.Generic;
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
		private const string SAMPLE_MARKDOWN = "This is a block of text formatted in markdown.";
		private static GeoLocation RandomLocation => new GeoLocation(m_random.NextDouble() * 90, m_random.NextDouble() * 90);
		private static DateTime RandomDate => DateTime.Now + TimeSpan.FromDays(m_random.NextDouble() * 365);
		private static Dictionary<Type, Func<AccessContext, ResourceSchemaBase>> m_mappings =
			new Dictionary<Type, Func<AccessContext, ResourceSchemaBase>>()
		{
			{ typeof(User), GetRandomUser },
			{ typeof(Rebellion), GetRandomRebellion },
			{ typeof(WorkingGroup), GetRandomWorkinGroup },
			{ typeof(Site), GetRandomSite },
			{ typeof(Role), GetRandomRole },
		};

		private static RoleSchema GetRandomRole(AccessContext context)
		{
			var wg = ResourceUtility.GetFactory<WorkingGroup>().CreateObject(
				GetRandomWorkinGroup(context));
			return new RoleSchema(context,
				StringExtensions.RandomString(32),
				SAMPLE_MARKDOWN,
				wg);
		}

		private static SiteSchema GetRandomSite(AccessContext context)
		{
			var wg = ResourceUtility.GetFactory<WorkingGroup>().CreateObject(
				GetRandomWorkinGroup(context));
			return new SiteSchema(context,
				StringExtensions.RandomString(32),
				ReflectionExtensions.GetChildClasses<Site>().Random().FullName,
				wg,
				RandomLocation,
				SAMPLE_MARKDOWN);
		}

		private static UserSchema GetRandomUser(AccessContext context)
		{
			return new UserSchema(
				context,
				StringExtensions.RandomString(32),
				StringExtensions.RandomString(32).ToLowerInvariant(),
				ValidationExtensions.GenerateStrongPassword(),
				$"{StringExtensions.RandomString(16)}@{StringExtensions.RandomString(16)}.com"
			);
		}

		private static RebellionSchema GetRandomRebellion(AccessContext context)
		{
			var startDate = RandomDate;
			return new RebellionSchema(
				context,
				StringExtensions.RandomString(32),
				RandomLocation,
				SAMPLE_MARKDOWN,
				null,
				startDate,
				startDate + TimeSpan.FromDays(m_random.NextDouble() * 14)
			);
		}

		private static WorkingGroupSchema GetRandomWorkinGroup(AccessContext context)
		{
			var rebellion = ResourceUtility.GetFactory<Rebellion>().CreateObject(
				GetRandomRebellion(context));
			return new WorkingGroupSchema(
				context,
				StringExtensions.RandomString(32),
				SAMPLE_MARKDOWN,
				rebellion
			);
		}
	}
}
