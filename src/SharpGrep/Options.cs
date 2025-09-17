namespace SharpGrep
{
	public class Options
	{
		public string Pattern { get;}
		public string? Path { get;}
		public Options(string pattern, string? path)
		{
			Pattern = pattern;
			Path = path;
		}
	}
}