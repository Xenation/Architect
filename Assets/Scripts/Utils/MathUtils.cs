using UnityEngine;

namespace Architect {
	public static class MathUtils {

		// VECTOR2
		public static Vector2 Ceil(this Vector2 v) {
			return new Vector2(Mathf.Ceil(v.x), Mathf.Ceil(v.y));
		}

		public static Vector2 Floor(this Vector2 v) {
			return new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
		}

		public static Vector2 Round(this Vector2 v) {
			return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
		}

		public static Vector3 Unflat(this Vector2 v, float y) {
			return new Vector3(v.x, y, v.y);
		}

		// VECTOR3
		public static Vector3 Ceil(this Vector3 v) {
			return new Vector3(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z));
		}

		public static Vector3 Floor(this Vector3 v) {
			return new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
		}

		public static Vector3 Round(this Vector3 v) {
			return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
		}

		public static Vector2 Flat(this Vector3 v) {
			return new Vector2(v.x, v.z);
		}

	}
}
