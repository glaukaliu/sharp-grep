using System; 

namespace SharpGrep
{
	class Program
	{
		static int Main(string[] args)
		{
			var parser = new ArgParser();
			Options options = parser.ParseArgs(args);

			bool matchesPattern = false;
			TextReader reader;
			try
			{
				reader = new StreamReader(options.Path);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error opening file: {ex.Message}");
				return ExitCodes.FileError;
			}

			string line;
			while ((line = reader.ReadLine()) != null)
			{
				if (line.IndexOf(options.Pattern, StringComparison.Ordinal) >= 0)
                {
					matchesPattern = true;
					Console.WriteLine(line);
				}
			}
			return matchesPattern? ExitCodes.Success : ExitCodes.NoMatchesFound;
		}
	}
}	