﻿using System.Collections;
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
		private Dictionary<RoomLink, LightLine> linkLights = new Dictionary<RoomLink, LightLine>();
		

		public Vector3 center {
			get {
				return (centerTransf != null) ? centerTransf.position : transform.position;
			}
		}

		public Transform centerTransform {
			get {
				return (centerTransf != null) ? centerTransf : transform;
			}
		}

		private GameObject togglable;
		private Transform centerTransf;
		private Transform insideTransf;
		private Collider[] insideColliders;

		private void Awake() {
			traverser = new RoomTraverser(this, GetComponentInParent<RoomNetwork>());
			grid = GetComponent<SnapGrid>();
			togglable = transform.Find("Togglable")?.gameObject;
			togglable?.SetActive(false);
			centerTransf = transform.Find("Center");
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
			if (lightNode == null) BuildLightNode();
			links.Add(link);
			LightLine line = LightLine.BuildNew(transform, link.gameObject.name, lightNode, link.GetLightPoint(this));
			linkLights.Add(link, line);
			link.lightLines.Add(line);
		}

		public void UnregisterLink(RoomLink link) {
			links.Remove(link);
			LightLine line;
			if (linkLights.TryGetValue(link, out line)) {
				linkLights.Remove(link);
				link.lightLines.Remove(line);
				LightLine.DestroyLine(line);
			}
		}

		public void UpdateConnected() {
			if (isConnectedToStart && !togglable.activeInHierarchy) {
				togglable?.SetActive(true);
				// TODO trigger Lumiere s'allume
			} else if (!isConnectedToStart && togglable.activeInHierarchy) {
				togglable?.SetActive(false);
				// TODO trigger Lumiere eteinte
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
