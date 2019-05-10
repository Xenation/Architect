using System.Collections.Generic;

namespace Architect {
	public class OutlinedManager : AutoSingleton<OutlinedManager> {

		public HashSet<Outlined> outlinedObjects = new HashSet<Outlined>();

	}
}
