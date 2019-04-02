using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class GridManager : AutoSingleton<GridManager> {

		private List<SnapGrid> grids = new List<SnapGrid>();

		private List<Material> gridMaterialInstances = new List<Material>();
		private bool[] activeFocuses = { false, false };
		private int[] focusPosLocations = new int[2];
		private int[] focusRadiusLocations = new int[2];
		private int[] focusFalloffRadiusLocations = new int[2];

		public GridManager() {
			focusPosLocations[0] = Shader.PropertyToID("_FocusPos1");
			focusRadiusLocations[0] = Shader.PropertyToID("_FocusRadius1");
			focusFalloffRadiusLocations[0] = Shader.PropertyToID("_FocusFalloffRadius1");
			focusPosLocations[1] = Shader.PropertyToID("_FocusPos2");
			focusRadiusLocations[1] = Shader.PropertyToID("_FocusRadius2");
			focusFalloffRadiusLocations[1] = Shader.PropertyToID("_FocusFalloffRadius2");
		}

		public void RegisterSnapGrid(SnapGrid grid) {
			grids.Add(grid);
			grid.gridMat = CreateGridMaterialInstance();
		}

		public void UnregisterSnapGrid(SnapGrid grid) {
			grids.Remove(grid);
			ReleaseGridMaterialInstance(grid.gridMat);
		}

		public Material CreateGridMaterialInstance() {
			Material matInstance = new Material(GridSettings.I.gridMaterial);
			gridMaterialInstances.Add(matInstance);
			return matInstance;
		}

		public void ReleaseGridMaterialInstance(Material matInstance) {
			gridMaterialInstances.Remove(matInstance);
		}

		public int GetInactiveFocusIndex() {
			for (int i = 0; i < activeFocuses.Length; i++) {
				if (!activeFocuses[i]) {
					return i;
				}
			}
			return -1;
		}

		public void ActivateFocus(int index) {
			activeFocuses[index] = true;
		}

		public void DeactivateFocus(int index) {
			activeFocuses[index] = false;
			SetFocusRadius(index, 0.001f, 0.0001f);
		}

		public void SetFocusPosition(int index, Vector3 pos) {
			foreach (SnapGrid grid in grids) {
				Vector3 locPos = grid.transform.worldToLocalMatrix.MultiplyPoint3x4(pos);
				grid.gridMat.SetVector(focusPosLocations[index], new Vector4(locPos.x, locPos.y, locPos.z, 0));
			}
		}

		public void SetFocusRadius(int index, float radius, float falloff) {
			foreach (Material matInstance in gridMaterialInstances) {
				matInstance.SetFloat(focusRadiusLocations[index], radius);
				matInstance.SetFloat(focusFalloffRadiusLocations[index], falloff);
			}
		}

	}
}
