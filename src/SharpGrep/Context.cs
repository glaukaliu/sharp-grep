
namespace SharpGrep
{
	public class Context
	{
		private Queue<string> beforeContext;
		private int _capacity;

		public Context(int capacity)
		{
			_capacity = capacity;
			beforeContext = new Queue<string>(capacity);
		}

		public void AddLine(string line)
		{
			if (_capacity == 0) return;
			if (beforeContext.Count == _capacity)
			{
				beforeContext.Dequeue();
			}
			beforeContext.Enqueue(line);
		}
		public IEnumerable<string> GetLines()
		{
			return beforeContext;
		}
		public void Clear()
		{
			beforeContext.Clear();
		}
	}

}