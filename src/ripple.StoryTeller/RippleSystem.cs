using StoryTeller.Engine;

namespace ripple.StoryTeller
{
	public class RippleSystem : ISystem
	{
		public void Dispose()
		{
		}

		public IExecutionContext CreateContext()
		{
			return new SimpleExecutionContext();
		}

		public void Recycle()
		{
		}
	}
}