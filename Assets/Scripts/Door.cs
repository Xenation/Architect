using UnityEngine;

namespace Architect {
	public class Door : MonoBehaviour {

		public Transform doorTransform;
		public float transitionSpeed = 5f;

		[System.NonSerialized] public bool openned = false;

		private float currentProgress = 0;

		private void Update() {
			if (openned && currentProgress < 1f) {
				currentProgress += transitionSpeed * Time.deltaTime;
				if (currentProgress > 1f) {
					currentProgress = 1f;
				}
			} else if (!openned && currentProgress > 0f) {
				currentProgress -= transitionSpeed * Time.deltaTime;
				if (currentProgress < 0f) {
					currentProgress = 0f;
				}
			}

			doorTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(-90f, 0f, 0f), Quaternion.Euler(-90f, 90f, 0f), currentProgress);
		}

	}
}
