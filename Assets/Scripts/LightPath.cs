using UnityEngine;

namespace Architect {
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class LightPath : MonoBehaviour {

		public float width = 0.01f;

		private float _progress = 0f;
		public float progress {
			get {
				return _progress;
			}
			set {
				_progress = value;
				material.SetFloat(progressID, _progress);
			}
		}

		public Transform start;
		public Transform end;

		private Material material;
		private int progressID;

		private MeshFilter filter;
		private MeshRenderer meshRenderer;

		private void Awake() {
			filter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();

			material = new Material(meshRenderer.material);
			progressID = Shader.PropertyToID("_Progress");
			GenerateMesh();
		}

		private void Update() {
			GenerateMesh(); // TODO temporary, for testing
		}

		public void GenerateMesh() {
			Vector3 localStart = transform.InverseTransformPoint(start.position);
			Vector3 localEnd = transform.InverseTransformPoint(end.position);
			Vector3 startToEnd = localEnd - localStart;
			Vector3 right = Vector3.Cross(startToEnd.normalized, Vector3.up);
			float length = startToEnd.magnitude;
			Mesh mesh = new Mesh();
			filter.mesh = mesh;

			Vector3[] vertices = new Vector3[4];
			vertices[0] = localStart - right * width;
			vertices[1] = localEnd - right * width;
			vertices[2] = localEnd + right * width;
			vertices[3] = localStart + right * width;
			Vector2[] uvs = new Vector2[4];
			uvs[0] = new Vector2(0, 0);
			uvs[1] = new Vector2(0, length / width);
			uvs[2] = new Vector2(1, length / width);
			uvs[3] = new Vector2(1, 0);
			int[] indices = new int[6];
			indices[0] = 0;
			indices[1] = 2;
			indices[2] = 1;
			indices[3] = 0;
			indices[4] = 3;
			indices[5] = 2;

			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		}

	}
}
