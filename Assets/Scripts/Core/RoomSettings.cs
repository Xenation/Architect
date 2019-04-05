using UnityEngine;

namespace Architect {
	[CreateAssetMenu(fileName = "GridSettings", menuName = "Architect/GridSettings", order = 31)]
	public class RoomSettings : ScriptableObject {

		private static RoomSettings instance = null;
		public static RoomSettings I {
			get {
				if (instance == null) {
					Debug.LogWarning("Accessing RoomSettings instance before assignement!");
				}
				return instance;
			}
		}

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
