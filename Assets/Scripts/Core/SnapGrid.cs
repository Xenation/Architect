using UnityEngine;

namespace Architect {
	public class SnapGrid : MonoBehaviour {

		public float snap = 0.1f;

		private Matrix4x4 wtl;
		private Matrix4x4 ltw;

		private float gridHeight { get { return transform.position.y; } }

		private void Awake() {
			RecalculateMatrices();
		}

		private void Update() {
			RecalculateMatrices();
		}

		private void RecalculateMatrices() {
			ltw = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			wtl = ltw.inverse;
		}

		public Vector3 SnapPosition(Vector3 pos) {
			pos = wtl.MultiplyPoint3x4(pos);
			pos.y = 0;
			pos = ((pos / snap).Flat().Round() * snap).Unflat(0);
			pos = ltw.MultiplyPoint3x4(pos);
			return pos;
		}

		public Quaternion SnapRotation(Quaternion rot) {
			Vector3 euler = rot.eulerAngles;
			euler.y -= transform.rotation.eulerAngles.y;
			euler.y = Mathf.Round(euler.y / 90f) * 90f;
			euler.y += transform.rotation.eulerAngles.y;
			return Quaternion.Euler(euler);
		}

	}
}
