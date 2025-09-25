namespace SharpGrep
{
	/// <summary>
	/// Contains help and manual text for printing to console.
	/// </summary>
	public class PrintMessages{
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
			--color 	  Print matches in color, can be used as --color=always, --color=never, --color
			-h, --help    Show this help message and exit
			""";
		
		public static string ManualText = 
			"""
			SHARPGREP

			NAME
				shargrep - search files for patterns

			SYNOPSIS
				sharpgrep [OPTIONS] PATTERN [FILE ...]

			DESCRIPTION
				SharpGrep searches input for lines matching PATTERN.
				Supports literal and whole-word matches, context printing,
				-o fragments, counting/listing, coloring matches, recursive directory search,
				early stop per file, and binary detection.

			OPTIONS
				-i            Ignore case distinctions
				-w            Match whole words only
				-o            Print only the matched parts of a line
				-A NUM        Print NUM lines of trailing context
				-B NUM        Print NUM lines of leading context
				-C NUM        Print NUM lines of output context
				-c            Print only a count of matching lines per FILE
				-l            Print only names of FILEs with matches
				-L            Print only names of FILEs without matches
				-r            Recursively search directories
				-m NUM        Stop reading a file after NUM matching lines
				--color 	  Print matches in color, can be used as --color=always, --color=never, --color
				-h, --help    Show a short help and exit
				--man         Show this manual page and exit

			EXIT STATUS
				0  one or more matches were found
				1  no matches found
				2  argument error
				3  file error

			EXAMPLES
				sharpgrep -i cat sample.txt
				sharpgrep -w cat sample.txt
				sharpgrep -o ab sample.txt
				sharpgrep --color ab sample.txt
				sharpgrep -C 1 ERROR sample.txt
				sharpgrep -c TODO src/Program.cs
				sharpgrep -r -l alpha demo-dir
			""";

	}
}