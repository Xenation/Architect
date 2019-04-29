using UnityEngine;
using Valve.VR;

namespace Architect {
	public class ExitTraverser : LinkTraverser {

		private SteamVR_LoadLevel levelLoader;

		public ExitTraverser(RoomLink link, RoomNetwork roomnet, SteamVR_LoadLevel levelLoader) {
			this.link = link;
			this.roomnet = roomnet;
			this.levelLoader = levelLoader;
		}

		public override bool Traverse(SilhouetteController controller) {
			Vector3 toTarget = target - controller.transform.localPosition;
			Vector3 direction = toTarget.normalized;
			controller.transform.localPosition += direction * controller.speed * Time.deltaTime;
			controller.transform.localRotation = Quaternion.Euler(0f, Vector3.SignedAngle(Vector3.forward, direction, Vector3.up), 0f);
			if (toTarget.magnitude < 0.01f) {
				levelLoader.Trigger();
				return true;
			}
			return false;
		}

	}
}
