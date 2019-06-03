using UnityEngine;

namespace Architect {
	public class MusicStarter : MonoBehaviour {

		private static bool musicStarted = false;

		private void Start() {
			if (!musicStarted) {
				DontDestroyOnLoad(gameObject);
				musicStarted = true;
				AkSoundEngine.PostEvent("Play_Music", gameObject);
			}
		}

	}
}
