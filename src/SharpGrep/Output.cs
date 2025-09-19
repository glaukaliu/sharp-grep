namespace SharpGrep
{
	public class Output
	{
		public static void PrintLine(string line, string? filePath, bool multipleFiles)
		{
			if (multipleFiles)
				Console.WriteLine(filePath + ":" + line);
			else
				Console.WriteLine(line);
		}


		public static void PrintBeforeContext(Context beforeContext, string? filePath, bool multipleFiles)
		{
			foreach (string beforeContextLine in beforeContext.GetLines())
			{
				PrintLine(beforeContextLine, filePath, multipleFiles);
			}
			beforeContext.Clear();
		}
		public static void PrintOnlyMatches(string line, List<(int start, int length)> matches, string? filePath, bool multipleFiles)
		{
			foreach (var match in matches)
			{
				var matchToPrint = line.Substring(match.start, match.length);
				PrintLine(matchToPrint, filePath, multipleFiles);
			}
		}

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
	}
}