using UnityEngine;

namespace Architect {
	public class SnapPoint : MonoBehaviour {

		public SnapPointType type;

		public GameObject model;
		public PointSnappable snapped = null;

		[System.NonSerialized] public RoomLink link;

		public Vector3 center {
			get {
				return centerTransf.position;
			}
		}

		private RoomNetwork roomnet;

		private Transform entry1;
		private Transform entry2;
		private Transform centerTransf;

		private Outlined outlined;

		private void Awake() {
			roomnet = GetComponentInParent<RoomNetwork>();
			link = GetComponentInParent<RoomLink>();
			entry1 = transform.Find("Entry1");
			entry2 = transform.Find("Entry2");
			centerTransf = transform.Find("Center");
			Vector3 entry1Pos = roomnet.WorldToRelativePos(entry1.position);
			Vector3 entry2Pos = roomnet.WorldToRelativePos(entry2.position);
			if (Vector3.Distance(entry1Pos, roomnet.WorldToRelativePos(link.room1.center)) < Vector3.Distance(entry2Pos, roomnet.WorldToRelativePos(link.room1.center))) {
				link.entry1 = entry1Pos;
				link.entry2 = entry2Pos;
			} else {
				link.entry1 = entry2Pos;
				link.entry2 = entry1Pos;
				link.reversed = true;
			}

			outlined = GetComponent<Outlined>();
		}

		public void Snap(Transform transf, Transform reference) {
			transf.rotation = transform.rotation;
			transf.position = transform.position;
		}

		public void EnablePreview() {
			outlined.EnableHighlight();
		}

		public void DisablePreview() {
			outlined.DisableHighlight();
			if (snapped != null) {
				model.SetActive(false);
			}
		}

	}
}
