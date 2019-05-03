using UnityEngine;

namespace Architect {
	public abstract class LinkTraverser : Traverser {

		protected RoomLink link;
		protected Vector3 target;
		protected Room targetRoom;

		public void SetTarget(Room currentRoom) {
			targetRoom = link.GetOther(currentRoom);
			target = link.GetEntry(targetRoom);
		}

	}
}
