using UnityEngine;

namespace Architect {
	public class AutoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour {
		
		private static T instance;
		public static T I {
			get {
				if (instance == null) {
					instance = CreateInstance();
				}
				return instance;
			}
		}

		private bool initialized = false;

		protected static T CreateInstance() {
			GameObject go = new GameObject(typeof(T).Name);
			T inst = go.AddComponent<T>();
			AutoBehaviourSingleton<T> autoInst = inst as AutoBehaviourSingleton<T>;
			if (autoInst != null) {
				autoInst.Initialize();
				autoInst.initialized = true;
			}
			return inst;
		}

		protected virtual void Initialize() { }

		private void Awake() {
			if (!initialized) return;
			Initialize();
			initialized = true;
		}

	}
}
