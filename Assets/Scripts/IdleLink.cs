using UnityEngine;

namespace Architect {
	public class IdleLink : MonoBehaviour {

		[System.NonSerialized] public RoomLink link;

		private RoomNetwork roomnet;

		private Transform entry1;
		private Transform entry2;

		private void Awake() {
			roomnet = GetComponentInParent<RoomNetwork>();
			link = GetComponentInParent<RoomLink>();
			entry1 = transform.Find("Entry1");
			entry2 = transform.Find("Entry2");
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
			link.isOpen = true;
		}

	}
}
