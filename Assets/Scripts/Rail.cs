using UnityEngine;

namespace Architect {
	public class Rail : MonoBehaviour {

		public Transform point1;
		public Transform point2;
		public Transform debugProject;

		public Rigidbody constrained;

		private void FixedUpdate() {
			if (constrained != null) {
				// Put Back On Rail if to far
				Vector3 projPos = Project(constrained.position);
				if (Vector3.Distance(projPos, constrained.position) > 0.00001f) {
					constrained.position = projPos;
				}
				// Constrain Velocity to rail
				Vector3 vel = constrained.velocity;
				vel = Constrain(constrained.position, vel);
				constrained.velocity = vel;
			}
		}

		private void OnDrawGizmos() {
			if (point1 == null || point2 == null) return;
			Color tmpCol = Gizmos.color;
			
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.TransformPoint(point1.localPosition), transform.TransformPoint(point2.localPosition));
			if (debugProject != null) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(Project(debugProject.position), 0.025f);
			}

			Gizmos.color = tmpCol;
		}

		public Vector3 Project(Vector3 pos) {
			if (point1 == null || point2 == null) return Vector3.zero;
			pos = transform.InverseTransformPoint(pos);
			Vector3 line = point2.localPosition - point1.localPosition;
			Vector3 dir = line.normalized;
			Vector3 projPos = pos - point1.localPosition;
			projPos = Vector3.Project(projPos, dir);
			float dot = Vector3.Dot(line, projPos);
			if (dot < 0f) {
				projPos = Vector3.zero;
			} else if (dot > line.sqrMagnitude) {
				projPos = line;
			}
			return transform.TransformPoint(point1.localPosition + projPos);
		}

		public Vector3 Constrain(Vector3 pos, Vector3 vec) {
			if (point1 == null || point2 == null) return vec;
			Vector3 projPos = Project(pos);
			return Project(pos + vec) - projPos;
		}

	}
}
