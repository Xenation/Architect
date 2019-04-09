using UnityEngine;

namespace Architect {
	public class RoomLink : MonoBehaviour {

		public Room room1;
		public Room room2;

		[System.NonSerialized] public SnapPoint snapPoint;

		private void Awake() {
			snapPoint = GetComponentInChildren<SnapPoint>();
		}

		public Room GetOther(Room r) {
			return (r == room1) ? room2 : room1;
		}

	}
}
