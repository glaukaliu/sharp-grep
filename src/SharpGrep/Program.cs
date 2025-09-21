using System; 

namespace SharpGrep
{
	/// <summary>
	/// The main program class.
	/// Parses command-line arguments, sets up the regex matcher, runs either the summary or stream engine.
	/// Exits with appropriate exit code.
	/// </summary>
	public class Program
	{
		public static int Main(string[] args)
		{
			Options options;
			var parser = new ArgParser();
			try
			{
				options = parser.ParseArgs(args);
			}
			catch (HelpRequestedException)
			{
				Console.WriteLine(ArgParser.HelpText);
				return ExitCodes.ArgumentError;
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine($"Argument error: {ex.Message}");
				return ExitCodes.ArgumentError;
			}

			var matcher = new RegexMatcher(options.Pattern, options.IgnoreCase, options.WholeWord);

			// if -c, -l, or -L is specified, use SummaryPrint
			// otherwise, use NormalPrint
			bool summaryOutput = options.CountOnly || options.ListWithMatches || options.ListWithoutMatches;

			if (summaryOutput)
			{
				var engine = new SummaryPrint();
				return engine.Run(options, matcher);
			}
			else
			{
				var engine = new NormalPrint();
				return engine.Run(options, matcher);
			}

		}
	}
}	