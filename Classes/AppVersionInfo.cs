using System;

namespace CryptLink {

	public class AppVersionInfo : Hashable {

		/// <summary>
		/// The name of this application
		/// </summary>
		public string Name { get; private set;}  

		/// <summary>
		/// The version of this application
		/// </summary>
		/// <value>The version.</value>
		public Version Version { get; private set; }

		/// <summary>
		/// The latest version of the CryptLink API implemented
		/// </summary>
		public Version ApiVersion { get; private set; }

		/// <summary>
		/// The oldest version of the CryptLink API that is supported by this application
		/// </summary>
		public Version ApiCompartibilityVersion { get; private set; }
	}
}

