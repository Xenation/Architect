using UnityEngine;

namespace Architect {
	public class RoomLink : MonoBehaviour {

		public Room room1;
		public Room room2;

		[System.NonSerialized] public Vector3 entry1;
		[System.NonSerialized] public Vector3 entry2;

		public bool overrideOpen = false;
		private bool _prevOverrideOpen = false;

		private bool _isOpen = false;
		public bool isOpen {
			get {
				return overrideOpen || _isOpen;
			}
			set {
				bool prevOpen = _isOpen;
				_isOpen = value;
				if (prevOpen != _isOpen) {
					roomnet?.OnLinkChange(this);
				}
			}
		}

		[System.NonSerialized] public bool valid = false;
		public LinkTraverser traverser;

		private RoomNetwork roomnet;

		public delegate void NotifyCallback();
		public event NotifyCallback OnCharEntered;
		public event NotifyCallback OnCharExited;

		public Room GetOther(Room r) {
			return (r == room1) ? room2 : room1;
		}

		public Vector3 GetEntry(Room r) {
			return (r == room1) ? entry1 : entry2;
		}
		
		public void ApplyLink() {
			room1.RegisterLink(this);
			room2.RegisterLink(this);
			valid = true;
			roomnet?.OnLinkChange(this);
		}

		public void BreakLink() {
			room1.UnregisterLink(this);
			room2.UnregisterLink(this);
			valid = false;
			roomnet?.OnLinkChange(this);
		}

		private void Start() {
			roomnet = FindObjectOfType<RoomNetwork>();
		}

		private void Update() {
			if (_prevOverrideOpen != overrideOpen) {
				_prevOverrideOpen = overrideOpen;
				roomnet?.OnLinkChange(this);
			}
		}

		public void NotifyCharacterEnter() {
			OnCharEntered?.Invoke();
		}

		public void NotifyCharacterExit() {
			OnCharExited?.Invoke();
		}

	}
}
