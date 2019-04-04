using UnityEngine;

namespace Architect {
	public class PointSnappable : Snappable {
		
		public SnapPointType pointType;
		public GameObject previewPrefab;

		private SnapPoint parentPoint;
		private SnapPoint hoveredPoint;
		private SnapPoint prevPoint;

		protected override GameObject CreatePreview() {
			GameObject prev = Instantiate(previewPrefab);
			prev.name = "SnapPreview";
			return prev;
		}

		private void Update() {
			if (showPreview) {
				prevPoint = hoveredPoint;
				hoveredPoint = roomnet.GetLinkHover(transform.position)?.snapPoint;
				if (hoveredPoint != null) {
					if (!preview.activeInHierarchy) {
						EnablePreview();
					}
					hoveredPoint.Snap(preview.transform, transform);
				} else {
					if (preview.activeInHierarchy) {
						DisablePreview();
					}
				}
			}
		}

		protected override void EnablePreview() {
			base.EnablePreview();
			hoveredPoint.model?.SetActive(false);
		}

		protected override void DisablePreview() {
			base.DisablePreview();
			if (!isSnapped) {
				prevPoint?.model?.SetActive(true);
			}
		}

		protected override void Snapped() {
			base.Snapped();
			parentPoint = hoveredPoint;
			parentPoint.snapped = this;
		}

		protected override void Unsnapped() {
			base.Unsnapped();
			parentPoint.model?.SetActive(true);
			parentPoint.snapped = null;
			parentPoint = null;
		}

	}
}
