namespace Architect {
	public class TriggerTraverser : DefaultLinkTraverser {

		public delegate void TraversedCallback();
		private TraversedCallback callback;

		public TriggerTraverser(RoomLink link, RoomNetwork roomnet, TraversedCallback callback) : base(link, roomnet) {
			this.callback = callback;
		}

		public override bool Traverse(SilhouetteController controller) {
			if (base.Traverse(controller)) {
				callback();
				return true;
			} else {
				return false;
			}
		}

	}
}
