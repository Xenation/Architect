using UnityEngine;

namespace Architect {
	public class Snapable : MonoBehaviour {

		public Vector2Int size;
		public SnapGrid grid;
		public Transform reference;

		private void Update() {
			grid.Snap(transform, size);
		}

	}
}
