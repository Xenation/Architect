using UnityEngine;

namespace Architect {
	public class ProgressingElement : MonoBehaviour {

		public float targetProgress = 0f;
		public float speed = 1f;

		private float _progress = 0f;
		public virtual float progress {
			get {
				return _progress;
			}
			set {
				_progress = value;
			}
		}

		private bool _reverse;
		public virtual bool reverse {
			get {
				return _reverse;
			}
			set {
				_reverse = value;
			}
		}

		public delegate void NotifyCallback(ProgressingElement path);
		public event NotifyCallback HasReachedTarget;

		protected virtual void Update() {
			if (targetProgress > progress) {
				progress += speed * Time.deltaTime;
				if (progress > targetProgress) { // At target
					progress = targetProgress;
					TriggerReachedTarget();
				}
			} else if (targetProgress < progress) {
				progress -= speed * Time.deltaTime;
				if (progress < targetProgress) { // At target
					progress = targetProgress;
					TriggerReachedTarget();
				}
			}
		}

		protected void TriggerReachedTarget() {
			HasReachedTarget?.Invoke(this);
		}

	}
}
