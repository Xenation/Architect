using UnityEngine;

namespace Architect {
	public class Snapable : MonoBehaviour {

		public Vector2Int size;
		public SnapGrid grid;
		public Transform reference;

		//private void Update() {
		//	Recti gridRect = grid.WorldToGrid(reference.transform.position, size);
		//	Recti snappedRect = grid.SnapRectangle(gridRect);

		//	Vector3 nPos;
		//	grid.GridToWorld(snappedRect, out nPos, out size);
		//	transform.position = nPos;

		//	transform.rotation = grid.SnapRotation(reference.rotation);
		//}

	}
}
