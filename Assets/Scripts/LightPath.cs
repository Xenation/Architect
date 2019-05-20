using UnityEngine;

namespace Architect {
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class LightPath : ProgressingElement {
		
		public override float progress {
			get {
				return base.progress;
			}
			set {
				base.progress = value;
				if (material != null) {
					material.SetFloat(progressID, base.progress);
				}
			}
		}
		
		public override bool reverse {
			get {
				return base.reverse;
			}
			set {
				base.reverse = value;
				if (material != null) {
					material.SetFloat(reverseID, (base.reverse) ? -1f : 1f);
				}
			}
		}

		public Transform start;
		public Transform end;

		private Material material;
		private int progressID;
		private int reverseID;

		private MeshFilter filter;
		private MeshRenderer meshRenderer;

		private void Start() {
			filter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();

			material = new Material(meshRenderer.material);
			meshRenderer.material = material;
			progressID = Shader.PropertyToID("_Progress");
			reverseID = Shader.PropertyToID("_Reverse");
			progress = 0f; // Safety
			GenerateMesh();
		}

		public void GenerateMesh() {
			Vector3 localStart = transform.InverseTransformPoint(start.position);
			Vector3 localEnd = transform.InverseTransformPoint(end.position);
			Vector3 startToEnd = localEnd - localStart;
			Vector3 right = Vector3.Cross(startToEnd.normalized, Vector3.up);
			float length = startToEnd.magnitude;
			float width = SettingsManager.I.roomSettings.pathWidth;
			Mesh mesh = new Mesh();
			filter.mesh = mesh;

			Vector3[] vertices = new Vector3[4];
			vertices[0] = localStart - right * width;
			vertices[1] = localEnd - right * width;
			vertices[2] = localEnd + right * width;
			vertices[3] = localStart + right * width;
			Vector2[] uvs = new Vector2[4]; // UVs used by textures
			uvs[0] = new Vector2(0, 0);
			uvs[1] = new Vector2(0, length / width);
			uvs[2] = new Vector2(1, length / width);
			uvs[3] = new Vector2(1, 0);
			Vector2[] uvs2 = new Vector2[4]; // UVs used for progress
			uvs2[0] = new Vector2(0, 0);
			uvs2[1] = new Vector2(0, 1);
			uvs2[2] = new Vector2(1, 1);
			uvs2[3] = new Vector2(1, 0);
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
			mesh.uv2 = uvs2;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		}

	}
}
