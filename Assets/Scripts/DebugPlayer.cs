using UnityEngine;

namespace Architect {
	public class DebugPlayer : MonoBehaviour {

		public float speed = 1f;
		public float fastSpeed = 2f;
		public float mouseSensivity = 10f;

		public void Update() {
			Vector3 input;
			input.x = Input.GetAxisRaw("Horizontal");
			input.z = Input.GetAxisRaw("Depth");
			input.y = Input.GetAxisRaw("Vertical");

			input = transform.forward * input.z + transform.right * input.x + transform.up * input.y;
			if (input.x != 0 && input.y != 0 && input.z != 0) {
				input.Normalize();
				input *= (Input.GetKey(KeyCode.LeftShift)) ? fastSpeed : speed;
			}

			transform.position += input * Time.deltaTime;

			if (Cursor.lockState == CursorLockMode.None) {
				if (Input.GetMouseButtonDown(1)) {
					Cursor.lockState = CursorLockMode.Locked;
				}
			} else {
				if (Input.GetKeyDown(KeyCode.Escape)) {
					Cursor.lockState = CursorLockMode.None;
				}
			}

			if (Cursor.lockState == CursorLockMode.Locked) {
				Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
				mouseInput *= mouseSensivity;
				Vector3 currentEuler = transform.rotation.eulerAngles;
				transform.rotation = Quaternion.Euler(currentEuler.x - mouseInput.y, currentEuler.y + mouseInput.x, currentEuler.z);
			}

		}

	}
}
