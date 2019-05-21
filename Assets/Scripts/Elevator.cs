using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class Elevator : MonoBehaviour {

		public Transform movingRoot;
		public BoxCollider insideBox;

		private RoomLink link;
		private RoomNetwork roomnet;
		private LinearMapping linearMapping;

		private Transform entry1;
		private Transform entry2;

		private void Awake() {
			roomnet = GetComponentInParent<RoomNetwork>();
			link = GetComponent<RoomLink>();
			linearMapping = GetComponent<LinearMapping>();
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
			link.isOpen = true; // Elevator Link Always open
		}

		private void Start() {
			link.traverser = new ElevatorTraverser(link, this, GetComponent<LinearMapping>(), roomnet);
		}

		private void Update() {
            // TODO send linear (0,1) elevator height to Wwise
            AkSoundEngine.SetRTPCValue("Elevator_RTPC", linearMapping.value, gameObject);
			// linearMapping.value
		}

	}
}
