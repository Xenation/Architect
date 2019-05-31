using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class NatureElement : MonoBehaviour {
		
		private Shader requiredShader;
		private int waveAmplitudeID;

		private Dictionary<Material, float> materialInstances = new Dictionary<Material, float>();

		private void Awake() {
			requiredShader = Shader.Find("Shader Graphs/Plants");
			waveAmplitudeID = Shader.PropertyToID("_WaveAmplitude");

			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			List<Material> materials = new List<Material>();
			foreach (Renderer renderer in renderers) {
				materials.Clear();
				renderer.GetMaterials(materials);
				for (int i = 0; i < materials.Count; i++) {
					if (materials[i].shader == requiredShader) {
						Material materialInstance = materials[i];
						materialInstance.SetFloat(waveAmplitudeID, 0f);
						materialInstances.Add(materialInstance, materialInstance.GetFloat(waveAmplitudeID));
					}
				}
			}
		}

		private void OnTriggerEnter(Collider other) {
			Debug.Log("Enter");
			foreach (KeyValuePair<Material, float> materialPair in materialInstances) {
				materialPair.Key.SetFloat(waveAmplitudeID, materialPair.Value);
			}
		}

		private void OnTriggerExit(Collider other) {
			Debug.Log("Exit");
			foreach (KeyValuePair<Material, float> materialPair in materialInstances) {
				materialPair.Key.SetFloat(waveAmplitudeID, 0f);
			}
		}

	}
}
