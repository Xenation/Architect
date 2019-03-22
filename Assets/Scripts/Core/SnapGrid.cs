﻿using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class SnapGrid : MonoBehaviour {

		public float snap = 0.1f;
		public List<Recti> boundsTest = new List<Recti>();

		private Matrix4x4 wtl;
		private Matrix4x4 ltw;

		private List<Recti> boundingRects = new List<Recti>();

		private MeshFilter filter;
		private Mesh mesh;

		private float gridHeight { get { return transform.position.y; } }

		private void Awake() {
			filter = GetComponent<MeshFilter>();
			mesh = filter.mesh = new Mesh();
			RecalculateMatrices();
			foreach (Recti rect in boundsTest) {
				boundingRects.Add(rect);
			}
			RecreateMesh();
		}

		private void Update() {
			RecalculateMatrices();
		}

		private void RecalculateMatrices() {
			ltw = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			wtl = ltw.inverse;
		}

		private void RecreateMesh() {
			mesh.Clear();

			Vector3[] verts = new Vector3[boundingRects.Count * 4];
			int[] indices = new int[boundingRects.Count * 6];
			for (int i = 0; i < boundingRects.Count; i++) {
				Vector3 min = boundingRects[i].min.Unflat(0).Float() * snap;
				Vector3 max = boundingRects[i].max.Unflat(0).Float() * snap;
				verts[i * 4] = new Vector3(min.x, min.y, min.z);
				verts[i * 4 + 1] = new Vector3(min.x, min.y, max.z);
				verts[i * 4 + 2] = new Vector3(max.x, min.y, max.z);
				verts[i * 4 + 3] = new Vector3(max.x, min.y, min.z);
				indices[i * 6] = i * 4;
				indices[i * 6 + 1] = i * 4 + 1;
				indices[i * 6 + 2] = i * 4 + 2;
				indices[i * 6 + 3] = i * 4;
				indices[i * 6 + 4] = i * 4 + 2;
				indices[i * 6 + 5] = i * 4 + 3;
			}

			mesh.vertices = verts;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		}

		public void AddBoundingRect(Recti rect) {
			boundingRects.Add(rect);
			RecreateMesh();
		}

		public Vector3 SnapPosition(Vector3 pos) {
			Vector2Int gridPos = WorldToGrid(pos);
			if (!IsInGrid(gridPos)) {
				gridPos = ProjectInGrid(gridPos);
			}
			pos = GridToWorld(gridPos);
			return pos;
		}

		public Quaternion SnapRotation(Quaternion rot) {
			Vector3 euler = rot.eulerAngles;
			euler.y -= transform.rotation.eulerAngles.y;
			euler.y = Mathf.Round(euler.y / 90f) * 90f;
			euler.y += transform.rotation.eulerAngles.y;
			return Quaternion.Euler(euler);
		}

		public void Snap(Transform transf, Vector2Int size) {
			transf.rotation = SnapRotation(transf.rotation);
			Vector3 euler = transf.rotation.eulerAngles;
			euler.y -= transform.rotation.eulerAngles.y;
			if (Mathf.RoundToInt(euler.y) == 90 || Mathf.RoundToInt(euler.y) == 270) { // TODO kinda ugly check
				int tmp = size.x;
				size.x = size.y;
				size.y = tmp;
			}
			euler.y += transform.rotation.eulerAngles.y;

			Recti gridRect = WorldToGrid(transf.position, size);
			if (!IsInGrid(gridRect)) {
				gridRect = ProjectInGrid(gridRect); // TODO not perfect when rect is part in one rect part in another
			}
			//Debug.DrawRay(GridToWorld(gridRect.min), Vector3.up, Color.white);
			//Debug.DrawRay(GridToWorld(gridRect.max), Vector3.up, Color.white);
			transf.position = GridToWorld(gridRect);
		}

		public bool IsInGrid(Vector2Int gridPos) {
			foreach (Recti rect in boundingRects) {
				if (rect.Contains(gridPos)) {
					return true;
				}
			}
			return false;
		}

		public bool IsInGrid(Recti gridRect) {
			return IsInGrid(gridRect.min) && IsInGrid(new Vector2Int(gridRect.min.x, gridRect.max.y)) && IsInGrid(gridRect.max) && IsInGrid(new Vector2Int(gridRect.max.x, gridRect.min.y));
		}

		public Vector2Int ProjectInGrid(Vector2Int gridPos) {
			if (boundingRects.Count == 0) return gridPos;
			Vector2Int closest = gridPos.Clamp(boundingRects[0]);

			for (int i = 1; i < boundingRects.Count; i++) {
				Vector2Int currentClosest = gridPos.Clamp(boundingRects[i]);
				if ((gridPos - closest).sqrMagnitude > (gridPos - currentClosest).sqrMagnitude) {
					closest = currentClosest;
				}
			}

			return closest;
		}

		public Recti ProjectInGrid(Recti rect) {
			if (boundingRects.Count == 0) return rect;
			Recti closest = rect.Constrain(boundingRects[0]);
			float distClosest = Vector2.Distance(rect.center, closest.center);

			for (int i = 1; i < boundingRects.Count; i++) {
				Recti currentClosest = rect.Constrain(boundingRects[i]);
				float distCurrent = Vector2.Distance(rect.center, currentClosest.center);
				if (distCurrent < distClosest) {
					closest = currentClosest;
				}
			}

			return closest;
		}

		public Vector2Int WorldToGrid(Vector3 worldPos) {
			return (wtl.MultiplyPoint3x4(worldPos) / snap).Flat().RoundToInt();
		}

		public Vector3 GridToWorld(Vector2Int gridPos) {
			return ltw.MultiplyPoint3x4((gridPos.Float() * snap).Unflat(0));
		}

		public Vector3 GridToWorld(Vector2 gridPos) {
			return ltw.MultiplyPoint3x4((gridPos * snap).Unflat(0));
		}

		public Recti WorldToGrid(Vector3 center, Vector2Int size) {
			Vector2Int gridCenter = WorldToGrid(center);
			//Debug.DrawRay(GridToWorld(gridCenter), Vector3.up, Color.yellow);
			Vector2Int gridCenterFloored = (wtl.MultiplyPoint3x4(center) / snap).Flat().FloorToInt();
			//Debug.DrawRay(GridToWorld(gridCenterFloored), Vector3.up, Color.red);
			Recti gridRect = new Recti();
			if (size.x % 2 != 0) { // Odd
				gridRect.min.x = gridCenterFloored.x - size.x / 2;
				gridRect.max.x = gridCenterFloored.x + size.x / 2 + 1;
			} else { // Even
				gridRect.min.x = gridCenter.x - size.x / 2;
				gridRect.max.x = gridCenter.x + size.x / 2;
			}
			if (size.y % 2 != 0) { // Odd
				gridRect.min.y = gridCenterFloored.y - size.y / 2;
				gridRect.max.y = gridCenterFloored.y + size.y / 2 + 1;
			} else { // Even
				gridRect.min.y = gridCenter.y - size.y / 2;
				gridRect.max.y = gridCenter.y + size.y / 2;
			}
			//Debug.DrawRay(GridToWorld(gridRect.min), Vector3.up, Color.green);
			//Debug.DrawRay(GridToWorld(gridRect.max), Vector3.up, Color.cyan);
			return gridRect;
		}

		public Vector3 GridToWorld(Recti gridRect) {
			int width = gridRect.max.x - gridRect.min.x;
			int height = gridRect.max.y - gridRect.min.y;
			return GridToWorld(gridRect.min.Float() + new Vector2(width / 2f, height / 2f));
		}

	}
}
