using UnityEngine;

namespace Architect {
	[ExecuteInEditMode]
	public class Outlined : MonoBehaviour {

		public Color outlineColor;

		private void OnEnable() {
			OutlinedManager.I.outlinedObjects.Add(this);
		}

		private void Start() {
			OutlinedManager.I.outlinedObjects.Add(this);
		}

		private void OnDisable() {
			OutlinedManager.I.outlinedObjects.Remove(this);
		}

		private void OnDestroy() {
			OutlinedManager.I.outlinedObjects.Remove(this);
		}

	}
}
