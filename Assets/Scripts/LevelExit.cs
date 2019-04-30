using UnityEngine;
using Valve.VR;

namespace Architect {
	public class LevelExit : MonoBehaviour {

		private RoomNetwork roomnet;
		private RoomLink link;

		private void Awake() {
			roomnet = GetComponentInParent<RoomNetwork>();
			link = GetComponent<RoomLink>();
			Vector3 entry1Pos = roomnet.WorldToRelativePos(transform.Find("Entry1").position);
			Vector3 entry2Pos = roomnet.WorldToRelativePos(transform.Find("Entry2").position);
			if (Vector3.Distance(entry1Pos, roomnet.WorldToRelativePos(link.room1.transform.position)) < Vector3.Distance(entry2Pos, roomnet.WorldToRelativePos(link.room1.transform.position))) {
				link.entry1 = entry1Pos;
				link.entry2 = entry2Pos;
			} else {
				link.entry1 = entry2Pos;
				link.entry2 = entry1Pos;
			}
			link.isOpen = true;
		}

		private void Start() {
			link.traverser = new ExitTraverser(link, roomnet, GetComponent<SteamVR_LoadLevel>());
		}

	}
}
