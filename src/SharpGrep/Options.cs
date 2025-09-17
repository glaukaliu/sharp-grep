namespace SharpGrep
{
	public class Options
	{
		public string Pattern { get;}
		public string[] Inputs { get; }
		public bool IgnoreCase { get; }
        public bool WholeWord { get; }
		public Options(string pattern, string[] inputs, bool ignoreCase, bool wholeWord)
		{
			Pattern = pattern;
			Inputs = inputs;
			IgnoreCase = ignoreCase;
			WholeWord = wholeWord;
		}
	}
}