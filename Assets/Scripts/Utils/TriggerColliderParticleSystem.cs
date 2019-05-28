using UnityEngine;

namespace Architect {
	[RequireComponent(typeof(ParticleSystem))]
	public class TriggerColliderParticleSystem : MonoBehaviour {

		public bool includeChildren = false;

		private ParticleSystem[] particleSystems;
		private ParticleSystem.EmissionModule[] psEmissions;

		private void Start() {
			if (includeChildren) {
				particleSystems = GetComponentsInChildren<ParticleSystem>();
			} else {
				particleSystems = new ParticleSystem[1] { GetComponent<ParticleSystem>() };
			}
			psEmissions = new ParticleSystem.EmissionModule[particleSystems.Length];
			for (int i = 0; i < particleSystems.Length; i++) {
				psEmissions[i] = particleSystems[i].emission;
				if (psEmissions[i].enabled) {
					psEmissions[i].enabled = false;
				}
			}
		}

		private void OnTriggerEnter(Collider other) {
			for (int i = 0; i < particleSystems.Length; i++) {
				psEmissions[i].enabled = true;
			}
		}

		private void OnTriggerExit(Collider other) {
			for (int i = 0; i < particleSystems.Length; i++) {
				psEmissions[i].enabled = false;
			}
		}

	}
}
