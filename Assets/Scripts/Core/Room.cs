﻿using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	[RequireComponent(typeof(SnapGrid))]
	public class Room : MonoBehaviour {

		[System.NonSerialized] public SnapGrid grid;
		private List<RoomLink> links = new List<RoomLink>();

		private void Awake() {
			grid = GetComponent<SnapGrid>();
		}

		public void RegisterLink(RoomLink link) {
			links.Add(link);
		}

	}
}
