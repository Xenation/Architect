using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class RoomNetwork : MonoBehaviour {

		private class RoomNode {
			public Room room;
			public int parentLinkIndex;
			public RoomNode parentNode;
			public float gCost;
			public float hCost;

			public float fCost {
				get {
					return gCost + hCost;
				}
			}

			public RoomNode(Room room) {
				this.room = room;
				Reset();
			}

			public void Reset() {
				gCost = 0f;
				hCost = 0f;
				parentLinkIndex = 0;
				parentNode = null;
			}

			public override int GetHashCode() {
				return room.gameObject.GetInstanceID();
			}
		}

		private class NodeFCost : IComparer<RoomNode> {
			public int Compare(RoomNode x, RoomNode y) {
				return (int) ((x.fCost - y.fCost) * 10000f);
			}
		}
		private class NodeGCost : IComparer<RoomNode> {
			public int Compare(RoomNode x, RoomNode y) {
				return (int) (x.gCost - y.gCost);
			}
		}

		public Room startingRoom;
		public Room[] endingRooms;
		[System.NonSerialized] public Room lastLitRoom = null;
		public delegate void LastLitChanged(Room lastLit);
		public event LastLitChanged LastLitChangedEvent;

		private List<Room> rooms = new List<Room>();
		private List<SnapPoint> points = new List<SnapPoint>();

		private Dictionary<Room, RoomNode> roomGraph = new Dictionary<Room, RoomNode>();

		private Transform respawnPoint;
		public Vector3 moduleRespawnPosition {
			get {
				return respawnPoint.position;
			}
		}

		public float moduleZoneRadius = 2;
		public float moduleZoneFloor = 0.1f;
		public float moduleZoneCeilling = 4f;

		[System.NonSerialized] public Room fallbackRoom = null;

		[Header("Light Path")]
		public bool useLightPaths = true;
		public Material pathMaterial;
		public float pathWidth = 0.01f;
		public GameObject normalNodePrefab;
		public GameObject fallbackNodePrefab;

		[Header("Debug")]
		public float tableMoveSpeed = .02f;

		private void Awake() {
			SettingsManager.I.activeRoomnet = this;

			DevKeys.CreateInstance(); // Make sure an instance of devkeys exists

			BuildNetwork();
			BuildGraph();

			respawnPoint = transform.Find("ModulesRespawn");
		}

		private void BuildNetwork() {
			startingRoom.isConnectedToStart = true;
			startingRoom.isFallback = true;
			fallbackRoom = startingRoom;
			// Initialize Links and Rooms
			foreach (Transform child in transform) {
				RoomLink link = child.GetComponent<RoomLink>();
				if (link != null) {
					SnapPoint snapPoint = link.transform.GetComponentInChildren<SnapPoint>();
					if (snapPoint != null && snapPoint.type == SnapPointType.Door) {
						link.traverser = new DoorTraverser(link, this, snapPoint);
					} else {
						link.traverser = new DefaultLinkTraverser(link, this);
					}
					link.ApplyLink();
				}
				Room room = child.GetComponent<Room>();
				if (room != null) {
					rooms.Add(room);
				}
			}
			GetComponentsInChildren(points);
			startingRoom.UpdateConnected();
		}

		private void BuildGraph() {
			foreach (Room room in rooms) {
				roomGraph.Add(room, new RoomNode(room));
			}
		}

		private void OnDrawGizmos() {
			if (rooms == null) return;
			Color tmpCol = Gizmos.color;

			HashSet<RoomLink> exploredLinks = new HashSet<RoomLink>();

			foreach (Room room in rooms) {
				if (room.isFallback) {
					Gizmos.color = Color.cyan;
					if (room == fallbackRoom) {
						Gizmos.color = Color.blue;
					}
				} else {
					Gizmos.color = Color.yellow;
				}
				Gizmos.DrawWireSphere(room.center, .05f);

				foreach (RoomLink link in room.links) {
					if (exploredLinks.Contains(link)) continue;
					Gizmos.color = (link.isOpen) ? Color.green : Color.red;
					Gizmos.DrawLine(link.room1.center, RelativeToWorldPos(link.entry1));
					Gizmos.DrawLine(RelativeToWorldPos(link.entry1), RelativeToWorldPos(link.entry2));
					Gizmos.DrawLine(RelativeToWorldPos(link.entry2), link.room2.center);
					exploredLinks.Add(link);
				}
			}

			Gizmos.color = tmpCol;
		}

		private void Update() {
			UpdateRoomConnections();
			if (useLightPaths) {
				startingRoom.lightNode.UpdateSignal(null, Time.deltaTime);
				startingRoom.lightNode.ClearUpdateFlag(null);
			}
		}

		public Room GetRoom(Vector3 pos) {
			foreach (Room room in rooms) {
				if (room.isInside(pos)) {
					return room;
				}
			}
			return null;
		}

		public Room GetRoomGridHover(Vector3 pos) {
			foreach (Room room in rooms) {
				if (room.grid.IsOverGrid(pos, SettingsManager.I.roomSettings.gridSnapOverHeight, SettingsManager.I.roomSettings.gridSnapUnderHeight)) {
					return room;
				}
			}
			return null;
		}

		public SnapPoint GetPointHover(Vector3 pos) {
			foreach (SnapPoint point in points) {
				if (Vector3.Distance(pos, point.center) < SettingsManager.I.roomSettings.linkSnapDistance) {
					return point;
				}
			}
			return null;
		}

		public Vector3 WorldToRelativePos(Vector3 pos) {
			return transform.InverseTransformPoint(pos);
		}

		public Vector3 WorldToRelativeVec(Vector3 vec ) {
			return transform.InverseTransformVector(vec);
		}

		public Vector3 WorldToRelativeDir(Vector3 dir) {
			return transform.InverseTransformDirection(dir);
		}

		public Vector3 RelativeToWorldPos(Vector3 pos) {
			return transform.TransformPoint(pos);
		}

		public Vector3 RelativeToWorldVec(Vector3 vec) {
			return transform.TransformVector(vec);
		}

		public Vector3 RelativeToWorldDir(Vector3 dir) {
			return transform.TransformDirection(dir);
		}

		public void OnLinkChange(RoomLink link) {
			//UpdateRoomConnections();
		}

		public void OnLinkOpenned(RoomLink link) {
			
		}

		public void OnLinkClosed(RoomLink link) {

		}

		public void UpdateRoomConnections() {
			// Finding previously lit rooms
			HashSet<Room> previouslyLit = new HashSet<Room>();
			foreach (Room room in rooms) {
				room.linkCountToStart = 2000; // Reseting link distance
				if (room.isConnectedToStart) {
					previouslyLit.Add(room);
				}
			}
			startingRoom.linkCountToStart = 0;

			if (useLightPaths) {
				// Settings all links to unlit
				foreach (Room room in rooms) {
					foreach (RoomLink link in room.links) {
						link.MarkUnlit();
					}
				}
			}

			// Exploring rooms that are linked to start
			List<Room> toExplore = new List<Room>();
			HashSet<Room> explored = new HashSet<Room>();
			toExplore.Add(startingRoom);

			while (toExplore.Count != 0) {
				Room current = toExplore[0];
				toExplore.RemoveAt(0);
				explored.Add(current);

				foreach (RoomLink link in current.links) {
					if (!link.isOpen) continue;
					if (useLightPaths) link.MarkLit();
					Room neighbor = link.GetOther(current);
					int potentialLinkCount = current.linkCountToStart + 1;
					if (neighbor.linkCountToStart > potentialLinkCount) {
						neighbor.linkCountToStart = potentialLinkCount;
					}
					if (!explored.Contains(neighbor) && !toExplore.Contains(neighbor)) {
						toExplore.Add(neighbor);
					}
				}
			}

			// Updating connected state
			foreach (Room room in rooms) { // Set all to disconnected
				room.isConnectedToStart = false;
			}
			foreach (Room room in explored) { // Set connected room to connected
				room.isConnectedToStart = true;
				Debug.DrawLine(room.transform.position, room.transform.position + Vector3.up * 0.1f);
			}
			foreach (Room room in rooms) { // Update connect state
				room.UpdateConnected();
			}

			// Finding last lit room(s)
			explored.ExceptWith(previouslyLit);
			Room furthest = null;
			foreach (Room nLitRoom in explored) {
				if (furthest == null || furthest.linkCountToStart < nLitRoom.linkCountToStart) {
					furthest = nLitRoom;
				}
			}
			if (furthest != null) {
				LastLitChangedEvent?.Invoke(furthest);
				lastLitRoom = furthest;
				// Find the furthest previously lit fallback room
				Room currentFallback = null;
				foreach (Room room in rooms) {
					if (!room.isConnectedToStart || !room.isFallback || explored.Contains(room) || (currentFallback != null && currentFallback.linkCountToStart > room.linkCountToStart)) continue;
					currentFallback = room;
				}
				if (currentFallback != null) {
					fallbackRoom = currentFallback;
				}
			}
		}

		public List<RoomLink> FindPath(Room start, Room target) {
			// Reset Node graph values
			foreach (RoomNode node in roomGraph.Values) {
				node.Reset();
			}

			// Pathfinding
			SortedSet<RoomNode> openSet = new SortedSet<RoomNode>(new NodeFCost());
			HashSet<RoomNode> closedSet = new HashSet<RoomNode>();
			RoomNode startNode = roomGraph[start];
			startNode.hCost = (target.center - start.center).magnitude;
			RoomNode targetNode = roomGraph[target];
			openSet.Add(startNode);

			while (openSet.Count > 0) {
				RoomNode current = openSet.Min; // TODO Check that Min is trully the lowest fCost

				openSet.Remove(current);
				closedSet.Add(current);

				if (current.room == target) {
					return Retrace(startNode, current);
				}

				foreach (RoomLink link in current.room.links) {
					Room neighbor = link.GetOther(current.room);
					RoomNode neighborNode = roomGraph[neighbor];
					if (!link.isOpen || closedSet.Contains(neighborNode)) {
						continue;
					}

					float nCostToNeighbor = current.gCost + (neighbor.center - current.room.center).magnitude;
					if (nCostToNeighbor < neighborNode.gCost || !openSet.Contains(neighborNode)) {
						neighborNode.gCost = nCostToNeighbor;
						neighborNode.hCost = (target.center - neighbor.center).magnitude;
						neighborNode.parentLinkIndex = neighbor.links.IndexOf(link);
						neighborNode.parentNode = current;

						if (!openSet.Contains(neighborNode)) {
							openSet.Add(neighborNode);
						}
					}
				}
			}

			return null;
		}

		private List<RoomLink> Retrace(RoomNode start, RoomNode end) {
			List<RoomLink> path = new List<RoomLink>();
			RoomNode current = end;

			while (current != start) {
				RoomLink link = current.room.links[current.parentLinkIndex];
				path.Add(link);
				current = current.parentNode;
				Debug.DrawLine(link.room1.transform.position, link.room2.transform.position, Color.blue);
			}

			path.Reverse();

			return path;
		}

		public bool IsInModuleZone(Vector3 pos) {
			Vector2 toPos = new Vector2(pos.x - transform.position.x, pos.z - transform.position.z);

			return toPos.magnitude < moduleZoneRadius && pos.y > moduleZoneFloor && pos.y < moduleZoneCeilling;
		}

		public Room GetLinkedEndRoom() {
			foreach (Room room in endingRooms) {
				if (room.isConnectedToStart) {
					return room;
				}
			}
			return null;
		}

		// DEBUG
		public void RaiseTable() {
			transform.parent.position += Vector3.up * tableMoveSpeed * Time.deltaTime;
			ModelRotator rotator = FindObjectOfType<ModelRotator>();
			if (rotator != null && rotator.rigidbody != null) {
				rotator.rigidbody.MovePosition(transform.parent.position);
			}
		}

		public void LowerTable() {
			transform.parent.position += Vector3.down * tableMoveSpeed * Time.deltaTime;
			ModelRotator rotator = FindObjectOfType<ModelRotator>();
			if (rotator != null && rotator.rigidbody != null) {
				rotator.rigidbody.MovePosition(transform.parent.position);
			}
		}

	}
}
