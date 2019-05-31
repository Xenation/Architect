using UnityEngine;

namespace Architect {
	public class SettingsManager : Singleton<SettingsManager> {

		public RoomSettings roomSettings;
		public RoomNetwork activeRoomnet {
			get {
				if (_activeRoomnet == null) {
					_activeRoomnet = FindObjectOfType<RoomNetwork>();
				}
				return _activeRoomnet;
			}
			set {
				_activeRoomnet = value;
			}
		}
		private RoomNetwork _activeRoomnet;

	}
}
