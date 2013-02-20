using StoryTeller.Engine;

namespace ripple.StoryTeller
{
	public class RippleSystem : ISystem
	{
		public IExecutionContext CreateContext()
		{
			return new SimpleExecutionContext();
		}

		public void Recycle()
		{
		}

		public void Dispose()
		{
		}
	}
}