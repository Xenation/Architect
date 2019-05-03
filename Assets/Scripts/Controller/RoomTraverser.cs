using UnityEngine;

namespace Architect {
	public class RoomTraverser : Traverser {
		
		private Room room;
		private Vector3 target;

		public RoomTraverser(Room room, RoomNetwork roomnet) {
			this.room = room;
			this.roomnet = roomnet;
		}

		public void SetTarget(RoomLink link) {
			target = link.GetEntry(room);
		}

		public void SetTarget(Vector3 relPos) {
			target = relPos;
		}

		public override bool Traverse(SilhouetteController controller) {
			Vector3 toTarget = target - controller.transform.localPosition;
			Vector3 direction = toTarget.normalized;
			controller.transform.localPosition += direction * controller.speed * Time.deltaTime;
			controller.transform.localRotation = Quaternion.Euler(0f, Vector3.SignedAngle(Vector3.forward, direction, Vector3.up), 0f);
			if (toTarget.magnitude < 0.01f) { // Arrived at target link entry
				return true;
			}
			return false;
		}

	}
}
