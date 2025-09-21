namespace SharpGrep
{
	/// <summary>
	/// Parses command-line arguments into an Options object.
	/// Throws ArgumentException for invalid args, HelpRequestedException for -h/--help.
	/// </summary>
	public class ArgParser
	{
		public static string HelpText =
			"""
			usage: sharpgrep [OPTIONS] PATTERN [FILE...]
			options:
			-i            Ignore case distinctions
			-w            Match whole words only
			-o            Print only the matched parts of a line
			-A N	      Print N lines of context AFTER matching lines
			-B N    	  Print N lines of context BEFORE matching lines
			-C NU         Print N lines of context BEFORE and AFTER matching lines
			-c            Print only a count of matching lines per file
			-l            Print only names of files with matches
			-L            Print only names of files without matches
			-r            Recursively search directories
			-m N          Stop reading a file after N matching lines
			-h, --help    Show this help message and exit
			""";
		public Options ParseArgs(string[] args)
		{

			if (args.Length == 0)
			{
				throw new ArgumentException("No arguments provided.");
			}

			bool ignoreCase = false;            //-i
			bool wholeWord = false;             //-w
			bool onlyMatches = false;           //-o	
			int after = 0;                      //-A
			int before = 0;                     //-B		
			bool countOnly = false;             //-c	
			bool listWithMatches = false;       //-l
			bool listWithoutMatches = false;    //-L
			bool recursive = false;             //-r	
			int searchStop = 0;                 //-m

			// Parse options
			// Remove the parsed options from args for each iteration
			while (args.Length > 0 && args[0].StartsWith("-"))
			{
				string option = args[0];
				args = args[1..];
				switch (option)
				{
					case "-i":
						ignoreCase = true;
						break;
					case "-w":
						wholeWord = true;
						break;
					case "-o":
						onlyMatches = true;
						break;
					case "-c":
						countOnly = true;
						break;
					case "-l":
						listWithMatches = true;
						break;
					case "-L":
						listWithoutMatches = true;
						break;
					case "-r":
					case "-R":
						recursive = true;
						break;
					case "-A":
						// Expect a non-negative integer after -A
						if (args.Length == 0 || !int.TryParse(args[0], out after) || after < 0)
						{
							throw new ArgumentException("Missing or invalid number for -A option.");
						}
						args = args[1..];
						break;
					case "-B":
						// Expect a non-negative integer after -B
						if (args.Length == 0 || !int.TryParse(args[0], out before) || before < 0)
						{
							throw new ArgumentException("Missing or invalid number for -B option.");
						}
						args = args[1..];
						break;
					case "-C":
						// Expect a non-negative integer after -C
						if (args.Length == 0 || !int.TryParse(args[0], out int context) || context < 0)
						{
							throw new ArgumentException("Missing or invalid number for -C option.");
						}
						after = context;
						before = context;
						args = args[1..];
						break;
					case "-m":
						// Expect a positive integer after -m
						if (args.Length == 0 || !int.TryParse(args[0], out searchStop) || searchStop <= 0)
						{
							throw new ArgumentException("Missing or invalid number for -m option.");
						}
						args = args[1..];
						break;
					case "-h":
					case "--help":
						throw new HelpRequestedException();
					default:
						throw new ArgumentException($"Unknown option: {option}");
				}
			}

			if (args.Length == 0)
			{
				throw new ArgumentException("No pattern provided.");
			}
			// 
			if (listWithMatches && listWithoutMatches)
			{
				throw new ArgumentException("Options -l and -L cannot be used together.");
			}
			if (countOnly && (listWithMatches || listWithoutMatches))
			{
				throw new ArgumentException("Option -c cannot be used with -l or -L.");
			}

			// Get pattern and any following file paths
			string pattern = args[0];
			int nfiles = args.Length - 1;
			string[] files = new string[nfiles];
			for (int i = 0; i < nfiles; i++)
			{
				files[i] = args[i + 1];
			}

			// Return the populated Options object
			return new Options(
				pattern,
				files,
				ignoreCase,
				wholeWord,
				onlyMatches,
				after,
				before,
				countOnly,
				listWithMatches,
				listWithoutMatches,
				recursive,
				searchStop
				);

		}
	}
}