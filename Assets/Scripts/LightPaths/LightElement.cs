using UnityEngine;

namespace Architect.LightPaths {
	public abstract class LightElement : MonoBehaviour {

		public bool activated = false;
		private bool updated = false;

		public void Update(LightElement origin, float dt) {
			if (updated) return;
			OnUpdate(origin, dt);
			updated = true;
		}

		public void ClearUpdateFlag(LightElement origin) {
			if (!updated) return;
			OnClearUpdateFlag(origin);
			updated = false;
		}

		protected abstract void OnUpdate(LightElement origin, float dt);
		protected abstract void OnClearUpdateFlag(LightElement origin);

	}
}
