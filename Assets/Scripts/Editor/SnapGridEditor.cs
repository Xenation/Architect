using UnityEditor;
using UnityEngine;

namespace Architect.Edit {
	[CustomEditor(typeof(SnapGrid))]
	public class SnapGridEditor : Editor {

		public void OnEnable() {
			Object[] targs = targets;
			foreach (SnapGrid grid in targs) {
				grid.RecreatePreview();
			}
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			Object[] targs = targets;
			if (GUILayout.Button("Update Preview")) {
				foreach (SnapGrid grid in targs) {
					grid.RecreatePreview();
				}
			}
			
		}

		public void OnDisable() {
			Object[] targs = targets;
			foreach (SnapGrid grid in targs) {
				grid.DeletePreview();
			}
		}

	}
}
