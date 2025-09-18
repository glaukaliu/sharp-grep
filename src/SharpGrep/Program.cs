using System; 

namespace SharpGrep
{
	class Program
	{
		static int Main(string[] args)
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
							foreach (string beforeContextLine in beforeContext.GetLines())
							{
								Console.WriteLine(beforeContextLine);
							}
							beforeContext.Clear();

							Console.WriteLine(line);
							matchesPattern = true;
							remainingAfter = options.After;
						}
						else
						{
							if (remainingAfter > 0)
							{
								Console.WriteLine(line);
								remainingAfter--;
							}
							else if (options.Before > 0)
							{
								beforeContext.AddLine(line);
							}
						}
					}
					else
					{
						var matches = new List<(int start, int length)>();
						matcher.CollectMatches(line, matches);
						if (matches.Count > 0)
						{
							foreach (string beforeContextLine in beforeContext.GetLines())
							{
								Console.WriteLine(beforeContextLine);
							}
							beforeContext.Clear();

							foreach (var match in matches)
							{
								Console.WriteLine(line.Substring(match.start, match.length));
							}
							matchesPattern = true;
							remainingAfter = options.After;
						}
						else
						{
							if (remainingAfter > 0)
							{
								Console.WriteLine(line);
								remainingAfter--;
							}
							else if (options.Before > 0)
							{
								beforeContext.AddLine(line);
							}
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
										foreach (string beforeContextLine in beforeContext.GetLines())
										{
											if (multipleFiles)
											{
												Console.WriteLine($"{filePath}:{beforeContextLine}");
											}
											else
											{
												Console.WriteLine(beforeContextLine);
											}
										}
										beforeContext.Clear();
										if (multipleFiles)
										{
											Console.WriteLine($"{filePath}:{line}");
										}
										else
										{
											Console.WriteLine(line);
										}
										matchesPattern = true;

										remainingAfter = options.After;
									}
									else
									{
										if (remainingAfter > 0)
										{
											if (multipleFiles)
											{
												Console.WriteLine($"{filePath}:{line}");
											}
											else
											{
												Console.WriteLine(line);
											}
											remainingAfter--;
										}
										else if (options.Before > 0)
										{
											beforeContext.AddLine(line);
										}
									}
								}
								else
								{
									var matches = new List<(int start, int length)>();
									matcher.CollectMatches(line, matches);
									if (matches.Count > 0)
									{
										foreach (string beforeContextLine in beforeContext.GetLines())
										{
											if (multipleFiles)
											{
												Console.WriteLine($"{filePath}:{beforeContextLine}");
											}
											else
											{
												Console.WriteLine(beforeContextLine);
											}
										}
										beforeContext.Clear();
										foreach (var match in matches)
										{
											if (multipleFiles)
											{
												Console.WriteLine($"{filePath}:{line.Substring(match.start, match.length)}");
											}
											else
											{
												Console.WriteLine(line.Substring(match.start, match.length));
											}
											matchesPattern = true;
											remainingAfter = options.After;
										}
									}
									else
									{
										if (remainingAfter > 0)
										{
											if (multipleFiles)
											{
												Console.WriteLine($"{filePath}:{line}");
											}
											else
											{
												Console.WriteLine(line);
											}
											remainingAfter--;
										}
										else if (options.Before > 0)
										{
											beforeContext.AddLine(line);
										}
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
			return matchesPattern? ExitCodes.Success : ExitCodes.NoMatchesFound;
		}
	}
}	