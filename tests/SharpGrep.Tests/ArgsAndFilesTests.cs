using System;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;



public class ArgAndFileTests
{
	[Fact]
	public void MissingArgumentTest()
	{
		// Arrange
		string[] args = { };
		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		SharpGrep.Program.Main(args);

		// Assert (Program prints: "Argument error: Argument Error")
		Assert.Equal("Argument error: No arguments provided." + output.NewLine, output.ToString());
	}

	[Fact]
	public void UnknownOptionTest()
	{
		// Arrange
		string[] args = { "-Q", "pat" };
		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		SharpGrep.Program.Main(args);

		// Assert
		Assert.Equal("Argument error: Unknown option: -Q" + output.NewLine, output.ToString());
	}

	[Fact]
	public void MissingFileTest()
	{
		// Arrange
		string[] args = { "foo", "no_such_file.txt" };
		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		SharpGrep.Program.Main(args);

		// Assert:
		Assert.StartsWith("Error: File or directory not found:", output.ToString());
	}

	[Fact]
	public void StdinPathWorksTest()
	{
		// Arrange
		var input = new StringReader("Sample\nStrings\nFor\nTesting");
		Console.SetIn(input);
		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		string[] args = { "o" }; // simple substring "o"
		SharpGrep.Program.Main(args);

		// Assert: Lines containing "o", case-sensitive
		string expected = "For" + output.NewLine; // only "For"
		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void ArgumentAfterPatternTest()
	{
		string file = "sample.txt";
		string content = "Sample content";
		File.WriteAllText(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		string currentPath = Directory.GetCurrentDirectory();
		// Act
		string[] args = { "ab", file, "-o" };
		SharpGrep.Program.Main(args);

		// Assert
		string expected = "Error: File or directory not found: -o\n";

		Assert.Equal(expected, output.ToString());
	}

	[Fact]
	public void BinaryFileTest()
	{
		// Arrange
		byte[] content = new byte[] { 0x01, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x65, 0x65, 0x65, 0x0A, 0x0B, 0x0F };
		string file = "sampleBinary.dll";
		File.WriteAllBytes(file, content);

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		string[] args = { "e", file }; // pattern that matches byte value 0x65 ('e')
		SharpGrep.Program.Main(args);

		// Assert
		string expected = "Binary file " + file + " matches" + output.NewLine;
		Assert.Equal(expected, output.ToString());

		// Cleanup
		File.Delete(file);
	}
}
