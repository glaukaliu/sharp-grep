namespace SharpGrep
{
	public class ArgParser
	{
		public Options ParseArgs(string[] args)
		{

			if (args.Length == 0)
			{
				throw new ArgumentException("No arguments provided.");
			}

			bool ignoreCase = false;
			bool wholeWord = false;
			bool onlyMatches = false;
			int after = 0;
			int before = 0;

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
					case "-A":
						if (args.Length == 0 || !int.TryParse(args[0], out after))
						{
							throw new ArgumentException("Missing or invalid number for -A option.");
						}
						args = args[1..];
						break;
					case "-B":
						if (args.Length == 0 || !int.TryParse(args[0], out before))
						{
							throw new ArgumentException("Missing or invalid number for -B option.");
						}
						args = args[1..];
						break;
					case "-C":
						if (args.Length == 0 || !int.TryParse(args[0], out int context))
						{
							throw new ArgumentException("Missing or invalid number for -C option.");
						}
						after = context;
						before = context;
						args = args[1..];
						break;
					


					default:
						throw new ArgumentException($"Unknown option: {option}");
				}
			}
			
			if (args.Length == 0)
			{
				throw new ArgumentException("No pattern provided.");
			}

			string pattern = args[0];
			int nfiles = args.Length - 1;
			string[] files = new string[nfiles];
			for (int i = 0; i < nfiles; i++)
			{
				files[i] = args[i + 1];
			}

			return new Options(pattern, files, ignoreCase, wholeWord, onlyMatches, after, before);

		}
	}
}