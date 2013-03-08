using System.Collections.Generic;
using System.Linq;
using StoryTeller;
using StoryTeller.Engine;
using ripple.New.Model;

namespace ripple.StoryTeller.Fixtures
{
	public class RestoreFixture : Fixture
	{
		public RestoreFixture()
		{
			Title = "When restoring Nugets";
		} 

		public IGrammar VerifyNugets()
		{
			return VerifySetOf(nugetsToRestore)
				.Titled("The restored nugets are")
				.MatchOn(x => x.Name, x => x.Version);
		}

		private IEnumerable<ConfiguredNuget> nugetsToRestore()
		{
			var solution = Retrieve<Solution>();
			var missing = solution.MissingNugets().ToList();

			return missing.Select(solution.Restore).Cast<ConfiguredNuget>();
		}
	}
}