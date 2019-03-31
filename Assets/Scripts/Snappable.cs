using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class Snappable : MonoBehaviour {

		public RoomNetwork roomnet;
		public Vector2Int size;
		public Material previewMaterial;

		private bool showPreview = false;

		private SnapGrid currentGrid;
		private GameObject preview;
		private Mesh previewMesh;
		private Throwable throwable;

		private void Awake() {
			MeshRenderer previewRenderer;
			preview = Utils.CreateMeshObject("SnapPreview", transform.parent, out previewRenderer, out previewMesh);
			previewRenderer.material = previewMaterial;
			previewMesh.CreateQuad(size.Float() * 0.1f, Vector3.forward, Vector3.right);
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
						preview.SetActive(true);
					}
					currentGrid.Snap(preview.transform, transform, size);
				} else {
					if (preview.activeInHierarchy) {
						preview.SetActive(false);
					}
				}
			}
		}

		private void PickedUp() {
			EnablePreview();
		}

		private void Detached() {
			Snap();
		}

		private void EnablePreview() {
			showPreview = true;
		}

		private void DisablePreview() {
			showPreview = false;
			if (preview.activeInHierarchy) {
				preview.SetActive(false);
			}
		}

		private void Snap() {
			if (preview.activeInHierarchy) { // Has a valid snapped
				transform.position = preview.transform.position;
				transform.rotation = preview.transform.rotation;
				GetComponent<Rigidbody>().isKinematic = true;

				DisablePreview();
			} else {
				GetComponent<Rigidbody>().isKinematic = false;
			}
		}

	}
}
