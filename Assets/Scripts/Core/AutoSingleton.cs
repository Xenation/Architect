namespace Architect {
	public class AutoSingleton<T> where T : new() {

		private static T instance;
		public static T I {
			get {
				if (instance == null) {
					instance = new T();
				}
				return instance;
			}
		}

	}
}
