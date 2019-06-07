using UnityEngine;

namespace Architect.LightPaths {
	public abstract class LightElement : MonoBehaviour {

		public bool activated = false;
		protected bool wasUpdated = false;
		private bool updated = false;
		private bool cleared = false;

		public void UpdateSignal(LightElement origin, float dt) {
			if (updated) return;
			updated = true;
			OnSignalUpdate(origin, dt);
			cleared = false;
			wasUpdated = true;
		}

		public void ClearUpdateFlag(LightElement origin) {
			if (cleared) return;
			cleared = true;
			OnClearUpdateFlag(origin);
			updated = false;
		}

		protected abstract void OnSignalUpdate(LightElement origin, float dt);
		protected abstract void OnClearUpdateFlag(LightElement origin);

	}
}
