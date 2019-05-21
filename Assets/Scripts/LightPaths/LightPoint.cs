using System.Collections.Generic;

namespace Architect.LightPaths {
	public class LightPoint : LightElement {

		protected List<LightLink> connected = new List<LightLink>();

		protected override void OnSignalUpdate(LightElement origin, float dt) {
			foreach (LightLink link in connected) {
				if (!link.activated || link == origin) continue;
				link.UpdateSignal(this, dt);
			}
		}

		protected override void OnClearUpdateFlag(LightElement origin) {
			foreach (LightLink link in connected) {
				if (!link.activated || link == origin) continue;
				link.ClearUpdateFlag(this);
			}
		}

		public void RegisterConnected(LightLink elem) {
			connected.Add(elem);
		}

		public void UnregisterConnected(LightLink elem) {
			connected.Remove(elem);
		}
	}
}
