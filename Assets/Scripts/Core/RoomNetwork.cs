using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class RoomNetwork : MonoBehaviour {

		public Room startingRoom;

		private List<Room> rooms = new List<Room>();

		private void Awake() {
			BuildNetwork();
		}

		private void BuildNetwork() {
			foreach (Transform child in transform) {
				RoomLink link = child.GetComponent<RoomLink>();
				if (link != null) {
					link.room1.RegisterLink(link);
					link.room2.RegisterLink(link);
				}
				Room room = child.GetComponent<Room>();
				if (room != null) {
					rooms.Add(room);
				}
			}
		}

		public Room GetRoomHover(Vector3 pos) {
			foreach (Room room in rooms) {
				if (room.grid.IsOverGrid(pos, 1f)) {
					return room;
				}
			}
			return null;
		}

	}
}
