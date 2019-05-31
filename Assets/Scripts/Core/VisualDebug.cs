using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Architect {
	public static class VisualDebug {

		private struct Line {
			public Vector3 p1;
			public Vector3 p2;
			public Color color;
		}

		private struct Sphere {
			public Vector3 center;
			public float radius;
			public Color color;
		}

		private static bool initialized = false;

		private static List<Line> lines = new List<Line>();
		private static List<Sphere> spheres = new List<Sphere>();

		private static void TryInitialize() {
#if UNITY_EDITOR
			if (!initialized) {
				EditorApplication.playModeStateChanged += OnPlayModeChanged;
				SceneView.onSceneGUIDelegate += OnSceneGUI;
				initialized = true;
			}
#endif
		}

		public static void DrawLine(Vector3 p1, Vector3 p2, Color color) {
#if UNITY_EDITOR
			TryInitialize();
			lines.Add(new Line { p1 = p1, p2 = p2, color = color });
#endif
		}

		public static void ClearLines() {
#if UNITY_EDITOR
			lines.Clear();
#endif
		}

		public static void DrawWireSphere(Vector3 center, float radius, Color color) {
#if UNITY_EDITOR
			TryInitialize();
			spheres.Add(new Sphere { center = center, radius = radius, color = color });
#endif
		}

		public static void ClearSpheres() {
#if UNITY_EDITOR
			spheres.Clear();
#endif
		}

#if UNITY_EDITOR
		private static void OnSceneGUI(SceneView scene) {
			Color tmpCol = Handles.color;

			foreach (Line line in lines) {
				Handles.color = line.color;
				Handles.DrawLine(line.p1, line.p2);
			}

			foreach (Sphere sphere in spheres) {
				Handles.color = sphere.color;
				Handles.DrawWireDisc(sphere.center, Vector3.up, sphere.radius);
				int discs = 4;
				for (int i = 0; i < discs; i++) {
					Handles.DrawWireDisc(sphere.center, new Vector3(Mathf.Sin(Mathf.PI * (i / (float) discs)), 0f, Mathf.Cos(Mathf.PI * (i / (float) discs))), sphere.radius);
				}
			}

			Handles.color = tmpCol;
		}

		private static void OnPlayModeChanged(PlayModeStateChange state) {
			if (state == PlayModeStateChange.ExitingPlayMode) {
				ClearLines();
				ClearSpheres();
			}
		}
#endif

	}
}
