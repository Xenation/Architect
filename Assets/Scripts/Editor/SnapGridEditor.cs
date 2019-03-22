using UnityEditor;
using UnityEngine;

namespace Architect.Edit {
	[CustomEditor(typeof(SnapGrid))]
	public class SnapGridEditor : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			SnapGrid grid = (SnapGrid) target;
			if (GUILayout.Button("Preview Shape")) {
				grid.RecreatePreview();
			}
			if (GUILayout.Button("Delete Preview")) {
				grid.DeletePreview();
			}
		}

	}
}
