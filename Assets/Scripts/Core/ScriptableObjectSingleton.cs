using UnityEngine;

namespace Architect {
	public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject {

		protected static T instance = null;
		public static T I {
			get {
				if (instance == null) {
					Debug.LogWarning("Accessing " + typeof(T).Name + " instance before assignement!");
				}
				return instance;
			}
		}

	}
}
