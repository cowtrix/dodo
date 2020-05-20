namespace Resources
{
	public static class PermissionLevel
	{
		public static string PUBLIC => nameof(EPermissionLevel.PUBLIC).ToLowerInvariant();
		public static EPermissionLevel Public => EPermissionLevel.PUBLIC;
		public static string USER => nameof(EPermissionLevel.USER).ToLowerInvariant();
		public static EPermissionLevel Member => EPermissionLevel.MEMBER;
		public static string MEMBER => nameof(EPermissionLevel.MEMBER).ToLowerInvariant();
		public static EPermissionLevel Admin => EPermissionLevel.ADMIN;
		public static string ADMIN => nameof(EPermissionLevel.ADMIN).ToLowerInvariant();
		public static EPermissionLevel Owner => EPermissionLevel.OWNER;
		public static string OWNER => nameof(EPermissionLevel.OWNER).ToLowerInvariant();
	}

	public enum EPermissionLevel
	{
		PUBLIC = 0,	// Any requester
		USER = 1,	// A valid, signed in user
		MEMBER = 2, // A user who is a member of the resource
		ADMIN = 3,	// An administrator of the resource
		OWNER = 4,	// An owner of the resource


		SYSTEM = byte.MaxValue,	// Cannot patch
	}
}
