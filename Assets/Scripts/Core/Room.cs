﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		[System.NonSerialized] public SnapGrid grid;
		[System.NonSerialized] public bool isConnectedToStart = false;

		[System.NonSerialized] public List<RoomLink> links = new List<RoomLink>();

		private GameObject togglable;

		private void Awake() {
			grid = GetComponent<SnapGrid>();
			togglable = transform.Find("Togglable")?.gameObject;
			togglable?.SetActive(false);
		}

		public void RegisterLink(RoomLink link) {
			links.Add(link);
		}

		public void UnregisterLink(RoomLink link) {
			links.Remove(link);
		}

		public void UpdateConnected() {
			if (togglable == null) return;
			if (isConnectedToStart && !togglable.activeInHierarchy) {
				togglable.SetActive(true);
			} else if (!isConnectedToStart && togglable.activeInHierarchy) {
				togglable.SetActive(false);
			}
		}

		IEnumerator<Room> IEnumerable<Room>.GetEnumerator() {
			return new NeighboorsEnumerator(this, links);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new NeighboorsEnumerator(this, links);
		}

	}
}
