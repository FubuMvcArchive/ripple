using System.Collections;
using System.Collections.Generic;
using ripple.New.Nuget;

namespace ripple.New.Steps
{
	public class DownloadedNugets : IEnumerable<INugetFile>
	{
		private readonly IEnumerable<INugetFile> _nugets;

		public DownloadedNugets(IEnumerable<INugetFile> nugets)
		{
			_nugets = nugets;
		}

		public IEnumerator<INugetFile> GetEnumerator()
		{
			return _nugets.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}