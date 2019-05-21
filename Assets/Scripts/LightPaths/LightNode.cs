namespace Architect.LightPaths {
	public class LightNode : LightPoint {

		private bool open = false;
		private LightElement origin = null;

		private float progress = 0f;

		public void Open() {
			open = true;
		}

		public void Close() {
			open = false;
		}

		protected override void OnUpdate(LightElement origin, float dt) {
			progress += dt;
			if (progress >= 1f) {
				progress = 1f;
			}
			if (open) {
				base.OnUpdate(origin, dt);
			}
		}

	}
}
