using UnityEngine;

namespace Architect {
	public class RoomLink : MonoBehaviour {

		public Room room1;
		public Room room2;

		public bool overrideOpen = false;
		[System.NonSerialized] public SnapPoint snapPoint;

		public bool isOpen {
			get {
				return overrideOpen || snapPoint.snapped != null;
			}
		}

		private void Awake() {
			snapPoint = GetComponentInChildren<SnapPoint>();
			if (snapPoint != null) {
				snapPoint.parentLink = this;
			}
		}

		public Room GetOther(Room r) {
			return (r == room1) ? room2 : room1;
		}

	}
}
