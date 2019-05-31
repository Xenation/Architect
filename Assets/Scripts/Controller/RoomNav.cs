using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class RoomNav {

		private class NodeFCost : IComparer<NavNode> {
			public int Compare(NavNode x, NavNode y) {
				return (int) ((x.fCost - y.fCost) * 100000f);
			}
		}

		public int nodeCount {
			get {
				return nodes.Count;
			}
		}

		private Room room;
		private RoomNetwork roomnet;
		private Transform navRoot;
		private List<NavNode> nodes = new List<NavNode>();

		public RoomNav(Room room) {
			this.room = room;
			roomnet = room.GetComponentInParent<RoomNetwork>();
			navRoot = room.transform.Find("Nav");
		}

		public void BuildGraph() {
			// Fetch node list
			navRoot.GetComponentsInChildren(nodes);

			// Make sure manual links are both ways and compute relativePos
			foreach (NavNode node in nodes) {
				foreach (NavNode otherNode in node.links) {
					if (!otherNode.links.Contains(node)) {
						otherNode.links.Add(node);
					}
				}
			}

			// Generate auto links
			foreach (NavNode node in nodes) {
				if (node.linkRadius == 0f) continue;
				float sqrRadius = node.linkRadius * node.linkRadius;
				foreach (NavNode otherNode in nodes) {
					if (otherNode == node) continue;
					if ((node.transform.position - otherNode.transform.position).sqrMagnitude < sqrRadius) {
						node.Link(otherNode);
					}
				}
			}
		}

		public NavNode ClosestNode(Vector3 relpos) {
			Vector3 worldPos = roomnet.RelativeToWorldPos(relpos);
			NavNode closestNode = null;
			float closestDistanceSqr = float.PositiveInfinity;
			foreach (NavNode node in nodes) {
				float distanceSqr = (node.transform.position - worldPos).sqrMagnitude;
				if (distanceSqr < closestDistanceSqr) {
					closestDistanceSqr = distanceSqr;
					closestNode = node;
				}
			}
			return closestNode;
		}

		public void FindPath(Vector3 start, Vector3 target, List<NavNode> path) {
			//Debug.Log("NavNode Pathfind (Node Count: " + nodes.Count + ")");
			NavNode startNode = ClosestNode(start);
			NavNode targetNode = ClosestNode(target);

			// Reset Node graph values
			foreach (NavNode node in nodes) {
				node.ResetCosts();
			}

			// Pathfinding
			SortedSet<NavNode> openSet = new SortedSet<NavNode>(new NodeFCost());
			HashSet<NavNode> closedSet = new HashSet<NavNode>();
			startNode.hCost = (targetNode.transform.position - startNode.transform.position).magnitude;
			openSet.Add(startNode);

			while (openSet.Count > 0) {
				NavNode current = openSet.Min;

				openSet.Remove(current);
				closedSet.Add(current);

				if (current == targetNode) {
					Retrace(startNode, current, path);
				}

				foreach (NavNode neighbor in current.links) {
					if (closedSet.Contains(neighbor)) {
						continue;
					}

					float nCostToNeighbor = current.gCost + (neighbor.transform.position - current.transform.position).magnitude;
					if (nCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
						neighbor.gCost = nCostToNeighbor;
						neighbor.hCost = (targetNode.transform.position - neighbor.transform.position).magnitude;
						neighbor.parent = current;

						if (!openSet.Contains(neighbor)) {
							openSet.Add(neighbor);
						}
					}
				}
			}
		}

		private List<NavNode> Retrace(NavNode start, NavNode end, List<NavNode> path) {
			VisualDebug.ClearLines();
			VisualDebug.ClearSpheres();
			path.Clear();
			NavNode current = end;
			path.Add(current);

			while (current != start) {
				path.Add(current.parent);
				VisualDebug.DrawLine(current.transform.position, current.parent.transform.position, Color.blue);
				VisualDebug.DrawWireSphere(current.transform.position, 0.01f, Color.blue);
				current = current.parent;
			}

			path.Reverse();

			return path;
		}

	}
}
