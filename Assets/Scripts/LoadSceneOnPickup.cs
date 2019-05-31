using UnityEngine;
using Valve.VR;

namespace Architect {
	[RequireComponent(typeof(SteamVR_LoadLevel))]
	public class LoadSceneOnPickup : MonoBehaviour {

		private Snappable snappable;
		private SteamVR_LoadLevel levelLoader;

		private void Awake() {
			snappable = GetComponent<Snappable>();
			snappable.OnPickedUp += PickedUp;
			levelLoader = GetComponent<SteamVR_LoadLevel>();
		}

		private void PickedUp() {
			levelLoader.Trigger();
			Destroy(snappable); // A bit overkill but prevents errors
		}

	}
}
