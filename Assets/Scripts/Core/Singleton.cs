using UnityEngine;

namespace Architect {
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

		private static T instance = null;
		public static T I {
			get {
				if (instance == null) {
					instance = FindObjectOfType<T>();
					if (instance == null) {
						Debug.LogWarning("Singleton<" + typeof(T).Name + ">: Unable to find an instance");
					}
				}
				return instance;
			}
		}

	}
}
