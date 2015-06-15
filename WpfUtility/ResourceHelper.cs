using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
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

        private static Regex regPropertyResources = new Regex(@"\.Properties\.");
        private static Regex regLastResources = new Regex(@"\.resources$");

        public static ResourceManager GetResourceManager(Object obj) {
            Assembly assembly;
            string[] resNames;
            ResourceManager resourceManager;
            if (obj == null ||
                (assembly = obj.GetType().Assembly) == null ||
                (resNames = assembly.GetManifestResourceNames()) == null ||
                resNames.Length == 0 ||
                (resNames = resNames.Where(name => regPropertyResources.IsMatch(name)).ToArray()) == null ||
                resNames.Length == 0 ||
                (resourceManager = new ResourceManager(regLastResources.Replace(resNames[0], ""), assembly)) == null) {
                return null;
            }
            return resourceManager;
        }
    }
}
