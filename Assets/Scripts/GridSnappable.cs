using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class GridSnappable : MonoBehaviour {

		public RoomNetwork roomnet;
		public Vector2Int size;
		public Material previewMaterial;

		private bool showPreview = false;

		private SnapGrid currentGrid;
		private GameObject preview;
		private Mesh previewMesh;
		private Throwable throwable;

		private int matFocusIndex = -1;

		private void Awake() {
			MeshRenderer previewRenderer;
			preview = Utils.CreateMeshObject("SnapPreview", transform.parent, out previewRenderer, out previewMesh);
			previewRenderer.material = previewMaterial;
			previewMesh.CreateQuad(size.Float() * roomnet.gridSettings.snapStep, Vector3.forward, Vector3.right);
			preview.SetActive(false);

			throwable = GetComponent<Throwable>();
			throwable?.onPickUp.AddListener(PickedUp);
			throwable?.onDetachFromHand.AddListener(Detached);
		}

		private void OnDestroy() {
			throwable?.onPickUp.RemoveListener(PickedUp);
			throwable?.onDetachFromHand.RemoveListener(Detached);
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

		private void PickedUp() {
			showPreview = true;
		}

		private void Detached() {
			if (preview.activeInHierarchy) { // Has a valid snap point -> Snap
				transform.position = preview.transform.position;
				transform.rotation = preview.transform.rotation;
				GetComponent<Rigidbody>().isKinematic = true;
			} else { // Re-enable physics
				GetComponent<Rigidbody>().isKinematic = false;
			}
			showPreview = false;
			DisablePreview();
		}

		private void EnablePreview() {
			preview.SetActive(true);
			matFocusIndex = GridManager.I.GetInactiveFocusIndex();
			GridManager.I.ActivateFocus(matFocusIndex);
			float focusRadius = size.magnitude / 2f * GridSettings.I.snapStep;
			GridManager.I.SetFocusRadius(matFocusIndex, focusRadius, focusRadius * .75f);
		}

		private void DisablePreview() {
			if (preview.activeInHierarchy) {
				preview.SetActive(false);
				GridManager.I.DeactivateFocus(matFocusIndex);
				matFocusIndex = -1;
			}
		}

	}
}
