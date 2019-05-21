using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	[ExecuteInEditMode]
	public class Outlined : MonoBehaviour {

		public Color outlineColor;
		public List<Transform> ignoredObjects = new List<Transform>();

		private List<Renderer> _renderers;
		public List<Renderer> renderers {
			get {
				return _renderers;
			}
		}


		private void OnEnable() {
			Init();
		}

		private void Start() {
			Init();
		}

		private void OnDisable() {
			Clear();
		}

		private void OnDestroy() {
			Clear();
		}

		private void Init() { // Currently does not handle objects added between init and clear
			if (_renderers == null) {
				_renderers = new List<Renderer>();
			}
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
				if (ignoredObjects.Contains(renderer.transform)) continue;
				int i = 0;
				for (; i < ignoredObjects.Count; i++) {
					if (renderer.transform.IsChildOf(ignoredObjects[i])) break;
				}
				if (i != ignoredObjects.Count) continue;
				_renderers.Add(renderer);
			}
			OutlinedManager.I.outlinedObjects.Add(this);
		}

		private void Clear() {
			OutlinedManager.I.outlinedObjects.Remove(this);
			if (_renderers != null) {
				_renderers.Clear();
			}
		}

	}
}
