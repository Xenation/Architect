using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

namespace Architect {
	public class MenuManager : MonoBehaviour {

		public RoomLink linkPlay;
		public string playLevel;
		public RoomLink linkCredits;
		public string creditsLevel;

		private RoomNetwork roomnet;
		private SteamVR_LoadLevel levelLoader;

		private void Awake() {
			roomnet = GetComponent<RoomNetwork>();
			levelLoader = GetComponent<SteamVR_LoadLevel>();
		}

		private void Start() {
			if (linkPlay != null) {
				linkPlay.traverser = new TriggerTraverser(linkPlay, roomnet, PlayTriggered);
			}
			if (linkCredits != null) {
				linkCredits.traverser = new TriggerTraverser(linkCredits, roomnet, CreditsTriggered);
			}
		}

		//private void Update() {
			// Debug Keys
			//if (Input.GetKeyDown(KeyCode.Alpha1)) { // Play
			//	PlayTriggered();
			//} else if (Input.GetKeyDown(KeyCode.Alpha2)) { // Credits
			//	CreditsTriggered();
			//}
		//}

		private void PlayTriggered() {
			if (!Application.CanStreamedLevelBeLoaded(playLevel)) {
				Debug.LogError("Play Scene Not Found!");
				return;
			}
			levelLoader.levelName = playLevel;
			levelLoader.Trigger();
		}

		private void CreditsTriggered() {
			if (!Application.CanStreamedLevelBeLoaded(creditsLevel)) {
				Debug.LogWarning("Credits Scene Not Found!");
				return;
			}
			levelLoader.levelName = creditsLevel;
			levelLoader.Trigger();
		}

	}
}
