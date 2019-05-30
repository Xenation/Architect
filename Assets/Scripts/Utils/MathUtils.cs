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

		public static Vector3 Flat3(this Vector3 v) {
			return new Vector3(v.x, 0, v.z);
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

		// COLLIDERS
		public static bool IsInside(this BoxCollider box, Vector3 point) {
			point = box.transform.InverseTransformPoint(point) - box.center;
			float halfX = box.size.x * .5f;
			float halfY = box.size.y * .5f;
			float halfZ = box.size.z * .5f;
			if (point.x < halfX && point.x > -halfX &&
				point.y < halfY && point.y > -halfY &&
				point.z < halfZ && point.z > -halfZ) {
				return true;
			}
			return false;
		}

	}

	[System.Serializable]
	public struct Recti {
		public Vector2Int min;
		public Vector2Int max;

		public Vector2 center {
			get {
				return min + max.Float() / 2f;
			}
		}

		public Recti(Vector2Int min, Vector2Int max) {
			this.min = min;
			this.max = max;
		}

		public bool Contains(Vector2Int pos) {
			return !(pos.x < min.x || pos.y < min.y || pos.x > max.x || pos.y > max.y);
		}

		public bool Contains(Recti rect) {
			return !(rect.min.x < min.x || rect.min.y < min.y || rect.max.x > max.x || rect.max.y > max.y);
		}

		public bool Intersects(Recti rect) {
			return !(rect.min.x > max.x || rect.max.x < min.x || rect.min.y > max.y || rect.max.y < min.y);
		}

		public float Distance(Recti rect) {
			if (Intersects(rect)) {
				return 0f;
			}
			// Corners
			Vector2Int localCorner;
			Vector2Int otherCorner;
			if (rect.min.x > max.x && rect.min.y > max.y) { // Top right
				localCorner = max;
				otherCorner = rect.min;
				return (otherCorner - localCorner).magnitude;
			}
			if (rect.max.x < min.x && rect.min.y > max.y) { // Top left
				localCorner = new Vector2Int(min.x, max.y);
				otherCorner = new Vector2Int(rect.max.x, rect.min.y);
				return (otherCorner - localCorner).magnitude;
			}
			if (rect.max.x < min.x && rect.max.y < min.y) { // Bottom left
				localCorner = min;
				otherCorner = rect.max;
				return (otherCorner - localCorner).magnitude;
			}
			if (rect.min.x > max.x && rect.max.y < min.y) { // Bottom right
				localCorner = new Vector2Int(max.x, min.y);
				otherCorner = new Vector2Int(rect.min.x, rect.max.y);
				return (otherCorner - localCorner).magnitude;
			}
			// Sides
			if (rect.min.x > max.x) { // Right
				return rect.min.x - max.x;
			}
			if (rect.max.x < min.x) { // Left
				return min.x - rect.max.x;
			}
			if (rect.min.y > max.y) { // Top
				return rect.min.y - max.y;
			}
			if (rect.max.y < min.y) { // Bottom
				return min.y - rect.max.y;
			}

			return 0f; // Should not be possible
		}

		public bool Constrain(Recti bounds, out Recti constrained) {
			int width = max.x - min.x;
			int height = max.y - min.y;
			if (bounds.max.x - bounds.min.x < width || bounds.max.y - bounds.min.y < height) {
				constrained = this;
				return false;
			}
			constrained = Constrain(bounds);

			return true;
		}

		public Recti Constrain(Recti bounds) { // Unchecked constrain
			Recti constrained = this;
			int offset = 0;
			if (constrained.min.x < bounds.min.x) { // Too left
				offset = bounds.min.x - constrained.min.x;
			}
			if (constrained.max.x > bounds.max.x) { // Too right
				offset = bounds.max.x - constrained.max.x;
			}
			constrained.min.x += offset;
			constrained.max.x += offset;

			offset = 0;
			if (constrained.min.y < bounds.min.y) { // Too low
				offset = bounds.min.y - constrained.min.y;
			}
			if (constrained.max.y > bounds.max.y) { // Too high
				offset = bounds.max.y - constrained.max.y;
			}
			constrained.min.y += offset;
			constrained.max.y += offset;

			return constrained;
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

		public bool Contains(Boxi box) {
			return !(box.min.x < min.x || box.min.y < min.y || box.min.z < min.z || box.max.x > max.x || box.max.y > max.y || box.max.z > max.z);
		}
	}
}
