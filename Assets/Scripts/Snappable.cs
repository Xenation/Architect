using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	[RequireComponent(typeof(Throwable))]
	public abstract class Snappable : MonoBehaviour {

		public RoomNetwork roomnet;

		protected bool showPreview = false;
		protected bool isSnapped = false;

		protected GameObject preview;
		protected Throwable throwable;
		protected new Rigidbody rigidbody;

		protected virtual void Awake() {
			preview = CreatePreview();
			preview.SetActive(false);

			throwable = GetComponent<Throwable>();
			throwable.onPickUp.AddListener(PickedUp);
			throwable.onDetachFromHand.AddListener(Detached);

			rigidbody = GetComponent<Rigidbody>();
		}

		protected virtual void OnDestroy() {
			throwable.onPickUp.AddListener(PickedUp);
			throwable.onDetachFromHand.AddListener(Detached);
		}

		protected abstract GameObject CreatePreview();

		protected virtual void PickedUp() {
			showPreview = true;
			if (isSnapped) {
				Unsnapped();
			}
		}

		protected virtual void Detached() {
			if (preview.activeInHierarchy) { // Has a valid snap point -> Snap
				transform.position = preview.transform.position;
				transform.rotation = preview.transform.rotation;
				rigidbody.isKinematic = true;
				Snapped();
				DisablePreview();
			} else { // Re-enable physics
				rigidbody.isKinematic = false;
			}
			showPreview = false;
		}

		protected virtual void Snapped() {
			isSnapped = true;
		}

		protected virtual void Unsnapped() {
			isSnapped = false;
		}

		protected virtual void EnablePreview() {
			preview.SetActive(true);
		}

		protected virtual void DisablePreview() {
			preview.SetActive(false);
		}

	}
}
