﻿using UnityEngine;

namespace Architect {
	public class PointSnappable : Snappable {
		
		public SnapPointType pointType;

		private SnapPoint parentPoint;
		private SnapPoint hoveredPoint;
		private SnapPoint prevPoint;

		protected override GameObject CreatePreview() {
			GameObject prev = new GameObject("SnapTransform");
			return prev;
		}

		private void Start() {
			if (startSnapped) {
				hoveredPoint = roomnet.GetLinkHover(transform.position)?.snapPoint;
				if (hoveredPoint != null) {
					hoveredPoint.Snap(preview.transform, transform);
					transform.position = preview.transform.position;
					transform.rotation = preview.transform.rotation;
					rigidbody.isKinematic = true;
					Snapped();
				}
			}
		}

		private void Update() {
			if (showPreview) {
				prevPoint = hoveredPoint;
				hoveredPoint = roomnet.GetLinkHover(transform.position)?.snapPoint;
				if (hoveredPoint != null && hoveredPoint.snapped == null) {
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
			hoveredPoint.EnablePreview();
		}

		protected override void DisablePreview() {
			base.DisablePreview();
			prevPoint.DisablePreview();
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