using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class Hint : MonoBehaviour {

		public enum InteractBehaviour {
			Cancel,
			Next
		}

		[Tooltip("Whether this hint is triggered on awake")]
		public bool startOnAwake = false;
		[Tooltip("The outlined object to hint at")]
		public Outlined outlined;
		[Tooltip("A delay before the start of the hint")]
		public float startDelay;
		[Tooltip("The duration during which the hint is showed (0 to keep forever)")]
		public float hintDuration;
		[Tooltip("The time between two blinks")]
		public float blinkDuration;
		[Tooltip("The portion of time the blink is off in the blink duration")]
		public float offPortion;
		[Tooltip("The color of blinks")]
		public Color blinkColor;
		[Tooltip("What to do when the player interacts with the hinted object")]
		public InteractBehaviour interactBehaviour;
		[Tooltip("The Hints to trigger after the current one")]
		public List<Hint> nexts;

		[System.NonSerialized]
		public bool started = false;
		[System.NonSerialized]
		public bool finished = false;
		private float startTime = 0f;
		private float hintStartTime = 0f;
		private bool prevOn = false;

		public void Awake() {
			if (outlined == null) {
				outlined = GetComponent<Outlined>();
			}
			started = startOnAwake;
		}

		public void Update() {
			if (!started || finished) return;

			if (startTime == 0f) {
				startTime = Time.time;
			}

			hintStartTime = startTime + startDelay;
			float time = Time.time - hintStartTime;
			if (time < 0) {
				return;
			} else if (hintDuration > 0 && time > hintDuration) {
				Finish();
				return;
			}

			float loopedTime = time % blinkDuration;
			float offTime = blinkDuration * offPortion;
			if (loopedTime < offTime / 2f || loopedTime > blinkDuration - offTime / 2f) { // Off
				if (prevOn) {
					outlined.DisableHighlight(-1);
				}
				prevOn = false;
			} else { // On
				float onProgress = (loopedTime - offTime / 2f) / (blinkDuration - offTime);
				float blinkStrength = (onProgress > .5f) ? 1 - onProgress * 2f + 1 : onProgress * 2f;
				Color actualBlinkColor = blinkColor;
				actualBlinkColor.a = blinkStrength;

				if (!prevOn) {
					outlined.EnableHighlight(-1, actualBlinkColor);
				}
				outlined.UpdateHighlight(-1, actualBlinkColor);

				prevOn = true;
			}
		}

		private void Finish() {
			finished = true;
			if (prevOn) {
				outlined.DisableHighlight(-1);
			}
			foreach (Hint hint in nexts) {
				hint.started = true;
			}
		}

		private void Cancel() {
			finished = true;
			if (prevOn) {
				outlined.DisableHighlight(-1);
			}
		}
		
		private void OnHintInteraction() {
			switch (interactBehaviour) {
				case InteractBehaviour.Cancel:
					Cancel();
					break;
				case InteractBehaviour.Next:
					Finish();
					break;
			}
		}

	}
}
