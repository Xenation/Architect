using UnityEngine;
using Valve.VR;

namespace Architect {
	public class LevelExit : MonoBehaviour {

		public string menuName;

		private RoomNetwork roomnet;
		private RoomLink link;
		private SteamVR_LoadLevel levelLoader;

		private void Awake() {
			roomnet = GetComponentInParent<RoomNetwork>();
			link = GetComponent<RoomLink>();
			levelLoader = GetComponent<SteamVR_LoadLevel>();
			Vector3 entry1Pos = roomnet.WorldToRelativePos(transform.Find("Entry1").position);
			Vector3 entry2Pos = roomnet.WorldToRelativePos(transform.Find("Entry2").position);
			if (Vector3.Distance(entry1Pos, roomnet.WorldToRelativePos(link.room1.center)) < Vector3.Distance(entry2Pos, roomnet.WorldToRelativePos(link.room1.center))) {
				link.entry1 = entry1Pos;
				link.entry2 = entry2Pos;
			} else {
				link.entry1 = entry2Pos;
				link.entry2 = entry1Pos;
			}
			link.isOpen = true;
		}

		private void Start() {
			link.traverser = new TriggerTraverser(link, roomnet, OnLevelEndReached);
		}

		private void Update() {
			// Debug Keys
			if (Input.GetKeyDown(KeyCode.Tab)) { // Restart
				OnLevelEndReached();
			} else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Tilde) || Input.GetKeyDown(KeyCode.Home)) { // Back To Menu
				BackToMenu();
			}
		}

		private void OnLevelEndReached() {
			levelLoader.Trigger();
		}

		private void BackToMenu() {
			levelLoader.levelName = menuName;
			levelLoader.Trigger();
		}

	}
}
