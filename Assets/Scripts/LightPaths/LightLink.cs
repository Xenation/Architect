using UnityEngine;

namespace Architect.LightPaths {
	public class LightLink : LightElement {

		public static LightLink BuildLink(Transform parent, string name, LightPoint p1, LightPoint p2) {
			GameObject pathObj = new GameObject("Line-" + name, typeof(MeshFilter), typeof(MeshRenderer));
			pathObj.transform.SetParent(parent, false);
			MeshRenderer renderer = pathObj.GetComponent<MeshRenderer>();
			renderer.sharedMaterial = SettingsManager.I.activeRoomnet.pathMaterial;
			LightLink lightLink = pathObj.AddComponent<LightLink>();
			lightLink.point1 = p1;
			lightLink.point2 = p2;
			p1.RegisterConnected(lightLink);
			p2.RegisterConnected(lightLink);
			return lightLink;
		}

		public static void DestroyLink(LightLink link) {
			link.point1.UnregisterConnected(link);
			link.point2.UnregisterConnected(link);
			Destroy(link.gameObject);
		}

		public LightPoint point1;
		public LightPoint point2;

		protected override void OnSignalUpdate(LightElement origin, float dt) {
			GetOther(origin as LightPoint)?.UpdateSignal(this, dt);
		}

		protected override void OnClearUpdateFlag(LightElement origin) {
			GetOther(origin as LightPoint)?.ClearUpdateFlag(this);
		}

		protected LightPoint GetOther(LightPoint point) {
			return (point == point1) ? point2 : ((point == point2) ? point1 : null);
		}

	}
}
