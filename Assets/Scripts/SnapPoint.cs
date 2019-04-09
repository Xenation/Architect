﻿using UnityEngine;

namespace Architect {
	public class SnapPoint : MonoBehaviour {

		public SnapPointType type;

		public GameObject model;
		public PointSnappable snapped = null;

		private uint previewCounter = 0;
		private GameObject preview;

		private void Awake() {
			InitPreview();
		}

		public void Snap(Transform transf, Transform reference) {
			transf.rotation = transform.rotation;
			transf.position = transform.position;
		}

		private void InitPreview() {
			preview = Instantiate(SettingsManager.I.roomSettings.doorPreview, transform);
			preview.transform.position = transform.position;
			preview.transform.rotation = transform.rotation;
			preview.name = "SnapPreview";
			preview.SetActive(false);
		}

		public void EnablePreview() {
			previewCounter++;
			if (preview.activeInHierarchy) return; // Already enabled
			preview.SetActive(true);
			model.SetActive(false);
		}

		public void DisablePreview() {
			previewCounter--;
			if (previewCounter != 0) return;
			preview.SetActive(false);
			if (snapped == null) {
				model.SetActive(true);
			}
		}

	}
}