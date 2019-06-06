using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class ModelRotator : MonoBehaviour {
		
		public LinearMapping rotationMapping;

		internal new Rigidbody rigidbody;
		private float prevRotation;
		private float deltaRotation;
        private float rotationFloatSound;

		private Transform modelRoot;

		private void Awake() {
			prevRotation = rotationMapping.value;
			deltaRotation = 0f;

			rigidbody = GetComponent<Rigidbody>();
			modelRoot = transform.parent;
			transform.SetParent(null, true);
		}

		private void Update() {
			deltaRotation = rotationMapping.value - prevRotation;
			prevRotation = rotationMapping.value;

			modelRoot.rotation = transform.rotation;

            // TODO send rotationFloatSound to Wwise
            AkSoundEngine.SetRTPCValue("RTPC_Table", rotationFloatSound);
            //print(rotationFloatSound);
            rotationFloatSound = deltaRotation * 1000;
        }

		private void FixedUpdate() {
			Quaternion rotation = rigidbody.rotation;
			Quaternion nRotation = rotation * Quaternion.Euler(0f, deltaRotation * Mathf.PI * 2 * Mathf.Rad2Deg, 0f);
			
			rigidbody.MoveRotation(nRotation);
			modelRoot.rotation = nRotation;
		}

	}
}
