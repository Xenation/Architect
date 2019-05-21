using UnityEngine;

namespace Architect.LightPaths {
	public abstract class LightElement : MonoBehaviour {

		public bool activated = false;
		protected bool wasUpdated = false;
		private bool updated = false;

		public void UpdateSignal(LightElement origin, float dt) {
			if (updated) return;
			OnSignalUpdate(origin, dt);
			updated = true;
			wasUpdated = true;
		}

		public void ClearUpdateFlag(LightElement origin) {
			if (!updated) return;
			OnClearUpdateFlag(origin);
			updated = false;
		}

		protected abstract void OnSignalUpdate(LightElement origin, float dt);
		protected abstract void OnClearUpdateFlag(LightElement origin);

	}
}
