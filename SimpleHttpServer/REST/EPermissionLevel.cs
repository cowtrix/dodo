namespace SimpleHttpServer.REST
{
	public enum EPermissionLevel
	{
		PUBLIC = 0,	// Any requester
		USER = 1,	// A valid, signed in user
		ADMIN = 2,	// An administrator of the resource
		OWNER = 3,	// An owner of the resource
		SYSTEM = byte.MaxValue,
	}
}
