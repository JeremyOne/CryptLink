using System;
using System.Text;
using System.Linq;

namespace CryptLink {

	public class AppVersionInfo {

		/// <summary>
		/// The name of this application
		/// </summary>
		public string Name { get; set; }  

		/// <summary>
		/// The version of this application
		/// </summary>
		/// <value>The version.</value>
		public Version Version { get; set; }

		/// <summary>
		/// The latest version of the CryptLink API implemented
		/// </summary>
		public Version ApiVersion { get; set; }

		/// <summary>
		/// The oldest version of the CryptLink API that is supported by this application
		/// </summary>
		public Version ApiCompartibilityVersion { get; set; }

    }
}

