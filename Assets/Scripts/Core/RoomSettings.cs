using UnityEngine;

namespace Architect {
	[CreateAssetMenu(fileName = "GridSettings", menuName = "Architect/GridSettings", order = 31)]
	public class RoomSettings : ScriptableObjectSingleton<RoomSettings> {

		[Header("Grid")]
		public float gridSnapStep = 0.01f;
		public Material gridMaterial;
		public float gridSnapOverHeight = 0.1f;
		public float gridSnapUnderHeight = 0.05f;

		[Header("Links")]
		public float linkSnapDistance = 0.05f;

		private void OnEnable() {
			instance = this;
		}

	}
}
