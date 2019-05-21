using UnityEngine;

namespace Architect.LightPaths {
	public class LightLine : LightElement {

		public static LightLine BuildNew(Transform parent, string name, LightPoint p1, LightPoint p2) {
			GameObject pathObj = new GameObject("Line-" + name, typeof(MeshFilter), typeof(MeshRenderer));
			pathObj.transform.SetParent(parent, false);
			MeshRenderer renderer = pathObj.GetComponent<MeshRenderer>();
			renderer.sharedMaterial = SettingsManager.I.roomSettings.pathMaterial;
			LightLine lightLine = pathObj.AddComponent<LightLine>();
			lightLine.point1 = p1;
			lightLine.point2 = p2;
			p1.RegisterConnected(lightLine);
			p2.RegisterConnected(lightLine);
			return lightLine;
		}

		private float _progress = 0f;
		public float progress {
			get {
				return _progress;
			}
			set {
				_progress = value;
				if (material != null) {
					material.SetFloat(progressID, _progress);
				}
			}
		}

		private bool _reverse = false;
		public bool reverse {
			get {
				return _reverse;
			}
			set {
				_reverse = value;
				if (material != null) {
					material.SetFloat(reverseID, (_reverse) ? -1f : 1f);
				}
			}
		}

		public LightPoint point1;
		public LightPoint point2;

		private Material material;
		private int progressID;
		private int reverseID;

		private MeshFilter filter;
		private MeshRenderer meshRenderer;

		protected override void OnUpdate(LightElement origin, float dt) {
			progress += dt;
			if (progress >= 1f) {
				progress = 1f;
				GetOther(origin as LightPoint)?.Update(this, dt);
			}
		}

		protected override void OnClearUpdateFlag(LightElement origin) {
			GetOther(origin as LightPoint)?.ClearUpdateFlag(this);
		}

		private LightPoint GetOther(LightPoint point) {
			return (point == point1) ? point1 : ((point == point2) ? point2 : null);
		}
		
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
			Vector3 local1 = transform.InverseTransformPoint(point1.transform.position);
			Vector3 local2 = transform.InverseTransformPoint(point2.transform.position);
			Vector3 startToEnd = local2 - local1;
			Vector3 right = Vector3.Cross(startToEnd.normalized, Vector3.up);
			float length = startToEnd.magnitude;
			float width = SettingsManager.I.roomSettings.pathWidth;
			Mesh mesh = new Mesh();
			filter.mesh = mesh;

			Vector3[] vertices = new Vector3[4];
			vertices[0] = local1 - right * width;
			vertices[1] = local2 - right * width;
			vertices[2] = local2 + right * width;
			vertices[3] = local1 + right * width;
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
