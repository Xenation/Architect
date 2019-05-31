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

		private uint stack = 0;

		private void OnEnable() {
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
			if (stack == 0) {
				enabled = true;
			}
			stack++;
		}

		public override void DisableHighlight() {
			stack--;
			if (stack == 0) {
				enabled = false;
			}
		}
	}
}
