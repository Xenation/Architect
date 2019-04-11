using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	[RequireComponent(typeof(SnapGrid))]
	public class Room : MonoBehaviour, IEnumerable<Room> {

		public class NeighboorsEnumerator : IEnumerator<Room> {
			private Room room;
			private List<RoomLink> links;
			private int currentIndex = 0;

			public Room Current {
				get {
					return links[currentIndex].GetOther(room);
				}
			}

			object IEnumerator.Current { get { return links[currentIndex].GetOther(room); } }

			public NeighboorsEnumerator(Room r, List<RoomLink> lnks) {
				room = r;
				currentIndex = 0;
				links = lnks;
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

		public GameObject whenConnected;
		[System.NonSerialized] public SnapGrid grid;
		[System.NonSerialized] public bool isConnectedToStart = false;

		[System.NonSerialized] public List<RoomLink> links = new List<RoomLink>();

		private void Awake() {
			grid = GetComponent<SnapGrid>();
		}

		public void RegisterLink(RoomLink link) {
			links.Add(link);
		}

		public void UpdateConnected() {
			if (whenConnected == null) return;
			if (isConnectedToStart && !whenConnected.activeInHierarchy) {
				whenConnected.SetActive(true);
			} else if (!isConnectedToStart && whenConnected.activeInHierarchy) {
				whenConnected.SetActive(false);
			}
		}

		public IEnumerator<Room> GetEnumerator() {
			return new NeighboorsEnumerator(this, links);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new NeighboorsEnumerator(this, links);
		}

	}

	
}
