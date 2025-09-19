using System;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace SharpGrep.Tests
{
	public class ArgAndFileTests
	{
		[Fact]
		public void MissingArgument()
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
		public void UnknownOption()
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
		public void MissingFile()
		{
			// Arrange
			string[] args = { "foo", "no_such_file.txt" };
			var output = new StringWriter();
			Console.SetOut(output);

			// Act
			SharpGrep.Program.Main(args);

			// Assert:
			Assert.StartsWith("Error opening file: ", output.ToString());
		}

		[Fact]
		public void StdinPath_Works()
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
		public void ArgumentAfterPattern()
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
			string expected = "Error opening file: Could not find file '" +
			Path.Combine(currentPath,"-o") + "'." + output.NewLine;

			Assert.Equal(expected, output.ToString());
		}
	}
}