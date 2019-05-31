using UnityEngine;

namespace Architect.LightPaths {
	public class LightLineEffect : MonoBehaviour {

		private LightLine line;
		private ParticleSystem.EmissionModule psEmission;

		private Vector3 localStart;
		private Vector3 localEnd;

		private Vector3 prevLocalPos;

		public void Init(LightLine ll) {
			line = ll;
			localStart = transform.InverseTransformPoint(line.point1.transform.position);
			localEnd = transform.InverseTransformPoint(line.point2.transform.position);
			GameObject effectGO = Instantiate(SettingsManager.I.roomSettings.lightPathEffect, transform);
			effectGO.transform.localPosition = Vector3.zero;
			psEmission = effectGO.GetComponent<ParticleSystem>().emission;
			prevLocalPos = transform.localPosition;
		}

		private void Update() {
			if (line.reverse) {
				transform.localPosition = Vector3.Lerp(localEnd, localStart, line.progress);
			} else {
				transform.localPosition = Vector3.Lerp(localStart, localEnd, line.progress);
			}
			if (prevLocalPos == transform.localPosition) {
				psEmission.enabled = false;
			} else {
				psEmission.enabled = true;
			}
			prevLocalPos = transform.localPosition;
		}

	}
}
