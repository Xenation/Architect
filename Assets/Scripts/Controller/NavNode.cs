using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class NavNode : MonoBehaviour {

		public float linkRadius = 0.05f;
		public List<NavNode> links = new List<NavNode>();
		
		[System.NonSerialized] public float gCost = 0f;
		[System.NonSerialized] public float hCost = 0f;
		[System.NonSerialized] public NavNode parent;

		public float fCost {
			get {
				return gCost + hCost;
			}
		}

		public void Link(NavNode node) {
			if (!links.Contains(node)) {
				links.Add(node);
				node.links.Add(this);
			}
		}

		public void Unlink(NavNode node) {
			if (links.Contains(node)) {
				links.Remove(node);
				node.links.Remove(this);
			}
		}

		public void ResetCosts() {
			gCost = 0f;
			hCost = 0f;
			parent = null;
		}

		private void OnDrawGizmosSelected() {
			Color tmpCol = Gizmos.color;
			Gizmos.color = Color.white;

			Gizmos.DrawWireSphere(transform.position, linkRadius);
			Gizmos.color = Color.cyan;
			foreach (NavNode node in links) {
				Gizmos.DrawLine(transform.position, node.transform.position);
			}

			Gizmos.color = tmpCol;
		}

	}
}
