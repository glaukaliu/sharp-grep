
public class OptionsTests
{
	[Fact]

	public void CaseInsensitivePrintAllMatches()
	{
		// Arrange
		string file = "case.txt";
		string content = "Cat\ncater\nscatter\nconcatenate\nA cat naps\n";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// case-insesitive search for "cat"
		string[] args = { "-i", "cat", file };
		SharpGrep.Program.Main(args);

		// Assert (all lines should match)
		string expected =
			"Cat" + output.NewLine +
			"cater" + output.NewLine +
			"scatter" + output.NewLine +
			"concatenate" + output.NewLine +
			"A cat naps" + output.NewLine;

		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void WholeWordTest()
	{
		// Arrange
		string file = "word.txt";
		string content = "Cat\ncater\nscatter\nconcatenate\nA cat naps\nthe cat sat\n";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -w matches only whole words "cat"
		string[] args = { "-w", "cat", file };
		SharpGrep.Program.Main(args);

		// Assert (only lines that contain "cat" as a whole word)
		string expected =
			"A cat naps" + output.NewLine +
			"the cat sat" + output.NewLine;

		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void WholeWordAndCaseInsensitiveTest()
	{
		// Arrange
		string file = "word_case.txt";
		string content = "Cat\ncater\nscatter\nconcatenate\nA cat naps\nthe cat sat\n";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -w matches only whole words "cat", -i makes it case-insensitive
		string[] args = { "-w", "-i", "cat", file };
		SharpGrep.Program.Main(args);

		// Assert (only lines that contain "cat" as a whole word, case insensitive)
		string expected =
			"Cat" + output.NewLine +
			"A cat naps" + output.NewLine +
			"the cat sat" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void OnlyMatchesTest()
	{
		// Arrange
		string file = "frags.txt";
		string content = "abab ab\nxxabyy\nbbb\n";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -o prints fragments only
		string[] args = { "-o", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Matches: line1: "ab","ab","ab"  (two in "abab", one in "ab")
		//          line2: "ab"
		string expected =
			"ab" + output.NewLine +
			"ab" + output.NewLine +
			"ab" + output.NewLine +
			"ab" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void OnlyMatchesAndWholeWordTest()
	{
		string file = "frags.txt";
		string content = "abab ab\nxxabyy\naba ab\n";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -o prints fragments only
		string[] args = { "-o", "-w", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Matches: line1: "ab"
		//          line3: "ab"
		string expected =
			"ab" + output.NewLine +
			"ab" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void OnlyMatchesAndCaseInsensitiveTest()
	{
		string file = "frags.txt";
		string content = "abab Ab AB\nxxabyy\naba ABA ab\n";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -o prints fragments only
		string[] args = { "-o", "-i", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Matches: line1: "ab", "ab", "Ab", "AB"
		//          line2: "ab"
		//          line3: "ab", "AB", "ab"
		string expected =
			"ab" + output.NewLine +
			"ab" + output.NewLine +
			"Ab" + output.NewLine +
			"AB" + output.NewLine +
			"ab" + output.NewLine +
			"ab" + output.NewLine +
			"AB" + output.NewLine +
			"ab" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void OnlyMatchesAndCaseInsensitiveAndWholeWordTest()
	{
		string file = "frags.txt";
		string content = "abab Ab AB\nxxabyy\naba ABA ab\n";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -o prints fragments only
		string[] args = { "-o", "-i", "-w", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Matches: line1: "Ab", "AB"
		//          line3: "ab"
		string expected =
			"Ab" + output.NewLine +
			"AB" + output.NewLine +
			"ab" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void AfterPrintLinesTest()
	{
		string file = "frags.txt";
		string content = "abab\nnomatch\nnomatch\nab\n\nafterempty\nnotprinted";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -o prints fragments only
		string[] args = { "-A", "2", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Matches: line1: "abab"
		// 	  Adds: line2: "nomatch"
		// 	  Adds: line3: "nomatch"
		// 			line4: "ab"
		// 	  Adds: line5: "" (empty line)
		// 	  Adds: line6: "afterempty"
		//   Skips: line7: "notprinted"

		string expected =
			"abab" + output.NewLine +
			"nomatch" + output.NewLine +
			"nomatch" + output.NewLine +
			"ab" + output.NewLine +
			"" + output.NewLine +
			"afterempty" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void BeforePrintLinesTest()
	{
		string file = "frags.txt";
		string content = "abab\nnomatch\nnomatch\nab\n\nafterempty\nnotprinted";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -o prints fragments only
		string[] args = { "-B", "2", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Matches: line1: "abab"
		// 	  Adds: line2: "nomatch"
		// 	  Adds: line3: "nomatch"
		// 			line4: "ab"
		//   Skips: rest

		string expected =
			"abab" + output.NewLine +
			"nomatch" + output.NewLine +
			"nomatch" + output.NewLine +
			"ab" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}
	[Fact]
	public void BeforeAndAfterPrintLinesTest()
	{
		string file = "frags.txt";
		string content = "abab\nnomatch\nnomatch\nab\n\nafterempty\nnotprinted";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -o prints fragments only
		string[] args = { "-C", "1", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Matches: line1: "abab"
		// 	  Adds: line2: "nomatch"
		// 	  Adds: line3: "nomatch"
		// 			line4: "ab"
		// 	  Adds: line5: "" (empty line)
		//   Skips: rest

		string expected =
			"abab" + output.NewLine +
			"nomatch" + output.NewLine +
			"nomatch" + output.NewLine +
			"ab" + output.NewLine +
			"" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}
	[Fact]
	public void BeforeAndAfterAndOnlyMatchesTest()
	{
		string file = "frags.txt";
		string content = "abcde\nnomatch\nnomatch\nab\n\nafterempty\nnotprinted";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		// -o prints fragments only
		string[] args = { "-C", "1", "-o", "ab", file };
		SharpGrep.Program.Main(args);

		// Assert
		// Matches: line1: "ab"
		// 	  Adds: line2: "nomatch"
		// 	  Adds: line3: "nomatch"
		// 			line4: "ab"
		// 	  Adds: line5: "" (empty line)
		//   Skips: rest

		string expected =
			"ab" + output.NewLine +
			"nomatch" + output.NewLine +
			"nomatch" + output.NewLine +
			"ab" + output.NewLine +
			"" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

}	

