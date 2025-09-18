namespace SharpGrep
{
	public class Options
	{
		public string Pattern { get;}
		public string[] Inputs { get; }
		public bool IgnoreCase { get; }
        public bool WholeWord { get; }
		public bool OnlyMatches { get; }
		public int After { get; }
		public int Before { get; }

		public Options(
			string pattern,
			string[] inputs,
			bool ignoreCase,
			bool wholeWord,
			bool onlyMatches,
			int after,
			int before
			)
		{
			Pattern = pattern;
			Inputs = inputs;
			IgnoreCase = ignoreCase;
			WholeWord = wholeWord;
			OnlyMatches = onlyMatches;
			After = after;
			Before = before;
		}
	}
}