using UnityEngine;

namespace Architect {
	public class RoomLink : MonoBehaviour {

		public Room room1;
		public Room room2;

		public bool overrideOpen = false;

		private bool _isOpen = false;
		public bool isOpen {
			get {
				return overrideOpen || _isOpen;
			}
			set {
				_isOpen = value;
			}
		}

		[System.NonSerialized] public bool valid = false;

		public Room GetOther(Room r) {
			return (r == room1) ? room2 : room1;
		}
		
		public void ApplyLink() {
			room1.RegisterLink(this);
			room2.RegisterLink(this);
			valid = true;
		}

		public void BreakLink() {
			room1.UnregisterLink(this);
			room2.UnregisterLink(this);
			valid = false;
		}

	}
}
