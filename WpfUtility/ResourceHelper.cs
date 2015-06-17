using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Media.Imaging;

namespace WpfUtility {

    public static class ResourceHelper {

        public static BitmapImage GetImage(string filename) {
            if (filename == null) {
                return null;
            }
            var assembly = Assembly.GetCallingAssembly().GetName().Name;
            return new BitmapImage(new Uri("/" + assembly + ";component/" + filename, UriKind.Relative));
        }

        /// <summary>
        /// Get text of embedded resource file
        /// </summary>
        /// <param name="filename">resource file name</param>
        /// <param name="encoding">encoding of file content. Default is UTF8.</param>
        /// <returns>content of resource file</returns>
        /// <remarks>The resource file must be embedded.</remarks>
        public static string GetText(string filename, Encoding encoding = null) {
            if (filename == null) {
                return null;
            }
            encoding = encoding ?? Encoding.UTF8;
            var assembly = Assembly.GetCallingAssembly();
            var directory = Path.GetDirectoryName(filename).Replace('-', '_');
            var file = Path.GetFileName(filename);
            var resourceName = Path.Combine(assembly.GetName().Name, directory, file).Replace('\\', '.').Replace('/', '.');
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream, encoding)) {
                return reader.ReadToEnd();
            }
        }

        public static ResourceManager GetResourceManager(Object obj) {
            Assembly assembly;
            if (obj == null ||
                (assembly = obj.GetType().Assembly) == null) {
                return null;
            }
            var resourceName = assembly.GetName().Name + ".Properties.Resources";
            if (!assembly.GetManifestResourceNames().Contains(resourceName + ".resources")) {
                return null;
            }
            return new ResourceManager(resourceName, assembly);
        }
    }
}
