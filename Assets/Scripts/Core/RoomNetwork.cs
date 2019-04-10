using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class RoomNetwork : MonoBehaviour {

		private struct RoomNode {
			public Room room;
			public float gCost;
			public float fCost;

			public RoomNode(Room room, float gCost, float fCost) {
				this.room = room;
				this.gCost = gCost;
				this.fCost = fCost;
			}

			public static bool operator==(RoomNode a, RoomNode b) {
				return (a.room == b.room);
			}
			public static bool operator !=(RoomNode a, RoomNode b) {
				return (a.room != b.room);
			}
		}

		private class NodeFCost : IComparer<RoomNode> {
			public int Compare(RoomNode x, RoomNode y) {
				return (int) (x.fCost - y.fCost);
			}
		}
		private class NodeGCost : IComparer<RoomNode> {
			public int Compare(RoomNode x, RoomNode y) {
				return (int) (x.gCost - y.gCost);
			}
		}

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

		//public List<Room> FindPath(Room start, Room end) {
		//	SortedSet<RoomNode> openSet = new SortedSet<RoomNode>(new NodeFCost());
		//	HashSet<RoomNode> closedSet = new HashSet<RoomNode>();
		//	openSet.Add(new RoomNode(start, 0, (end.transform.position - start.transform.position).sqrMagnitude));
		//	Dictionary<RoomNode, RoomNode> trace = new Dictionary<RoomNode, RoomNode>();

		//	while (openSet.Count != 0) {
		//		RoomNode current = openSet.Min;
		//		if (current.room == end) {
		//			return ReconstructPath(trace, current);
		//		}

		//		openSet.Remove(current);
		//		closedSet.Add(current);

		//		foreach (Room neighbor in current.room) {
		//			RoomNode neighborNode = new RoomNode(neighbor, 0f, 0f);
		//			if (closedSet.Contains(neighborNode)) continue;

		//			neighborNode.gCost = current.gCost + (current.room.transform.position - neighbor.transform.position).sqrMagnitude;

		//			if (!openSet.Contains(neighborNode)) {
		//				openSet.Add(neighborNode);
		//			} else if () {
		//				continue;
		//			}

		//			trace.Add(neighborNode, current);
		//			neighborNode.fCost = neighborNode.gCost + (end.transform.position - neighbor.transform.position).sqrMagnitude;
		//		}
		//	}
		//}

		//private List<Room> ReconstructPath(Dictionary<RoomNode, RoomNode> trace, RoomNode current) {
		//	List<Room> path = new List<Room>();
		//	path.Add(current.room);

		//	foreach (RoomNode curr in trace.Keys) {
		//		curr = trace[curr];
		//	}
		//	return path;
		//}

	}
}
