using UnityEngine;

namespace Architect {
	public class SnapPoint : MonoBehaviour {

		public SnapPointType type;

		public GameObject model;
		public PointSnappable snapped = null;

		[System.NonSerialized] public RoomLink link;

		private RoomNetwork roomnet;

		private uint previewCounter = 0;
		private GameObject preview;

		private Transform entry1;
		private Transform entry2;

		private void Awake() {
			roomnet = GetComponentInParent<RoomNetwork>();
			link = GetComponentInParent<RoomLink>();
			entry1 = transform.Find("Entry1");
			entry2 = transform.Find("Entry2");
			Vector3 entry1Pos = roomnet.WorldToRelativePos(entry1.position);
			Vector3 entry2Pos = roomnet.WorldToRelativePos(entry2.position);
			if (Vector3.Distance(entry1Pos, roomnet.WorldToRelativePos(link.room1.transform.position)) < Vector3.Distance(entry2Pos, roomnet.WorldToRelativePos(link.room1.transform.position))) {
				link.entry1 = entry1Pos;
				link.entry2 = entry2Pos;
			} else {
				link.entry1 = entry2Pos;
				link.entry2 = entry1Pos;
			}

			InitPreview();
		}

		public void Snap(Transform transf, Transform reference) {
			transf.rotation = transform.rotation;
			transf.position = transform.position;
		}

		private void InitPreview() {
			switch (type) {
				case SnapPointType.Door:
					preview = Instantiate(SettingsManager.I.roomSettings.doorPreview, transform);
					break;
				case SnapPointType.Balcony:
					preview = Instantiate(SettingsManager.I.roomSettings.balconyPreview, transform);
					break;
			}
			preview.transform.position = transform.position;
			preview.transform.rotation = transform.rotation;
			preview.name = "SnapPreview";
			preview.SetActive(false);
		}

		public void EnablePreview() {
			previewCounter++;
			if (preview.activeInHierarchy) return; // Already enabled
			preview.SetActive(true);
			model.SetActive(false);
		}

		public void DisablePreview() {
			previewCounter--;
			if (previewCounter != 0) return;
			preview.SetActive(false);
			if (snapped == null) {
				model.SetActive(true);
			}
		}

	}
}
