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
				string? line;
				while ((line = Console.In.ReadLine()) != null)
				{
					if (matcher.IsMatch(line))
					{
						Console.WriteLine(line);
						matchesPattern = true;
					}
				}
			}
			else
			{
				bool multipleFiles = false;
				if (options.Inputs.Length > 1){
					multipleFiles = true;
				}
				foreach (var filePath in options.Inputs)
				{
					try
					{
						using (StreamReader reader = new StreamReader(filePath))
						{
							string? line;
							while ((line = reader.ReadLine()) != null)
							{
								if (matcher.IsMatch(line))
								{
									if (multipleFiles)
									{
										Console.WriteLine($"{filePath}:{line}");
									}
									else
									{
										Console.WriteLine(line);
									}
									matchesPattern = true;
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