using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class GridSnappable : Snappable {
		
		public Vector2Int size;
		public Material previewMaterial;

		private Outlined previewOutlined;

		private Room currentRoom;
		private Transform previewRoot;
		private Transform entryDown;
		private Transform entryUp;
		private RoomLink link;
		private Interactable interactable;
		private Transform wallCheck1;
		private Transform wallCheck2;

		private int matFocusIndex = -1;

		protected override GameObject CreatePreview() {
			previewRoot = transform.Find("Preview");
			GameObject modelGO = transform.Find("Model").gameObject;
			GameObject prev = Instantiate(modelGO, previewRoot);
			prev.name = "PreviewModel";
			prev.transform.localPosition = Vector3.zero;
			Renderer[] renderers = prev.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderers) {
				renderer.renderingLayerMask = 0;
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			}
			previewOutlined = previewRoot.GetComponent<Outlined>();
			previewOutlined.listMode = Outlined.Mode.Whitelist;
			previewOutlined.objectList.Add(prev.transform);
			return previewRoot.gameObject;
		}

		private new void Awake() {
			base.Awake();
			entryUp = transform.Find("UpStairs");
			entryDown = transform.Find("DownStairs");
			link = gameObject.AddComponent<RoomLink>();
			interactable = GetComponent<Interactable>();
			wallCheck1 = transform.Find("WallCheck1");
			wallCheck2 = transform.Find("WallCheck2");
		}

		private void Start() {
			if (startSnapped) {
				currentRoom = roomnet.GetRoomGridHover(transform.position);
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
			interactable.enabled = !link.freezed;

			if (showPreview) {
				currentRoom = roomnet.GetRoomGridHover(transform.position);
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
			previewOutlined.EnableHighlight();
		}

		protected override void DisablePreview() {
			base.DisablePreview();
			GridManager.I.DeactivateFocus(matFocusIndex);
			matFocusIndex = -1;
			previewOutlined.EnableHighlight();
		}

		protected override void Snapped() {
			base.Snapped();
			transform.SetParent(roomnet.transform);
			Vector3 wallVec = wallCheck2.position - wallCheck1.position;
			bool wallBlocked = Physics.Raycast(new Ray(wallCheck1.position, wallVec.normalized), wallVec.magnitude, LayerMask.GetMask("Decor"));
			if (!wallBlocked) {
				Room linkedRoom = roomnet.GetRoomGridHover(entryUp.position);
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
