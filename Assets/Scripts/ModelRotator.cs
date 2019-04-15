using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class ModelRotator : MonoBehaviour {

		public float speed = 5f;

		private void Update() {
			float rotInput = 0f;
			if (Input.GetKey(KeyCode.C)) {
				rotInput -= 1f;
			}
			if (Input.GetKey(KeyCode.V)) {
				rotInput += 1f;
			}
			
			transform.rotation = Quaternion.Euler(0f, rotInput * speed * Time.deltaTime, 0f) * transform.rotation;
		}

	}
}
