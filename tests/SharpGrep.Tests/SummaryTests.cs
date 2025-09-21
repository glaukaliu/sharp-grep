public class SummaryTests
{
	[Fact]
	public void CountOnlySingleFile()
	{
		// Arrange
		string file = "c_single.txt";
		File.WriteAllText(file, "a\nb\na\nc\n");

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		string[] args = { "-c", "a", file };
		SharpGrep.Program.Main(args);

		// Assert (two lines match "a")
		string expected = "2" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void CountOnlyMultipleFilesTest()
	{
		// Arrange
		string f1 = "c_multi1.txt";
		string f2 = "c_multi2.txt";
		File.WriteAllText(f1, "xx\nyy\n");
		File.WriteAllText(f2, "yy\nyy\nxx\n");

		var output = new StringWriter();
		Console.SetOut(output);

		// Act (pattern "yy")
		string[] args = { "-c", "yy", f1, f2 };
		SharpGrep.Program.Main(args);

		// Assert (prefix when multiple files)
		string expected =
			"c_multi1.txt:1" + output.NewLine +
			"c_multi2.txt:2" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void CountOnlyAndMStopTest()
	{
		// Arrange
		string file = "c_m.txt";
		// Three matching lines for "hit"
		File.WriteAllText(file, "hit\nmiss\nhit\nmiss\nhit\n");

		var output = new StringWriter();
		Console.SetOut(output);

		// Act: -c -m 2 → stop counting at 2 matches
		string[] args = { "-c", "-m", "2", "hit", file };
		SharpGrep.Program.Main(args);

		// Assert
		string expected = "2" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void MStopTest()
	{
		// Arrange
		string file = "m_whole.txt";
		File.WriteAllText(file, "m1\nx\nm2\nx\nm3\nx\n");

		var output = new StringWriter();
		Console.SetOut(output);

		// Act: stop after 2 matches of "m"
		string[] args = { "-m", "2", "m", file };
		SharpGrep.Program.Main(args);

		// Assert: only first two matching lines printed ("m1", "m2")
		string expected =
			"m1" + output.NewLine +
			"m2" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void MStopAndOnlyMatchesTest()
	{
		// Arrange
		string file = "m_frag.txt";
		// Line1: "ab", Line2: "ab"
		File.WriteAllText(file, "ababab\nxxabyy\nab\n");

		var output = new StringWriter();
		Console.SetOut(output);

		// Act: -o -m 1 → stop after first matching LINE
		string[] args = { "-o", "-m", "1", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Line1 : "ab","ab","ab"
		string expected =
			"ab" + output.NewLine +
			"ab" + output.NewLine +
			"ab" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void searchStop_AfterContext()
	{
		// Arrange
		string file = "m_after.txt";
		// Two matching lines: 1 and 4
		File.WriteAllText(file, "hit\nA1\nA2\nhit\nA3\n");

		var output = new StringWriter();
		Console.SetOut(output);

		// Act: -A 2 -m 1 → print first match and do not continue
		string[] args = { "-A", "2", "-m", "1", "hit", file };
		SharpGrep.Program.Main(args);

		// Assert: Print the first match, and its 2 lines of after context.
		string expected =
			"hit" + output.NewLine +
			"A1" + output.NewLine +
			"A2" + output.NewLine;
		Assert.Equal(expected, output.ToString());
    }
}