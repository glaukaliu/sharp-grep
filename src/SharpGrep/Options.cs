namespace SharpGrep
{
	/// <summary>
	/// Holds all parsed command-line options.
	/// </summary>
	public class Options
	{
		public string Pattern { get;}				
		public string[] Inputs { get; }					
		public bool IgnoreCase { get; }             	//-i
        public bool WholeWord { get; }					//-w
		public bool OnlyMatches { get; }				//-o
		public int After { get; }						//-A
		public int Before { get; }						//-B
		public bool CountOnly { get; }					//-c
		public bool ListWithMatches { get; }			//-l
		public bool ListWithoutMatches { get; }			//-L
		public bool Recursive { get; }					//-r
		public int SearchStop { get; }                  //-s
		public ColorMode Color { get; }					//--color		


		public Options(
			string pattern,
			string[] inputs,
			bool ignoreCase,
			bool wholeWord,
			bool onlyMatches,
			int after,
			int before,
			bool countOnly,
			bool listWithMatches,
			bool listWithoutMatches,
			bool recursive,
			int searchStop,
			ColorMode color
			)
		{
			Pattern = pattern;
			Inputs = inputs;
			IgnoreCase = ignoreCase;
			WholeWord = wholeWord;
			OnlyMatches = onlyMatches;
			After = after;
			Before = before;
			CountOnly = countOnly;
			ListWithMatches = listWithMatches;
			ListWithoutMatches = listWithoutMatches;
			Recursive = recursive;
			SearchStop = searchStop;
			Color = color;  
		}
	}
}