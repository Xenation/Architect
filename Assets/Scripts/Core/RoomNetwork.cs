using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class RoomNetwork : MonoBehaviour {

		public Room startingRoom;

		private List<Room> rooms = new List<Room>();
		private List<RoomLink> links = new List<RoomLink>();

		private void Awake() {
			BuildNetwork();
		}

		private void BuildNetwork() {
			foreach (Transform child in transform) {
				RoomLink link = child.GetComponent<RoomLink>();
				if (link != null) {
					link.room1.RegisterLink(link);
					link.room2.RegisterLink(link);
					links.Add(link);
				}
				Room room = child.GetComponent<Room>();
				if (room != null) {
					rooms.Add(room);
				}
			}
		}

		public Room GetRoomHover(Vector3 pos) {
			foreach (Room room in rooms) {
				if (room.grid.IsOverGrid(pos, SettingsManager.I.roomSettings.gridSnapOverHeight, SettingsManager.I.roomSettings.gridSnapUnderHeight)) {
					return room;
				}
			}
			return null;
		}

		public RoomLink GetLinkHover(Vector3 pos) {
			foreach (RoomLink link in links) {
				if (Vector3.Distance(pos, link.snapPoint.transform.position) < SettingsManager.I.roomSettings.linkSnapDistance) {
					return link;
				}
			}
			return null;
		}

	}
}
