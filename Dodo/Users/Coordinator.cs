namespace XR.Dodo
{
	public class Coordinator : User
	{
		public string Email;
		public string Role;
		public WorkingGroup WorkingGroup;

		public override string ToString()
		{
			return $"{Name}: {PhoneNumber}";
		}
	}
}
