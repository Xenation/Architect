using UnityEngine;

namespace Architect {
	public class SmoothedCamera : MonoBehaviour {

		public Transform vrCamera;
		public float positionLerpFactor = 0.15f;
		public float rotationLerpFactor = 0.05f;

		private void LateUpdate() {
			transform.position = Vector3.Lerp(transform.position, vrCamera.position, positionLerpFactor);
			transform.rotation = Quaternion.Lerp(transform.rotation, vrCamera.rotation, rotationLerpFactor);
		}

	}
}
