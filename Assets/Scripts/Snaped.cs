using UnityEngine;

namespace Architect {
	public class Snaped : MonoBehaviour {

		public SnapGrid grid;
		public Transform reference;

		private void Update() {
			transform.position = grid.SnapPosition(reference.position);
			transform.rotation = grid.SnapRotation(reference.rotation);
		}

	}
}
