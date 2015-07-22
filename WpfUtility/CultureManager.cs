using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using TakeAshUtility;

namespace WpfUtility {

    public static class CultureManager {

        /// <summary>
        /// Culture used when specified culture is not available
        /// </summary>
        /// <remarks>
        /// Invariant culture is treated as this culture
        /// </remarks>
        public const string DefaultCultureName = "en-US";

        public static readonly CultureInfo DefaultCulture = new CultureInfo(DefaultCultureName);

        private const int InvariantCultureID = 0x007f;

        private static List<CultureInfo> _availableCultures;

        /// <summary>
        /// Available cultures in CallingAssembly
        /// </summary>
        /// <remarks>
        /// [c# - Programmatic way to get all the available languages (in satellite assemblies) - Stack Overflow](http://stackoverflow.com/questions/553244)
        /// </remarks>
        public static List<CultureInfo> AvailableCultures {
            get {
                if (_availableCultures == null) {
                    var resourceManager = ResourceHelper.GetResourceManager(Assembly.GetCallingAssembly());
                    _availableCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                        .Where(cultureInfo => cultureInfo.LCID != InvariantCultureID)
                        .Where(cultureInfo => resourceManager.GetResourceSet(cultureInfo, true, false) != null)
                        .Union(new[] { DefaultCulture })
                        .OrderBy(cultureInfo => cultureInfo.EnglishName)
                        .ToList();
                }
                return _availableCultures;
            }
        }

        /// <summary>
        /// Return CultureInfo specified by culture name in AvailableCultures
        /// </summary>
        /// <param name="cultureName">culture name in the format languagecode2-country/regioncode2</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>cultureName is null or empty</term><description>culture name of current thread is used instead of cultureName</description></item>
        /// <item><term>cultureName exists in AvailableCultures</term><description>specified CultureInfo</description></item>
        /// <item><term>cultureName doesn't exist in AvailableCultures</term><description>DefaultCulture</description></item>
        /// </list>
        /// </returns>
        public static CultureInfo GetCulture(string cultureName) {
            if (String.IsNullOrEmpty(cultureName)) {
                cultureName = Thread.CurrentThread.CurrentCulture.Name;
            }
            return AvailableCultures
                .Where(cultureInfo => cultureInfo.Name == cultureName)
                .FirstOrDefault() ?? DefaultCulture;
        }

        /// <summary>
        /// Set culture for current thread
        /// </summary>
        /// <param name="cultureName">culture name</param>
        public static void SetCulture(string cultureName) {
            var culture = GetCulture(cultureName);
            if (Thread.CurrentThread.CurrentCulture.Name == culture.Name) {
                return;
            }
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
