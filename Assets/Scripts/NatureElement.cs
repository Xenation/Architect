using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class NatureElement : MonoBehaviour {

		public float desactivateDelay = .5f;

		private Shader requiredShader;
		private int waveAmplitudeID;

		private Dictionary<Material, float> materialInstances = new Dictionary<Material, float>();
		private int colliderStack = 0;
		private bool disableRequested = false;
		private float disableRequestTime = 0f;

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
						materialInstances.Add(materialInstance, materialInstance.GetFloat(waveAmplitudeID));
						materialInstance.SetFloat(waveAmplitudeID, 0f);
					}
				}
			}
		}

		private void Update() {
			if (disableRequested && Time.time - disableRequestTime >= desactivateDelay) {
				disableRequested = false;
				DisableEffect();
			}
		}

		private void OnTriggerEnter(Collider other) {
			if (colliderStack == 0) {
				EnableEffect();
				disableRequested = false;
			}
			colliderStack++;
		}

		private void OnTriggerExit(Collider other) {
			colliderStack--;
			if (colliderStack == 0) {
				if (desactivateDelay != 0) {
					disableRequested = true;
					disableRequestTime = Time.time;
				} else {
					DisableEffect();
				}
			}
		}

		private void EnableEffect() {
			foreach (KeyValuePair<Material, float> materialPair in materialInstances) {
				materialPair.Key.SetFloat(waveAmplitudeID, materialPair.Value);
			}
		}

		private void DisableEffect() {
			foreach (KeyValuePair<Material, float> materialPair in materialInstances) {
				materialPair.Key.SetFloat(waveAmplitudeID, 0f);
			}
		}

	}
}
