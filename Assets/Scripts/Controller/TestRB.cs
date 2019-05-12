using UnityEngine;

namespace Architect {
	public class TestRB : MonoBehaviour {

		public float speed;

		private Rigidbody rb;

		private void Awake() {
			rb = GetComponent<Rigidbody>();
		}

		private void FixedUpdate() {
			if (Input.GetKey(KeyCode.Z)) {
				//rb.MovePosition(rb.position + Vector3.forward * speed * Time.fixedDeltaTime);
				rb.position += Vector3.forward * speed * Time.fixedDeltaTime;
			}
			if (Input.GetKey(KeyCode.S)) {
				//rb.MovePosition(rb.position - Vector3.forward * speed * Time.fixedDeltaTime);
				rb.position -= Vector3.forward * speed * Time.fixedDeltaTime;
			}
		}

	}
}
