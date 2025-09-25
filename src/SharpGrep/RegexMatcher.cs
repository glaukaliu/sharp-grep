using System.Text.RegularExpressions;
using System.Text;

namespace SharpGrep
{
	/// <summary>
	/// Encapsulates regex matching logic.
	/// Handles options like ignore case and whole word.
	/// Provides methods to check if a line matches, and to find all matches in a line
	/// Also provides method to check if a binary file matches the pattern.
	/// </summary>
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

		public void CollectMatches(string input, List<(int start, int length)> matches)
		{
			foreach (Match match in regex.Matches(input))
			{
				matches.Add((match.Index, match.Length));
			}
		}
		public bool IsBinaryFileMatch(string filePath)
		{
			byte[] data = File.ReadAllBytes(filePath);
			string text = Encoding.Latin1.GetString(data);
			return regex.IsMatch(text);
		}
	}
}