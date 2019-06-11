using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;

namespace Architect {
	[ExecuteInEditMode]
	public class Outlined : HighlightHandler {

		public enum Mode {
			Blacklist,
			Whitelist
		}

		[Tooltip("The Color of this object's outline")]
		public Color outlineColor;
		[Tooltip("The mode of the Object List: Whitelist or Blacklist")]
		public Mode listMode = Mode.Blacklist;
		[Tooltip("Whether to apply whitelisting/blacklisting to the childs of the objects")]
		public bool recursive = true;
		[Tooltip("The list of objects to whitelist or blacklist")]
		[FormerlySerializedAs("ignoredObjects")]
		public List<Transform> objectList = new List<Transform>();

		private List<Renderer> _renderers;
		public List<Renderer> renderers {
			get {
				return _renderers;
			}
		}

		[System.NonSerialized]
		public Color currentColor;
		private Dictionary<int, Color> priorityEnables = new Dictionary<int, Color>();
		private int currentPriority = 0;

		private void OnEnable() {
			if (priorityEnables.Count == 0) { // Allows in edit on/off
				EnableHighlight(0, outlineColor);
			}
			Init();
		}

		private void Start() {
			Init();
		}

		private void OnDisable() {
			Clear();
		}

		private void OnDestroy() {
			Clear();
		}

		private void Init() { // Currently does not handle objects added between init and clear
			if (_renderers == null) {
				_renderers = new List<Renderer>();
			}
			switch (listMode) {
				case Mode.Blacklist:
					if (recursive) {
						foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
							if (objectList.Contains(renderer.transform)) continue; // In blacklist: skip
							int i = 0;
							for (; i < objectList.Count; i++) {
								if (renderer.transform.IsChildOf(objectList[i])) break;
							}
							if (i != objectList.Count) continue; // child of blacklisted: skip
							_renderers.Add(renderer);
						}
					} else {
						foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
							if (objectList.Contains(renderer.transform)) continue; // In blacklist: skip
							_renderers.Add(renderer);
						}
					}
					break;
				case Mode.Whitelist:
					if (recursive) {
						foreach (Transform transf in objectList) {
							Renderer[] whitelistedRenderers = transf.GetComponentsInChildren<Renderer>();
							foreach (Renderer renderer in whitelistedRenderers) {
								_renderers.Add(renderer);
							}
						}
					} else {
						foreach (Transform transf in objectList) {
							Renderer renderer = transf.GetComponent<Renderer>();
							if (renderer == null) continue;
							_renderers.Add(renderer);
						}
					}
					break;
			}
			OutlinedManager.I.outlinedObjects.Add(this);
		}

		private void Clear() {
			OutlinedManager.I.outlinedObjects.Remove(this);
			if (_renderers != null) {
				_renderers.Clear();
			}
		}

		public override void EnableHighlight() {
			EnableHighlight(0, outlineColor);
		}

		public override void DisableHighlight() {
			DisableHighlight(0);
		}

		public void EnableHighlight(int priority, Color color) {
			priorityEnables.Add(priority, color);
			if (priorityEnables.Count == 1) {
				enabled = true;
			}
			currentPriority = FindHighestPriority();
			if (priority == currentPriority) {
				currentColor = priorityEnables[currentPriority];
			}
		}

		public void UpdateHighlight(int priority, Color color) {
			if (priorityEnables.ContainsKey(priority)) {
				priorityEnables[priority] = color;
				if (priority == currentPriority) {
					currentColor = priorityEnables[currentPriority];
				}
			}
		}

		public void DisableHighlight(int priority) {
			priorityEnables.Remove(priority);
			if (priorityEnables.Count == 0) {
				enabled = false;
			} else {
				currentPriority = FindHighestPriority();
				currentColor = priorityEnables[currentPriority];
			}
		}

		private int FindHighestPriority() {
			int maxPriority = int.MinValue;
			foreach (int priority in priorityEnables.Keys) {
				if (priority > maxPriority) {
					maxPriority = priority;
				}
			}
			return maxPriority;
		}
	}
}
