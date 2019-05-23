using UnityEngine;

namespace Architect.LightPaths {
	public class LightNode : LightPoint {

		private float _progress = 0f;
		private float progress {
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

		private Material material;
		private int progressID;

		private MeshFilter filter;
		private MeshRenderer meshRenderer;

		protected override void OnSignalUpdate(LightElement origin, float dt) {
			progress += dt;
			if (progress >= 1f) {
				progress = 1f;
			}
			base.OnSignalUpdate(origin, dt);
		}

		private void Start() {
			filter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();

			material = new Material(meshRenderer.material);
			meshRenderer.material = material;
			progressID = Shader.PropertyToID("_Progress");
			progress = 0f; // Safety
		}

		private void Update() {
			if (wasUpdated) {
				wasUpdated = false;
				return;
			}
			progress = 0f;
		}

	}
}
