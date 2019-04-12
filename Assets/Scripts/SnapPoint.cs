using UnityEngine;

namespace Architect {
	public class SnapPoint : MonoBehaviour {

		public SnapPointType type;

		public GameObject model;
		public PointSnappable snapped = null;

		[System.NonSerialized] public RoomLink link;

		private uint previewCounter = 0;
		private GameObject preview;

		private Transform entry1;
		private Transform entry2;

		private void Awake() {
			link = GetComponentInParent<RoomLink>();
			entry1 = transform.Find("Entry1");
			entry2 = transform.Find("Entry2");
			if (Vector3.Distance(entry1.position, link.room1.transform.position) < Vector3.Distance(entry2.position, link.room1.transform.position)) {
				link.entry1 = entry1.position;
				link.entry2 = entry2.position;
			} else {
				link.entry1 = entry2.position;
				link.entry2 = entry1.position;
			}

			InitPreview();
		}

		public void Snap(Transform transf, Transform reference) {
			transf.rotation = transform.rotation;
			transf.position = transform.position;
		}

		private void InitPreview() {
			switch (type) {
				case SnapPointType.Door:
					preview = Instantiate(SettingsManager.I.roomSettings.doorPreview, transform);
					break;
				case SnapPointType.Balcony:
					preview = Instantiate(SettingsManager.I.roomSettings.balconyPreview, transform);
					break;
			}
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
