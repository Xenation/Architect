using System.Collections.Generic;
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
				Debug.Log("recti min=" + boundingRects[i].min + "    max=" + boundingRects[i].max);
				Vector3 min = boundingRects[i].min.Unflat(0).Float() * snap;
				Vector3 max = boundingRects[i].max.Unflat(0).Float() * snap;
				Debug.Log("rect min=" + min + "    max=" + max);
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
			//pos = wtl.MultiplyPoint3x4(pos);
			//pos.y = 0;
			//Vector2Int gridPos = (pos / snap).Flat().RoundToInt();
			Vector2Int gridPos = WorldToGrid(pos);
			if (!IsInGrid(gridPos)) {
				gridPos = ProjectInGrid(gridPos);
			}
			pos = GridToWorld(gridPos);
			//pos = (gridPos.Float() * snap).Unflat(0);
			//pos = ltw.MultiplyPoint3x4(pos);
			return pos;
		}

		public Quaternion SnapRotation(Quaternion rot) {
			Vector3 euler = rot.eulerAngles;
			euler.y -= transform.rotation.eulerAngles.y;
			euler.y = Mathf.Round(euler.y / 90f) * 90f;
			euler.y += transform.rotation.eulerAngles.y;
			return Quaternion.Euler(euler);
		}

		public bool IsInGrid(Vector2Int gridPos) {
			foreach (Recti rect in boundingRects) {
				if (rect.Contains(gridPos)) {
					return true;
				}
			}
			return false;
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

		public Vector2Int WorldToGrid(Vector3 worldPos) {
			return (wtl.MultiplyPoint3x4(worldPos) / snap).Flat().RoundToInt();
		}

		public Vector3 GridToWorld(Vector2Int gridPos) {
			return ltw.MultiplyPoint3x4((gridPos.Float() * snap).Unflat(0));
		}

	}
}
