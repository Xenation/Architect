using UnityEngine;

namespace Architect {
	[ExecuteInEditMode]
	public class VolumeLight : MonoBehaviour {

		public Vector3 extents = Vector3.one * 0.15f;
		public Vector3 lightOrigin = Vector3.up * 0.1f;
		public Color color = Color.white;
		public float intensity = 0.5f;
		public float fadeWidth = 0.01f;

		[System.NonSerialized]
		public int id = 0;

		// Ugly af
		private Vector3 prevExtents;
		private Vector3 prevLightOrigin;
		private Color prevColor;
		private float prevIntensity;
		private float prevFadeWidth;

		private void OnEnable() {
			VolumeLightManager.I.RegisterVolumeLight(this);
			prevExtents = extents;
			prevLightOrigin = lightOrigin;
			prevColor = color;
			prevIntensity = intensity;
			prevFadeWidth = fadeWidth;
		}

		private void Update() {
			if (transform.hasChanged || extents != prevExtents || lightOrigin != prevLightOrigin || color != prevColor || intensity != prevIntensity || fadeWidth != prevFadeWidth) {
				VolumeLightManager.I.NotifyVolumeLightChange(this);
				transform.hasChanged = false;
				prevExtents = extents;
				prevLightOrigin = lightOrigin;
				prevColor = color;
				prevIntensity = intensity;
				prevFadeWidth = fadeWidth;
			}
		}

		private void OnDisable() {
			VolumeLightManager.I.UnregisterVolumeLight(this);
		}

	}
}
