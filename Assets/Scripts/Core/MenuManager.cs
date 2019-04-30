using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

namespace Architect {
	public class MenuManager : MonoBehaviour {

		private static bool musicStarted = false;

		public RoomLink linkPlay;
		public string playLevel;
		public RoomLink linkSettings;
		public string settingsLevel;
		public RoomLink linkCredits;
		public string creditsLevel;

		private RoomNetwork roomnet;
		private SteamVR_LoadLevel levelLoader;

		private void Awake() {
			roomnet = GetComponent<RoomNetwork>();
			levelLoader = GetComponent<SteamVR_LoadLevel>();
		}

		private void Start() {
			if (!musicStarted) {
				musicStarted = true;
				AkSoundEngine.PostEvent("Play_Music", gameObject);
			}
			if (linkPlay != null) {
				linkPlay.traverser = new TriggerTraverser(linkPlay, roomnet, PlayTriggered);
			}
			if (linkSettings != null) {
				linkSettings.traverser = new TriggerTraverser(linkSettings, roomnet, SettingsTriggered);
			}
			if (linkCredits != null) {
				linkCredits.traverser = new TriggerTraverser(linkCredits, roomnet, CreditsTriggered);
			}
		}

		private void Update() {
			// Debug Keys
			if (Input.GetKeyDown(KeyCode.Alpha1)) { // Credits
				CreditsTriggered();
			} else if (Input.GetKeyDown(KeyCode.Alpha2)) { // Play
				PlayTriggered();
			} else if (Input.GetKeyDown(KeyCode.Alpha3)) { // Settings
				SettingsTriggered();
			}
		}

		private void PlayTriggered() {
			if (!Application.CanStreamedLevelBeLoaded(playLevel)) {
				Debug.LogError("Play Scene Not Found!");
				return;
			}
			levelLoader.levelName = playLevel;
			levelLoader.Trigger();
		}

		private void SettingsTriggered() {
			if (!Application.CanStreamedLevelBeLoaded(settingsLevel)) {
				Debug.LogWarning("Settings Scene Not Found!");
				return;
			}
			levelLoader.levelName = settingsLevel;
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
