using UnityEngine;

namespace Architect {
	[RequireComponent(typeof(ParticleSystem))]
	public class AutoDestroyedParticleSystem : MonoBehaviour {

		private new ParticleSystem particleSystem;

		private void Start() {
			particleSystem = GetComponent<ParticleSystem>();
		}

		private void Update() {
			if (!particleSystem.IsAlive(true)) {
				Destroy(gameObject);
			}
		}

	}
}
