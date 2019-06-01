using UnityEngine;
using Valve.VR;

namespace Architect {
	public class DevKeys : MonoBehaviour {

		private static DevKeys instance;

		public static void CreateInstance() {
			if (instance != null) return;
			GameObject go = new GameObject("DevKeys");
			instance = go.AddComponent<DevKeys>();
			instance.levelLoader = go.AddComponent<SteamVR_LoadLevel>();
			DontDestroyOnLoad(go);
		}

		private SteamVR_LoadLevel levelLoader;

		private void Update() {
			if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Home)) { // Menu Scene
				LoadLevel("Menu3");
			} else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad1)) { // Tutorial Scene
				LoadLevel("Tutorial");
			} else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad2)) { // Level 1 Scene
				LoadLevel("Level_1");
			} else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad3)) { // Credits Scene
				LoadLevel("Credits");
			}
		}

		private void LoadLevel(string levelName) {
			levelLoader.levelName = levelName;
			levelLoader.Trigger();
		}

		private void OnDestroy() {
			Debug.Log("Destroyed!");
		}

	}
}
