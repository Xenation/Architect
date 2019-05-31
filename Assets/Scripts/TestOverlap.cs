using UnityEngine;

namespace Architect {
	public class TestOverlap : MonoBehaviour {

		public LayerMask mask;
		public float radius = 0.01f;

		private Collider[] colliders = new Collider[16];

		private void OnDrawGizmos() {
			Color tmpCol = Gizmos.color;

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, radius);
			int colCount = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, mask.value);

			Gizmos.color = Color.yellow;
			for (int i = 0; i < colCount; i++) {
				Debug.Log("Overlap[" + i + "]: " + colliders[i].gameObject.name);
			}

			Gizmos.color = tmpCol;
		}

	}
}
