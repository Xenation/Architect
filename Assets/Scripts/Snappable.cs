using UnityEngine;

namespace Architect {
	public class Snappable : MonoBehaviour {

		public RoomNetwork roomnet;
		public Vector2Int size;
		public Material previewMaterial;

		private SnapGrid currentGrid;
		private GameObject preview;
		private Mesh previewMesh;

		private void Awake() {
			MeshRenderer previewRenderer;
			preview = Utils.CreateMeshObject("SnapPreview", transform.parent, out previewRenderer, out previewMesh);
			previewRenderer.material = previewMaterial;
			previewMesh.CreateQuad(size.Float() * 0.1f, Vector3.forward, Vector3.right);
			preview.SetActive(false);
		}

		private void Update() {
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
}
