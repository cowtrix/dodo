namespace REST
{
	public static class PermissionLevel
	{
		public const string PUBLIC = "PUBLIC";
		public static EPermissionLevel Public => EPermissionLevel.PUBLIC;
		public const string USER = "USER";
		public static EPermissionLevel Admin => EPermissionLevel.ADMIN;
		public const string ADMIN = "ADMIN";
		public static EPermissionLevel Owner => EPermissionLevel.OWNER;
		public const string OWNER = "OWNER";
	}

	public enum EPermissionLevel
	{
		PUBLIC = 0,	// Any requester
		USER = 1,	// A valid, signed in user
		ADMIN = 2,	// An administrator of the resource
		OWNER = 3,	// An owner of the resource
		SYSTEM = byte.MaxValue,
	}
}
