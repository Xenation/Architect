using System.Collections.Generic;
using UnityEngine;
using Architect.LightPaths;

namespace Architect {
	public class RoomLightPath {
		
		private Room room;
		private RoomNetwork roomnet;
		private Dictionary<RoomLink, LightLine> lines = new Dictionary<RoomLink, LightLine>();
		private LightNode lightNode;

		private Transform root;
		private RoomSettings roomSettings;

		public RoomLightPath(Room room, RoomNetwork roomnet) {
			this.room = room;
			this.roomnet = roomnet;
			roomSettings = SettingsManager.I.roomSettings;
		}

		public void BuildAll() {
			root = new GameObject("LightPathRoot").transform;
			root.SetParent(room.transform, false);

			// Create Node
			if (room.isFallback) {
				lightNode = Object.Instantiate(roomSettings.fallbackNodePrefab, root).GetComponent<LightNode>();
			} else {
				lightNode = Object.Instantiate(roomSettings.normalNodePrefab, root).GetComponent<LightNode>();
			}
		}

		public void BuildLinkPath(RoomLink link) {
			GameObject pathObj = new GameObject("Path-" + link.gameObject.name, typeof(MeshFilter), typeof(MeshRenderer));
			pathObj.transform.SetParent(root, false);
			MeshRenderer renderer = pathObj.GetComponent<MeshRenderer>();
			renderer.sharedMaterial = roomSettings.pathMaterial;
			LightPath lightPath = pathObj.AddComponent<LightPath>();
			Transform start = new GameObject("LinkSide").transform;
			start.SetParent(pathObj.transform, false);
			Vector3 linkWorldPos = roomnet.RelativeToWorldPos(link.GetEntry(room));
			start.position = new Vector3(linkWorldPos.x, room.center.y, linkWorldPos.z);
			lightPath.start = start;
			lightPath.end = room.centerTransform;
			lightPath.HasReachedTarget += PathReachedTarget;
			lines.Add(link, LightLine.BuildNew(root, link.gameObject.name, lightNode, ));
		}

		public void DeleteLinkPath(RoomLink link) {
			LightPath path;
			if (paths.TryGetValue(link, out path)) {
				path.HasReachedTarget -= PathReachedTarget;
				paths.Remove(link);
				Object.Destroy(path.gameObject);
			}
		}

		private void PathReachedTarget(ProgressingElement elem) {
			if (elem is LightPath) {
				lightNode.targetProgress = 1f;
			}
		}

		public void InConnect(RoomLink link) {
			LightPath path;
			if (paths.TryGetValue(link, out path)) {
				path.reverse = false;
				path.targetProgress = 1f;
			}
		}

		public void InDisconnect(RoomLink link) {
			LightPath path;
			if (paths.TryGetValue(link, out path)) {
				path.reverse = false;
				path.targetProgress = 0f;
				lightNode.targetProgress = 0f;
			}
		}

		public void OutConnect(RoomLink link) {
			LightPath path;
			if (paths.TryGetValue(link, out path)) {
				path.reverse = false;
				path.targetProgress = 1f;
			}
		}

		public void OutDisconnect(RoomLink link) {
			LightPath path;
			if (paths.TryGetValue(link, out path)) {
				path.reverse = false;
				path.targetProgress = 0f;
			}
		}

	}
}
