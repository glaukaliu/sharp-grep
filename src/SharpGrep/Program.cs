using System; 

namespace SharpGrep
{
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
			catch (ArgumentException ex)
			{
				Console.WriteLine($"Argument error: {ex.Message}");
				return ExitCodes.ArgumentError;
			}

			var matcher = new RegexMatcher(options.Pattern, options.IgnoreCase, options.WholeWord);

			bool matchesPattern = false;

			bool summaryOutput = options.CountOnly || options.ListWithMatches || options.ListWithoutMatches;

			if (options.Inputs.Length == 0)
			{
				//  ---------- STANDARD INPUT ----------
				string? line;

				Context beforeContext = new Context(options.Before);
				int remainingAfter = 0;

				while ((line = Console.In.ReadLine()) != null)
				{
					if (!options.OnlyMatches)
					{
						if (matcher.IsMatch(line))
						{
							Output.PrintBeforeContext(beforeContext, null, false);
							Console.WriteLine(line);
							matchesPattern = true;
							remainingAfter = options.After;
						}
						else
						{
							Output.HandleAfterContext(line, null, false, ref remainingAfter, beforeContext, options.Before);
						}
					}
					else
					{
						var matches = new List<(int start, int length)>();
						matcher.CollectMatches(line, matches);
						if (matches.Count > 0)
						{
							Output.PrintBeforeContext(beforeContext, null, false);
							Output.PrintOnlyMatches(line, matches, null, false);
							matchesPattern = true;
							remainingAfter = options.After;
						}
						else
						{
							Output.HandleAfterContext(line, null, false, ref remainingAfter, beforeContext, options.Before);
						}

					}
				}
			}
			else
			{
				//  ---------- FILES ----------
				bool multipleFiles = false;
				if (options.Inputs.Length > 1)
				{
					multipleFiles = true;
				}
				foreach (var filePath in options.Inputs)
				{
					try
					{
						using (StreamReader reader = new StreamReader(filePath))
						{
							string? line;

							Context beforeContext = new Context(options.Before);
							int remainingAfter = 0;

							while ((line = reader.ReadLine()) != null)
							{
								if (!options.OnlyMatches)
								{
									if (matcher.IsMatch(line))
									{
										Output.PrintBeforeContext(beforeContext, filePath, multipleFiles);
										Output.PrintLine(line, filePath, multipleFiles);
										matchesPattern = true;

										remainingAfter = options.After;
									}
									else
									{
										Output.HandleAfterContext(line, filePath, multipleFiles, ref remainingAfter, beforeContext, options.Before);
									}
								}
								else
								{
									var matches = new List<(int start, int length)>();
									matcher.CollectMatches(line, matches);
									if (matches.Count > 0)
									{
										Output.PrintBeforeContext(beforeContext, filePath, multipleFiles);
										Output.PrintOnlyMatches(line, matches, filePath, multipleFiles);
										matchesPattern = true;
										remainingAfter = options.After;
									}
									else
									{
										Output.HandleAfterContext(line, filePath, multipleFiles, ref remainingAfter, beforeContext, options.Before);
									}
								}
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Error opening file: {ex.Message}");
						return ExitCodes.FileError;
					}
				}
			}
			return matchesPattern ? ExitCodes.Success : ExitCodes.NoMatchesFound;
		}
	}
}	