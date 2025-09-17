namespace SharpGrep
{
	public class ArgParser
	{
		public Options ParseArgs(string[] args)
		{
			string pattern = args[0];
			string? file = null;
			if (args.Length > 1)
			{
				file = args[1];

			}
			return new Options(pattern, file);

		}
	}
}