using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class PointSnappable : Snappable {
		
		public SnapPointType pointType;

		private SnapPoint parentPoint;
		private SnapPoint hoveredPoint;
		private SnapPoint prevPoint;
		private Interactable interactable;

		private Transform centerTransf;
		private Outlined outlined;

		protected override GameObject CreatePreview() {
			GameObject prev = new GameObject("SnapTransform");
			return prev;
		}

		protected override void Awake() {
			base.Awake();
			interactable = GetComponent<Interactable>();
			centerTransf = transform.Find("Center");
			outlined = GetComponent<Outlined>();
		}

		private void Start() {
			if (startSnapped) {
				hoveredPoint = roomnet.GetPointHover(centerTransf.position);
				if (hoveredPoint != null) {
					hoveredPoint.Snap(preview.transform, transform);
					transform.position = preview.transform.position;
					transform.rotation = preview.transform.rotation;
					rigidbody.isKinematic = true;
					Snapped();
				}
			}
		}

		private new void Update() {
			base.Update();
			if (parentPoint != null) {
				interactable.enabled = !parentPoint.link.freezed;
			}
			if (showPreview) {
				prevPoint = hoveredPoint;
				hoveredPoint = roomnet.GetPointHover(centerTransf.position);
				if (hoveredPoint != null && hoveredPoint.snapped == null && hoveredPoint.type == pointType) {
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

		private void OnHandHoverBegin(Hand hand) {
			if (parentPoint != null && parentPoint.link.room1.isConnectedToStart) {
				outlined.outlineColor = SettingsManager.I.roomSettings.breakingOutlineColor;
			} else {
				outlined.outlineColor = SettingsManager.I.roomSettings.defaultOutlineColor;
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
			transform.SetParent(roomnet.transform);
			parentPoint = hoveredPoint;
			parentPoint.snapped = this;
			parentPoint.link.isOpen = true;
			parentPoint.gameObject.SendMessage("OnHintInteraction", SendMessageOptions.DontRequireReceiver);
		}

		protected override void Unsnapped() {
			base.Unsnapped();
			transform.SetParent(null);
			parentPoint.model?.SetActive(true);
			parentPoint.snapped = null;
			parentPoint.link.isOpen = false;
			parentPoint = null;
		}

	}
}
