using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class VolumeLightManager : AutoSingleton<VolumeLightManager> {

		private const int maxVolumeLights = 64;
		private List<VolumeLight> volumeLights = new List<VolumeLight>();
		private Vector4[] lightExtentsBuffer = new Vector4[maxVolumeLights];
		private Vector4[] lightColorBuffer = new Vector4[maxVolumeLights];
		private Matrix4x4[] lightTransformBuffer = new Matrix4x4[maxVolumeLights];
		private Vector4[] lightOriginBuffer = new Vector4[maxVolumeLights];

		public void RegisterVolumeLight(VolumeLight light) {
			light.id = volumeLights.Count;
			volumeLights.Add(light);
			UpdateLightInBuffers(volumeLights.Count - 1);
			UploadVolumeLightsToGL();
		}

		public void NotifyVolumeLightChange(VolumeLight light) {
			UpdateLightInBuffers(light.id);
			UploadVolumeLightsToGL();
		}

		public void UnregisterVolumeLight(VolumeLight light) {
			for (int i = light.id + 1; i < volumeLights.Count; i++) {
				volumeLights[i].id--;
			}
			volumeLights.RemoveAt(light.id);
			RebuildLightBuffers();
			UploadVolumeLightsToGL();
		}

		private void UploadVolumeLightsToGL() {
			Shader.SetGlobalInt("_VolumeLightCount", volumeLights.Count);
			Shader.SetGlobalVectorArray("_VolumeLightExtentsBuffer", lightExtentsBuffer);
			Shader.SetGlobalVectorArray("_VolumeLightColorBuffer", lightColorBuffer);
			Shader.SetGlobalMatrixArray("_VolumeLightTransformBuffer", lightTransformBuffer);
			Shader.SetGlobalVectorArray("_VolumeLightOriginBuffer", lightOriginBuffer);
		}

		private void UpdateLightInBuffers(int i) {
			if (i >= maxVolumeLights) {
				//Debug.LogWarning("[Volume Lights] To many lights, some will be ignored!");
				return;
			}
			lightExtentsBuffer[i] = new Vector4(volumeLights[i].extents.x, volumeLights[i].extents.y, volumeLights[i].extents.z, volumeLights[i].fadeWidth);
			lightColorBuffer[i] = new Vector4(volumeLights[i].color.r, volumeLights[i].color.g, volumeLights[i].color.b, volumeLights[i].intensity);
			lightTransformBuffer[i] = volumeLights[i].transform.worldToLocalMatrix;
			lightOriginBuffer[i] = volumeLights[i].transform.TransformPoint(volumeLights[i].lightOrigin);
		}

		private void RebuildLightBuffers() {
			if (volumeLights.Count > maxVolumeLights) {
				Debug.LogWarning("[Volume Lights] To many lights, some will be ignored!");
			}
			for (int i = 0; i < volumeLights.Count && i < maxVolumeLights; i++) {
				UpdateLightInBuffers(i);
			}
		}

	}
}
