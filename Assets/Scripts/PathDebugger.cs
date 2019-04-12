using UnityEngine;

namespace Architect {
	public class PathDebugger : MonoBehaviour {

		public RoomNetwork roomnet;

		private Transform start;
		private Transform target;

		private void Awake() {
			start = transform.Find("Start");
			target = transform.Find("Target");
		}

		private void Update() {
			Room startRoom = roomnet.GetRoomHover(start.position);
			Room targetRoom = roomnet.GetRoomHover(target.position);
			if (startRoom == null || targetRoom == null) {
				//Debug.Log("No start or target!");
				return;
			}
			roomnet.FindPath(startRoom, targetRoom);
		}

	}
}
