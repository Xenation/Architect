using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Architect.Edit {
	[CustomEditor(typeof(VolumeLight)), CanEditMultipleObjects]
	public class VolumeLightEditor : Editor {

		private BoxBoundsHandle volumeBoundsHandle = new BoxBoundsHandle();

		protected virtual void OnSceneGUI() {
			VolumeLight volumeLight = (VolumeLight) target;

			volumeBoundsHandle.handleColor = Color.yellow;
			volumeBoundsHandle.wireframeColor = Color.yellow;
			volumeBoundsHandle.center = Vector3.zero;
			volumeBoundsHandle.size = volumeLight.extents * 2f;

			Matrix4x4 tmpMat = Handles.matrix;
			//Handles.matrix = Matrix4x4.TRS(volumeLight.transform.position, volumeLight.transform.rotation, Vector3.one);
			Handles.matrix = volumeLight.transform.localToWorldMatrix;
			EditorGUI.BeginChangeCheck();
			volumeBoundsHandle.DrawHandle();
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(volumeLight, "Change Volume Light Bounds");

				volumeLight.extents = volumeBoundsHandle.size * 0.5f;
			}

			EditorGUI.BeginChangeCheck();
			Vector3 lightOrigin = Handles.DoPositionHandle(volumeLight.lightOrigin, Quaternion.identity);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(volumeLight, "Move Volume Light Origin");

				volumeLight.lightOrigin = lightOrigin;
			}

			Handles.matrix = tmpMat;
		}

	}
}
