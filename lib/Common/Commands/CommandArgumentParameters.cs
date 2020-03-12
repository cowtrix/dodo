using Common.Config;

namespace Common.Commands
{
	/// <summary>
	/// Defines how we parse commands
	/// </summary>
	public class CommandArgumentParameters
	{
		public static CommandArgumentParameters Default => new ConfigVariable<CommandArgumentParameters>("DefaultCommandArgumentParameters", new CommandArgumentParameters()).Value;
		public readonly char CommandParamSeperator;
		public readonly char CommandSeperator;
		public readonly char CommandPrefix;
		public readonly char QuotationChar;
		public ValidateArgumentDelegate OnValidate;

		public CommandArgumentParameters(ICommandArgumentValidator validator, char prefix = '/', char paramSeperator = ':', char commandSeperator = ' ', char quotationChar = '"')
			: this(validator.Validate, prefix, paramSeperator, commandSeperator, quotationChar)
		{
		}

		public CommandArgumentParameters(ValidateArgumentDelegate validator = null, char prefix = '/', char paramSeperator = ':', char commandSeperator = ' ', char quotationChar = '"')
		{
			OnValidate = validator;
			CommandPrefix = prefix;
			CommandParamSeperator = paramSeperator;
			CommandSeperator = commandSeperator;
			QuotationChar = quotationChar;
		}
	}
}
