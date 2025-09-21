namespace SharpGrep
{
	public class Output
	{
		//Prints a line, in case of multiple files, specifies the file.
		public static void PrintLine(string line, string? filePath, bool multipleFiles)
		{
			if (multipleFiles)
				Console.WriteLine(filePath + ":" + line);
			else
				Console.WriteLine(line);
		}

		// Prints all lines stored in beforeContext, and clears it.
		public static void PrintBeforeContext(Context beforeContext, string? filePath, bool multipleFiles)
		{
			foreach (string beforeContextLine in beforeContext.GetLines())
			{
				PrintLine(beforeContextLine, filePath, multipleFiles);
			}
			beforeContext.Clear();
		}
		// Prints only the matched parts of a line.
		public static void PrintOnlyMatches(string line, List<(int start, int length)> matches, string? filePath, bool multipleFiles)
		{
			foreach (var match in matches)
			{
				var matchToPrint = line.Substring(match.start, match.length);
				PrintLine(matchToPrint, filePath, multipleFiles);
			}
		}

		// Handles after-context logic.
		// If it has remaining after lines, prints and decrements remainingAFter, else adds to beforeContext, if larger than 0.
		public static void HandleAfterContext(string line, string? filePath, bool multipleFiles, ref int remainingAfter, Context beforeContext, int beforeCapacity)
		{
			if (remainingAfter > 0)
			{
				PrintLine(line, filePath, multipleFiles);
				remainingAfter--;
			}
			else if (beforeCapacity > 0)
			{
				beforeContext.AddLine(line);
			}
		}

		// Prints the count of matches, with file name if multiple files.
		public static void PrintCount(int count, string? filePath, bool multipleFiles)
		{
			if (multipleFiles)
				Console.WriteLine(filePath + ":" + count);
			else
				Console.WriteLine(count);
		}
		// Prints message for binary file match.
		public static void PrintBinaryFileMatch(string filePath)
		{
			Console.WriteLine("Binary file " + filePath + " matches");
		}
	}
}