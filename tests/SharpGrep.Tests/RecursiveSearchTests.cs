public class RecursiveSearchTests
{
	[Fact]
	public void Recursive_R_FindsFilesInSubdirs()
	{
		// Arrange
		string root = "r_root";
		string sub1 = Path.Combine(root, "sub1");
		string sub2 = Path.Combine(root, "sub2");
		Directory.CreateDirectory(root);
		Directory.CreateDirectory(sub1);
		Directory.CreateDirectory(sub2);

		string f1 = Path.Combine(root, "a.txt");
		string f2 = Path.Combine(sub1, "b.txt");
		string f3 = Path.Combine(sub2, "c.txt");

		File.WriteAllText(f1, "alpha\n");
		File.WriteAllText(f2, "beta\nalpha\n");
		File.WriteAllText(f3, "gamma\n");

		var output = new StringWriter();
		Console.SetOut(output);

		// Act: -r pattern "alpha" over directory root
		// Multiple files, so expect file prefixes in the output
		string[] args = { "-r", "alpha", root };
		SharpGrep.Program.Main(args);

		// Assert
		// We'll assert both lines exist and count is 2.
		var text = output.ToString();
		int count = 0;
		using (var sr = new StringReader(text))
		{
			string? ln;
			while ((ln = sr.ReadLine()) != null)
			{
				if (ln.EndsWith(":alpha") || ln == "alpha")
				{
					// When multiple files are expanded, NormalPrint uses prefix.
					// Expect "path:alpha".
					count++;
				}
			}
		}
		Assert.Equal(2, count);
	}

	[Fact]
	public void Recursive_DirectoryWithoutR_ShowsError()
	{
		// Arrange
		string dir = "r_noflag";
		Directory.CreateDirectory(dir);
		var output = new StringWriter();
		Console.SetOut(output);

		// Act: Passing a directory without -r should produce a file-error message from FileEnumerator
		string[] args = { "alpha", dir };
		SharpGrep.Program.Main(args);

		// Assert: error prefix check (message includes "is a directory (use -r")
		Assert.Equal("Error: 'r_noflag' is a directory (use -r to search recursively)." + output.NewLine, output.ToString());
	}

	[Fact]
	public void Recursive_WithCount_c_ProducesPerFileCounts()
	{
		// Arrange
		string root = "r_count";
		string sub = Path.Combine(root, "sub");
		Directory.CreateDirectory(root);
		Directory.CreateDirectory(sub);

		string f1 = Path.Combine(root, "f1.txt");
		string f2 = Path.Combine(sub, "f2.txt");
		string f3 = Path.Combine(sub, "f3.txt");

		File.WriteAllText(f1, "hit\nmiss\nhit\n");
		File.WriteAllText(f2, "miss\nmiss\n");
		File.WriteAllText(f3, "hit\n");

		var output = new StringWriter();
		Console.SetOut(output);

		// Act
		string[] args = { "-c", "-r", "hit", root };
		SharpGrep.Program.Main(args);

		// Assert: three lines with counts; we don't fix the order, but ensure contents are correct.
		var text = output.ToString();
		int seen = 0, sum = 0;

		using (var sr = new StringReader(text))
		{
			string? ln;
			while ((ln = sr.ReadLine()) != null)
			{
				// Expect format "path:count"
				int idx = ln.LastIndexOf(':');
				Assert.True(idx > 0, "Expected 'path:count' line");
				string countStr = ln.Substring(idx + 1);
				int val;
				Assert.True(int.TryParse(countStr, out val), "Count must be integer");
				sum += val;
				seen++;
			}
		}

		Assert.Equal(3, seen);  // three files
		Assert.Equal(3, sum);   // total hits: f1=2, f2=0, f3=1
	}
}