using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class GridSnappable : Snappable {
		
		public Vector2Int size;
		public Material previewMaterial;

		private SnapGrid currentGrid;
		private Mesh previewMesh;

		private int matFocusIndex = -1;

		protected override GameObject CreatePreview() {
			MeshRenderer previewRenderer;
			GameObject prev = Utils.CreateMeshObject("SnapPreview", transform.parent, out previewRenderer, out previewMesh);
			previewRenderer.material = previewMaterial;
			previewMesh.CreateQuad(size.Float() * SettingsManager.I.roomSettings.gridSnapStep, Vector3.forward, Vector3.right);
			return prev;
		}

		private void Start() {
			if (startSnapped) {
				currentGrid = roomnet.GetRoomHover(transform.position)?.grid;
				if (currentGrid != null) {
					currentGrid.Snap(preview.transform, transform, size);
					transform.position = preview.transform.position;
					transform.rotation = preview.transform.rotation;
					rigidbody.isKinematic = true;
					Snapped();
				}
			}
		}

		private void Update() {
			if (showPreview) {
				currentGrid = roomnet.GetRoomHover(transform.position)?.grid;
				if (currentGrid != null) {
					if (!preview.activeInHierarchy) {
						EnablePreview();
					}
					currentGrid.Snap(preview.transform, transform, size);
				} else {
					if (preview.activeInHierarchy) {
						DisablePreview();
					}
				}
				if (matFocusIndex != -1) {
					GridManager.I.SetFocusPosition(matFocusIndex, preview.transform.position);
				}
			}
		}

		protected override void EnablePreview() {
			base.EnablePreview();
			matFocusIndex = GridManager.I.GetInactiveFocusIndex();
			GridManager.I.ActivateFocus(matFocusIndex);
			float focusRadius = size.magnitude / 2f * SettingsManager.I.roomSettings.gridSnapStep;
			GridManager.I.SetFocusRadius(matFocusIndex, focusRadius, focusRadius * .75f);
		}

		protected override void DisablePreview() {
			base.DisablePreview();
			GridManager.I.DeactivateFocus(matFocusIndex);
			matFocusIndex = -1;
		}

	}
}
