using UnityEngine;

namespace Architect {
	public static class MathUtils {

		// VECTOR2
		public static Vector2 Ceil(this Vector2 v) {
			return new Vector2(Mathf.Ceil(v.x), Mathf.Ceil(v.y));
		}
		public static Vector2Int CeilToInt(this Vector2 v) {
			return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
		}

		public static Vector2 Floor(this Vector2 v) {
			return new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
		}
		public static Vector2Int FloorToInt(this Vector2 v) {
			return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
		}

		public static Vector2 Round(this Vector2 v) {
			return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
		}
		public static Vector2Int RoundToInt(this Vector2 v) {
			return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
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

		// VECTOR2INT
		public static Vector2Int Clamp(this Vector2Int v, int min, int max) {
			return new Vector2Int((v.x < min) ? min : ((v.x > max) ? max : v.x), (v.y < min) ? min : ((v.y > max) ? max : v.y));
		}
		public static Vector2Int Clamp(this Vector2Int v, Recti rect) {
			return new Vector2Int((v.x < rect.min.x) ? rect.min.x : ((v.x > rect.max.x) ? rect.max.x : v.x), (v.y < rect.min.y) ? rect.min.y : ((v.y > rect.max.y) ? rect.max.y : v.y));
		}
		
		public static Vector3Int Unflat(this Vector2Int v, int y) {
			return new Vector3Int(v.x, y, v.y);
		}
		public static Vector2 Float(this Vector2Int v) {
			return new Vector2(v.x, v.y);
		}

		// VECTOR3INT
		public static Vector3Int Clamp(this Vector3Int v, int min, int max) {
			return new Vector3Int((v.x < min) ? min : ((v.x > max) ? max : v.x), (v.y < min) ? min : ((v.y > max) ? max : v.y), (v.z < min) ? min : ((v.z > max) ? max : v.z));
		}
		public static Vector3Int Clamp(this Vector3Int v, Boxi box) {
			return new Vector3Int((v.x < box.min.x) ? box.min.x : ((v.x > box.max.x) ? box.max.x : v.x), (v.y < box.min.y) ? box.min.y : ((v.y > box.max.y) ? box.max.y : v.y), (v.z < box.min.z) ? box.min.z : ((v.z > box.max.z) ? box.max.z : v.z));
		}

		public static Vector2Int Flat(this Vector3Int v) {
			return new Vector2Int(v.x, v.z);
		}
		public static Vector3 Float(this Vector3Int v) {
			return new Vector3(v.x, v.y, v.z);
		}

	}

	[System.Serializable]
	public struct Recti {
		public Vector2Int min;
		public Vector2Int max;

		public Recti(Vector2Int min, Vector2Int max) {
			this.min = min;
			this.max = max;
		}

		public bool Contains(Vector2Int pos) {
			return !(pos.x < min.x || pos.y < min.y || pos.x > max.x || pos.y > max.y);
		}
	}

	public struct Boxi {
		public Vector3Int min;
		public Vector3Int max;

		public Boxi(Vector3Int min, Vector3Int max) {
			this.min = min;
			this.max = max;
		}

		public bool Contains(Vector3Int pos) {
			return !(pos.x < min.x || pos.y < min.y || pos.z < min.z || pos.x > max.x || pos.y > max.y || pos.z > max.z);
		}
	}
}
