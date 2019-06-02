using UnityEngine;

namespace Architect {
	[CreateAssetMenu(fileName = "GridSettings", menuName = "Architect/GridSettings", order = 31)]
	public class RoomSettings : ScriptableObject {

		[Header("Grid")]
		public float gridSnapStep = 0.01f;
		public Material gridMaterial;
		public float gridSnapOverHeight = 0.1f;
		public float gridSnapUnderHeight = 0.05f;

		[Header("Links")]
		public float linkSnapDistance = 0.05f;

		[Header("Prefabs")]
		public GameObject doorPreview;
		public GameObject balconyPreview;

		[Header("Effects")]
		public GameObject respawnFadeOutEffect;
		public GameObject respawnFadeInEffect;
		public GameObject lightPathEffect;

		[Header("LightPath")]
		public float progressSpeed = 0.01f;

	}
}
