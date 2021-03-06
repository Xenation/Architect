﻿using UnityEngine;

namespace Architect {
	public class DefaultLinkTraverser : LinkTraverser {
		
		public DefaultLinkTraverser(RoomLink link, RoomNetwork roomnet) {
			this.link = link;
			this.roomnet = roomnet;
		}

		public override bool Traverse(SilhouetteController controller) {
			link.freezed = true;
			Vector3 toTarget = target - controller.transform.localPosition;
			Vector3 direction = toTarget.normalized;
			controller.transform.localPosition += direction * controller.speed * Time.deltaTime;
			controller.transform.localRotation = Quaternion.Euler(0f, Vector3.SignedAngle(Vector3.forward, direction.Flat3().normalized, Vector3.up), 0f);
			if (toTarget.magnitude < 0.01f) {
				link.freezed = false;
				return true;
			}
			return false;
		}

	}
}
