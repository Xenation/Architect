using System.Collections.Generic;
using UnityEngine;
using Architect.LightPaths;

namespace Architect {
	public class RoomLink : MonoBehaviour {

		public Room room1;
		public Room room2;

		[System.NonSerialized] public Vector3 entry1;
		[System.NonSerialized] public Vector3 entry2;

		[System.NonSerialized] public bool reversed = false;

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
					if (_isOpen) {
						OnLinkOpened?.Invoke(this);
					} else {
						OnLinkClosed?.Invoke(this);
					}
				}
			}
		}

		[System.NonSerialized] public bool valid = false;
		public LinkTraverser traverser;
		[System.NonSerialized] public bool freezed = false;

		private RoomNetwork roomnet;

		public delegate void NotifyCallback();
		public event NotifyCallback OnCharEntered;
		public event NotifyCallback OnCharExited;
		public delegate void TargetedNotifyCallback(RoomLink link);
		public event TargetedNotifyCallback OnLinkClosed;
		public event TargetedNotifyCallback OnLinkOpened;

		[System.NonSerialized] public LightPoint[] lightPoints;
		[System.NonSerialized] public List<LightLink> lightLinks = new List<LightLink>();
		private Transform lightPointRoot;
		private Transform lightLineRoot;
		public bool invisibleLinks = false;

		private bool lightElementsBuilded = false;

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

		public void BuildLightElements() {
			lightElementsBuilded = true;
			lightPointRoot = transform.Find("LightPoints");
			lightPoints = new LightPoint[lightPointRoot.childCount];
			for (int i = 0; i < lightPointRoot.childCount; i++) {
				lightPoints[i] = lightPointRoot.GetChild(i).GetComponent<LightPoint>();
			}
			lightLineRoot = new GameObject("LightLines").transform;
			lightLineRoot.SetParent(transform, false);
			if (invisibleLinks) {
				for (int i = 1; i < lightPointRoot.childCount; i++) {
					lightLinks.Add(LightLine.BuildLink(lightLineRoot, gameObject.name, lightPointRoot.GetChild(i - 1).GetComponent<LightPoint>(), lightPointRoot.GetChild(i).GetComponent<LightPoint>()));
				}
			} else {
				for (int i = 1; i < lightPointRoot.childCount; i++) {
					lightLinks.Add(LightLine.BuildLine(lightLineRoot, gameObject.name, lightPointRoot.GetChild(i - 1).GetComponent<LightPoint>(), lightPointRoot.GetChild(i).GetComponent<LightPoint>()));
				}
			}
		}

		public LightPoint GetLightPoint(Room r) {
			if (!lightElementsBuilded) BuildLightElements();
			if (reversed) {
				if (r == room1) {
					return lightPoints[lightPoints.Length - 1];
				} else {
					return lightPoints[0];
				}
			} else {
				if (r == room1) {
					return lightPoints[0];
				} else {
					return lightPoints[lightPoints.Length - 1];
				}
			}
		}

		public void MarkLit() {
			foreach (LightPoint point in lightPoints) {
				point.activated = true;
			}
			foreach (LightLink link in lightLinks) {
				link.activated = true;
			}
		}

		public void MarkUnlit() {
			foreach (LightPoint point in lightPoints) {
				point.activated = false;
			}
			foreach (LightLink link in lightLinks) {
				link.activated = false;
			}
		}

	}
}
