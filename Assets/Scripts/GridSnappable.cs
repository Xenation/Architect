using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class GridSnappable : Snappable {
		
		public Vector2Int size;
		public Material previewMaterial;

		private Room currentRoom;
		private Mesh previewMesh;
		private Transform entryDown;
		private Transform entryUp;
		private RoomLink link;

		private int matFocusIndex = -1;

		protected override GameObject CreatePreview() {
			MeshRenderer previewRenderer;
			GameObject prev = Utils.CreateMeshObject("SnapPreview", transform.parent, out previewRenderer, out previewMesh);
			previewRenderer.material = previewMaterial;
			previewMesh.CreateQuad(size.Float() * SettingsManager.I.roomSettings.gridSnapStep, Vector3.forward, Vector3.right);
			return prev;
		}

		private new void Awake() {
			base.Awake();
			entryUp = transform.Find("UpStairs");
			entryDown = transform.Find("DownStairs");
			link = gameObject.AddComponent<RoomLink>();
		}

		private void Start() {
			if (startSnapped) {
				currentRoom = roomnet.GetRoomHover(transform.position);
				if (currentRoom != null) {
					currentRoom.grid.Snap(preview.transform, transform, size);
					transform.position = preview.transform.position;
					transform.rotation = preview.transform.rotation;
					rigidbody.isKinematic = true;
					Snapped();
				}
			}
		}

		private new void Update() {
			base.Update();
			if (showPreview) {
				currentRoom = roomnet.GetRoomHover(transform.position);
				if (currentRoom != null) {
					if (!preview.activeInHierarchy) {
						EnablePreview();
					}
					currentRoom.grid.Snap(preview.transform, transform, size);
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

		protected override void Snapped() {
			base.Snapped();
			transform.SetParent(roomnet.transform);
			Room linkedRoom = roomnet.GetRoomHover(entryUp.position);
			if (linkedRoom != null) {
				link.traverser = new DefaultLinkTraverser(link, linkedRoom.GetComponentInParent<RoomNetwork>());
				link.room1 = currentRoom;
				link.entry1 = roomnet.WorldToRelativePos(entryDown.position);
				link.room2 = linkedRoom;
				link.entry2 = roomnet.WorldToRelativePos(entryUp.position);
				link.ApplyLink();
				link.isOpen = true;
			}
		}

		protected override void Unsnapped() {
			base.Unsnapped();
			transform.SetParent(null);
			if (link.valid) {
				link.isOpen = false;
				link.BreakLink();
			}
		}

	}
}
