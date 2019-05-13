using UnityEngine;

namespace Architect {
	public static class Utils {

		public static GameObject DuplicateVisual(this GameObject gameObject) {
			return gameObject.DuplicateVisual(gameObject.transform.parent);
		}

		public static GameObject DuplicateVisual(this GameObject gameObject, Transform nParent) {
			// GameObject duplication
			GameObject duplicate = new GameObject(gameObject.name);
			duplicate.transform.SetParent(nParent);
			duplicate.transform.localPosition = gameObject.transform.localPosition;
			duplicate.transform.localScale = gameObject.transform.localScale;
			duplicate.transform.localRotation = gameObject.transform.localRotation;

			// Components duplication
			foreach (Component comp in gameObject.GetComponents<Component>()) {
				MeshRenderer meshRenderer = comp as MeshRenderer;
				if (meshRenderer != null) {
					MeshRenderer dupMeshRenderer = duplicate.AddComponent<MeshRenderer>();
					dupMeshRenderer.materials = meshRenderer.materials;
				}
				MeshFilter meshFilter = comp as MeshFilter;
				if (meshFilter != null) {
					MeshFilter dupMeshFilter = duplicate.AddComponent<MeshFilter>();
					dupMeshFilter.sharedMesh = meshFilter.sharedMesh;
				}
			}

			// Child duplication
			foreach (Transform child in gameObject.transform) {
				child.gameObject.DuplicateVisual(duplicate.transform);
			}
			return duplicate;
		}

		public static GameObject CreateMeshObject(string name, Transform parent, out MeshRenderer renderer, out MeshFilter filter, out Mesh mesh) {
			GameObject go = new GameObject(name);
			go.transform.SetParent(parent, false);
			filter = go.AddComponent<MeshFilter>();
			mesh = filter.mesh;
			renderer = go.AddComponent<MeshRenderer>();
			return go;
		}

		public static GameObject CreateMeshObject(string name, Transform parent, out MeshRenderer renderer, out Mesh mesh) {
			GameObject go = new GameObject(name);
			go.transform.SetParent(parent, false);
			MeshFilter filter = go.AddComponent<MeshFilter>();
			mesh = filter.mesh;
			renderer = go.AddComponent<MeshRenderer>();
			return go;
		}

		public static GameObject CreateMeshObject(string name, Transform parent, out Mesh mesh) {
			GameObject go = new GameObject(name);
			go.transform.SetParent(parent, false);
			MeshFilter filter = go.AddComponent<MeshFilter>();
			mesh = filter.mesh;
			go.AddComponent<MeshRenderer>();
			return go;
		}

		public static void CreateQuad(this Mesh mesh, Vector2 size, Vector3 up, Vector3 right, bool centered = true) {
			mesh.Clear();

			Vector2 halfSize = size / 2f;
			Vector3 minCorner = (centered) ? -up * halfSize.y + -right * halfSize.x : Vector3.zero;

			Vector3[] verts = new Vector3[4];
			Vector2[] uv = new Vector2[4];
			int[] indices = new int[6];
			verts[0] = minCorner;
			verts[1] = minCorner + up * size.y;
			verts[2] = minCorner + up * size.y + right * size.x;
			verts[3] = minCorner + right * size.x;
			uv[0] = new Vector2(0, 0);
			uv[1] = new Vector2(0, 1);
			uv[2] = new Vector2(1, 1);
			uv[3] = new Vector2(1, 0);
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			indices[3] = 0;
			indices[4] = 2;
			indices[5] = 3;

			mesh.vertices = verts;
			mesh.uv = uv;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
			mesh.RecalculateBounds();
		}

		public static bool IsInside(this Collider collider, Vector3 pos) {
			Vector3 closest = collider.ClosestPoint(pos);
			Vector3 center = collider.bounds.center;

			Vector3 centerToContact = (closest - center).normalized;
			Vector3 centerToPos = (closest - pos).normalized;

			return Vector3.Dot(centerToContact, centerToPos) > 0;
		}

	}
}
