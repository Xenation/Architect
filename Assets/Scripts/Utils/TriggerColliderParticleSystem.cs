using UnityEngine;

namespace Architect {
	[RequireComponent(typeof(ParticleSystem))]
	public class TriggerColliderParticleSystem : MonoBehaviour { // TODO has been specialized to fix undetected triggers

		public bool includeChildren = false;

		private ParticleSystem[] particleSystems;
		private ParticleSystem.EmissionModule[] psEmissions;
		private int triggerStack = 0;

		private BoxCollider boxCollider;
		private bool collides = false;

		private void Start() {
			boxCollider = GetComponent<BoxCollider>();

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

		private void Update() { // Ugly manual check (did not work without)
			bool currentlyColliding = Physics.CheckBox(transform.position + boxCollider.center, boxCollider.size / 2f, transform.rotation, LayerMask.GetMask("Hand"), QueryTriggerInteraction.Collide);
			if (currentlyColliding && !collides) {
				collides = true;
				TriggerEnter();
			} else if (!currentlyColliding && collides) {
				collides = false;
				TriggerExit();
			}
		}

		private void TriggerEnter() {
			if (triggerStack == 0) {
				for (int i = 0; i < particleSystems.Length; i++) {
					psEmissions[i].enabled = true;
				}
			}
			triggerStack++;
		}

		private void TriggerExit() {
			triggerStack--;
			if (triggerStack == 0) {
				for (int i = 0; i < particleSystems.Length; i++) {
					psEmissions[i].enabled = false;
				}
			}
		}

	}
}
