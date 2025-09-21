

public class StdinTests
{
	[Fact]
	public void Stdin_SimpleMatch_WholeLine()
	{
		// Arrange
		var input = new StringReader("alpha\nBeta\nALPHA\nxalpha\nnone\n");
		Console.SetIn(input);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act (case-sensitive, matches only "alpha" and "xalpha")
		string[] args = { "alpha" };
		SharpGrep.Program.Main(args);

		// Assert
		string expected =
			"alpha" + output.NewLine +
			"xalpha" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void Stdin_CountOnly_c_Ignores_o_and_Context()
	{
		// Arrange
		var input = new StringReader("ab\nx\nab\nZ\nab\n");
		Console.SetIn(input);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act: -c over STDIN counts matching LINES; -o/-A/-B/-C do not affect counting
		string[] args = { "-c", "-o", "-A", "2", "ab" };
		SharpGrep.Program.Main(args);

		// Assert (three matching lines: 1,3,5)
		string expected = "3" + output.NewLine;
		Assert.Equal(expected, output.ToString());
	}
}
