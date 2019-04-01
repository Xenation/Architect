using UnityEngine;

namespace Architect {
	[CreateAssetMenu(fileName = "GridSettings", menuName = "Architect/GridSettings", order = 31)]
	public class GridSettings : ScriptableObject {

		public static GridSettings i;

		public float snapStep = 0.01f;

		private void OnEnable() {
			i = this;
		}

	}
}
