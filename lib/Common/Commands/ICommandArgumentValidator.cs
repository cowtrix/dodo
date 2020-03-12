namespace Common.Commands
{
	/// <summary>
	/// Represents a class that can validate a commandline argument
	/// </summary>
	public interface ICommandArgumentValidator
	{
		bool Validate(CommandArgumentParameters param, string key, string rawValue, out string error);
	}
}
