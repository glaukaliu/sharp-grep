using System.Text.RegularExpressions;

namespace SharpGrep
{
	public class RegexMatcher
	{
		private Regex regex;

		public RegexMatcher(string pattern, bool ignoreCase, bool wholeWord)
		{
			var options = RegexOptions.Compiled;
			if (ignoreCase)
			{
				options |= RegexOptions.IgnoreCase;
			}
			if (wholeWord)
			{
				pattern = "\\b(?:" + pattern + ")\\b";
			}
			else
			{
				pattern = Regex.Escape(pattern);
			}
			regex = new Regex(pattern, options);
		}
		public bool IsMatch(string input)
		{
			return regex.IsMatch(input);
		}

	}
}