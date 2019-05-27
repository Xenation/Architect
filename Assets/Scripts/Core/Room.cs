using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architect.LightPaths;

namespace Architect {
	[RequireComponent(typeof(SnapGrid))]
	public class Room : MonoBehaviour, IEnumerable<Room> {

		public class NeighboorsEnumerator : IEnumerator<Room> {
			private Room room;
			private List<RoomLink> links;
			private int currentIndex = 0;

			public Room Current { get { return links[currentIndex].GetOther(room); } }
			object IEnumerator.Current { get { return links[currentIndex].GetOther(room); } }

			public NeighboorsEnumerator(Room r, List<RoomLink> lnks) {
				room = r;
				links = lnks;
				currentIndex = 0;
			}

			public void Dispose() {
				
			}

			public bool MoveNext() {
				currentIndex++;
				return currentIndex < links.Count;
			}

			public void Reset() {
				currentIndex = 0;
			}
		}

		public bool isFallback = false;

		[System.NonSerialized] public SnapGrid grid;
		[System.NonSerialized] public bool isConnectedToStart = false;
		[System.NonSerialized] public int linkCountToStart = 0;

		[System.NonSerialized] public List<RoomLink> links = new List<RoomLink>();
		public RoomTraverser traverser;
		[System.NonSerialized] public LightNode lightNode = null;
		private Dictionary<RoomLink, LightLink> linkLightLinks = new Dictionary<RoomLink, LightLink>();
		

		public Vector3 center {
			get {
				if (centerTransf == null) {
					centerTransf = transform.Find("Center");
				}
				return (centerTransf != null) ? centerTransf.position : transform.position;
			}
		}

		public Transform centerTransform {
			get {
				if (centerTransf == null) {
					centerTransf = transform.Find("Center");
				}
				return (centerTransf != null) ? centerTransf : transform;
			}
		}

		private GameObject togglable;
		private Transform centerTransf;
		private Transform insideTransf;
		private Collider[] insideColliders;
		private RoomNetwork roomnet;

		private void Awake() {
			roomnet = GetComponentInParent<RoomNetwork>();
			traverser = new RoomTraverser(this, roomnet);
			grid = GetComponent<SnapGrid>();
			togglable = transform.Find("Togglable")?.gameObject;
			togglable?.SetActive(false);
			insideTransf = transform.Find("Inside");
			insideColliders = insideTransf.GetComponentsInChildren<Collider>();
			foreach (Collider collider in insideColliders) { // Make sure every "inside collider" is trigger
				collider.isTrigger = true;
			}
		}
		
		public void BuildLightNode() {
			RoomSettings roomSettings = SettingsManager.I.roomSettings;
			// Create Node
			if (isFallback) {
				lightNode = Instantiate(roomSettings.fallbackNodePrefab, transform).GetComponent<LightNode>();
			} else {
				lightNode = Instantiate(roomSettings.normalNodePrefab, transform).GetComponent<LightNode>();
			}
			lightNode.gameObject.name = "Node-" + gameObject.name;
			lightNode.transform.position = center;
			lightNode.activated = true;
		}

		public void RegisterLink(RoomLink link) {
			links.Add(link);
			if (roomnet.useLightPaths) {
				if (lightNode == null) BuildLightNode();
				LightLink lightLink = LightLine.BuildLine(transform, link.gameObject.name, lightNode, link.GetLightPoint(this));
				linkLightLinks.Add(link, lightLink);
				link.lightLinks.Add(lightLink);
			}
		}

		public void UnregisterLink(RoomLink link) {
			links.Remove(link);
			if (roomnet.useLightPaths) {
				LightLink lightLink;
				if (linkLightLinks.TryGetValue(link, out lightLink)) {
					linkLightLinks.Remove(link);
					link.lightLinks.Remove(lightLink);
					LightLink.DestroyLink(lightLink);
				}
			}
		}

		public void UpdateConnected() {
			if (isConnectedToStart && !togglable.activeInHierarchy) {
				togglable?.SetActive(true);
				// trigger Lumiere s'allume (TODO sauf première)
				AkSoundEngine.PostEvent("Play_Lumiere_Allume", gameObject);
			} else if (!isConnectedToStart && togglable.activeInHierarchy) {
				togglable?.SetActive(false);
				// trigger Lumiere eteinte
				AkSoundEngine.PostEvent("Play_Lumiere_Eteint", gameObject);
			}
		}

		public RoomLink GetOpenLink() {
			foreach (RoomLink link in links) {
				if (link.isOpen) {
					return link;
				}
			}
			return null;
		}

		public RoomLink GetOpenLinkToConnected() {
			foreach (RoomLink link in links) {
				if (link.isOpen && link.GetOther(this).isConnectedToStart) {
					return link;
				}
			}
			return null;
		}
		
		public RoomLink GetClosestOpenLinkToConnected() {
			RoomLink closestLink = null;
			foreach (RoomLink link in links) {
				if (link.isOpen && link.GetOther(this).isConnectedToStart && (closestLink == null || closestLink.GetOther(this).linkCountToStart > link.GetOther(this).linkCountToStart)) {
					closestLink = link;
				}
			}
			return closestLink;
		}

		public bool isInside(Vector3 pos) {
			foreach (Collider insideCollider in insideColliders) {
				if (insideCollider.ClosestPoint(pos) == pos) { // TODO maybe use error margin for test
					return true;
				}
			}
			return false;
		}

		IEnumerator<Room> IEnumerable<Room>.GetEnumerator() {
			return new NeighboorsEnumerator(this, links);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new NeighboorsEnumerator(this, links);
		}

	}
}
