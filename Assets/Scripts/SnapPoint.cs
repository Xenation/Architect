using UnityEngine;

namespace Architect {
	public class SnapPoint : MonoBehaviour {

		public SnapPointType type;

		public GameObject model;
		public PointSnappable snapped = null;

		public void Snap(Transform transf, Transform reference) {
			transf.rotation = transform.rotation;
			transf.position = transform.position;
		}

	}
}
