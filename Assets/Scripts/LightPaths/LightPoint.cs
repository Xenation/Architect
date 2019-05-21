using System.Collections.Generic;
using UnityEngine;

namespace Architect.LightPaths {
	public class LightPoint : LightElement {

		protected List<LightLine> connected = new List<LightLine>();

		protected override void OnSignalUpdate(LightElement origin, float dt) {
			foreach (LightLine line in connected) {
				if (!line.activated || line == origin) continue;
				line.UpdateSignal(this, dt);
			}
		}

		protected override void OnClearUpdateFlag(LightElement origin) {
			foreach (LightLine line in connected) {
				if (!line.activated || line == origin) continue;
				line.ClearUpdateFlag(this);
			}
		}

		public void RegisterConnected(LightLine elem) {
			connected.Add(elem);
		}

		public void UnregisterConnected(LightLine elem) {
			connected.Remove(elem);
		}
	}
}
