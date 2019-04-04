using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	[CreateAssetMenu(fileName = "GridSettings", menuName = "Architect/GridSettings", order = 31)]
	public class GridSettings : ScriptableObject {

		private static GridSettings instance = null;
		public static GridSettings I {
			get {
				if (instance == null) {
					Debug.LogWarning("Accessing GridSettings instance before assignement!");
				}
				return instance;
			}
		}

		public float snapStep = 0.01f;
		public Material gridMaterial;

		private void OnEnable() {
			instance = this;
		}

	}
}
