using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class RoomTraverser : Traverser {
		
		private Room room;
		private RoomNav nav;
		private Vector3 target;
		private List<NavNode> path = new List<NavNode>();
		private int pathIndex = 0;

		public RoomTraverser(Room room, RoomNetwork roomnet) {
			this.room = room;
			nav = new RoomNav(room);
			nav.BuildGraph();
			this.roomnet = roomnet;
		}

		public void SetTarget(RoomLink link) {
			target = link.GetEntry(room);
		}

		public void SetTarget(Vector3 relPos) {
			target = relPos;
		}

		public override bool Traverse(SilhouetteController controller) {
			if (nav.nodeCount == 0) {
				return FallbackTraverse(controller);
			}

			if (path.Count == 0) {
				nav.FindPath(controller.transform.localPosition, target, path);
				//Debug.Log("Path length: " + path.Count);
				pathIndex = 0;
			}

			Vector3 direction;
			if (pathIndex >= path.Count - 1) {
				Vector3 toTarget = target - controller.transform.localPosition;
				if (toTarget.magnitude < 0.01f) { // Arrived at target link entry
					//Debug.Log("Arrived at target");
					path.Clear();
					return true;
				}
				direction = toTarget.normalized;
			} else {
				Vector3 toNode = roomnet.WorldToRelativePos(path[pathIndex].transform.position) - controller.transform.localPosition;
				direction = toNode.normalized;
				if (toNode.magnitude < 0.01f) { // Arrived at node
					//Debug.Log("Arrived at node");
					pathIndex++;
				}
			}
			controller.transform.localPosition += direction * controller.speed * Time.deltaTime;
			controller.transform.localRotation = Quaternion.Euler(0f, Vector3.SignedAngle(Vector3.forward, direction, Vector3.up), 0f);

			return false;
		}

		public bool FallbackTraverse(SilhouetteController controller) { // used when no nav nodes are found
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
