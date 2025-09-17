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

			return new Options(pattern, files, ignoreCase, wholeWord);

		}
	}
}