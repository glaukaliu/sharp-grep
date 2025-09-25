namespace SharpGrep
{
	/// <summary>
	/// Handles all output printing logic.
	/// </summary>
	public class Output
	{
		// ANSI sequences for red text
		private const string AnsiStart = "\u001b[1;31m";
        private const string AnsiEnd   = "\u001b[0m";
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

		public static void PrintOnlyMatchesColored(string line, List<(int start, int length)> matches, string? filePath, bool multipleFiles)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                var m = matches[i];
                string frag = line.Substring(m.start, m.length);
                PrintLine(AnsiStart + frag + AnsiEnd, filePath, multipleFiles);
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
		// Builds a colored line for output.
		public static string BuildColoredLine(string line, List<(int start, int length)> spans)
        {
            if (spans.Count == 0) return line;

            var sb = new System.Text.StringBuilder(line.Length + spans.Count * 10);
            int pos = 0;

            for (int i = 0; i < spans.Count; i++)
            {
                int s = spans[i].start;
                int len = spans[i].length;

                if (s > pos)
                    sb.Append(line.Substring(pos, s - pos));

                sb.Append(AnsiStart);
                sb.Append(line.Substring(s, len));
                sb.Append(AnsiEnd);

                pos = s + len;
            }
            if (pos < line.Length)
                sb.Append(line.Substring(pos));

            return sb.ToString();
        }
	}
}