using UnityEngine;

namespace Architect {
	public class DoorTraverser : DefaultLinkTraverser {

		private SnapPoint parentPoint;

		public DoorTraverser(RoomLink link, RoomNetwork roomnet, SnapPoint parentPoint) : base(link, roomnet) {
			this.parentPoint = parentPoint;
		}

		public override bool Traverse(SilhouetteController controller) {
			Door door = parentPoint?.snapped?.GetComponent<Door>();
			if (base.Traverse(controller)) {
				if (door != null) {
					door.openned = false;
				}
				return true;
			} else {
				if (door != null) {
					door.openned = true;
				}
				return false;
			}
		}

	}
}
