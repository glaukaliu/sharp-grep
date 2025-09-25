namespace SharpGrep
{
	/// <summary>
	/// Defines exit codes used by the application.
	/// Success = 0
	/// NoMatchesFound = 1
	/// ArgumentError = 2
	/// FileError = 3
	/// </summary>
	public class ExitCodes
	{
		public const int Success = 0;
		public const int NoMatchesFound = 1;
		public const int ArgumentError = 2;
		public const int FileError = 3;
	}
}