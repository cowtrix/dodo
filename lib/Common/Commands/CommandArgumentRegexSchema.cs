using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Commands
{
	/// <summary>
	/// Validates input parameters with a regex pattern
	/// </summary>
	public class CommandArgumentRegexSchema : ICommandArgumentValidator
	{
		private Dictionary<string, ValueTuple<string, string>> m_regexMapping = new Dictionary<string, (string, string)>();
		private bool m_allowUnspecifiedArgs;

		public CommandArgumentRegexSchema(bool allowUnspecifiedArgs, params ValueTuple<string, string, string>[] values)
		{
			m_allowUnspecifiedArgs = allowUnspecifiedArgs;
			foreach (var kvp in values)
			{
				m_regexMapping[kvp.Item1] = (kvp.Item2, kvp.Item3);
			}
		}

		public bool Validate(CommandArgumentParameters param, string key, string rawValue, out string error)
		{
			if (!m_regexMapping.TryGetValue(key, out var regex))
			{
				error = $"No argument found with key {key}";
				return m_allowUnspecifiedArgs;
			}
			if (!Regex.IsMatch(rawValue, regex.Item1))
			{
				error = $"Regex mismatch: {param.CommandPrefix}{key}{param.CommandParamSeperator}{rawValue} did not match required regex. {regex.Item2}";
				return false;
			}
			error = null;
			return true;
		}

		public string GetHelpString(CommandArgumentParameters argParams)
		{
			var sb = new StringBuilder("");
			foreach (var mapping in m_regexMapping)
			{
				sb.AppendLine($"{argParams.CommandPrefix}{mapping.Key}{argParams.CommandParamSeperator}value\t{mapping.Value.Item2}");
			}
			return sb.ToString();
		}
	}
}
