using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace WpfUtility {

    using FontFamilyPair = KeyValuePair<FontFamily, string>;
    using FontFamilyDic = Dictionary<string, KeyValuePair<FontFamily, string>>;

    public static class FontFamilyHelper {

        private static readonly XmlLanguage _defaultLang = XmlLanguage.GetLanguage("en-us");
        private static readonly string _systemFontFamilyName = SystemFonts.MessageFontFamily.FamilyNames[_defaultLang];
        private static XmlLanguage _currentLang;
        private static FontFamilyPair[] _fontFamilyPairs;
        private static FontFamilyDic _fontFamilyDic;

        /// <summary>
        /// The pairs of the FontFamily and the localized name.
        /// </summary>
        /// <remarks>
        /// This can be used for ItemsSource of a ComboBox.
        /// </remarks>
        public static FontFamilyPair[] FontFamilyPairs {
            get { return _fontFamilyPairs ?? (_fontFamilyPairs = PrepareFontFamilyPairs()); }
        }

        /// <summary>
        /// The Dictionary of the FontFamily name in the default language and the FontFamilyPair.
        /// </summary>
        /// <remarks>
        /// This is used for searching in GetFontFamilyPair().
        /// </remarks>
        public static FontFamilyDic FontFamilyDic {
            get { return _fontFamilyDic ?? (_fontFamilyDic = PrepareFontFamilyDic()); }
        }

        /// <summary>
        /// Get the FontFamily name in the default language.
        /// </summary>
        /// <param name="pair">The FontFamilyPair.</param>
        /// <returns>The FontFamily name in the default language.</returns>
        public static string GetDefaultFamilyName(this FontFamilyPair pair) {
            var names = pair.Key.FamilyNames;
            return names.ContainsKey(_defaultLang) ?
                names[_defaultLang] :
                names.FirstOrDefault().Value;
        }

        /// <summary>
        /// Get FontFamilyPair according to the FontFamily name in the default language.
        /// </summary>
        /// <param name="defaultFamilyName">The FontFamily name in the default language.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item>The FontFamilyPair with the defaultFamilyName, if it exist.</item>
        /// <item>The System FontFamily pair, if the defaultFamilyName is null or default, or the FontFamily does not exist.</item>
        /// </list>
        /// </returns>
        public static FontFamilyPair GetFontFamilyPair(string defaultFamilyName = null) {
            if (String.IsNullOrEmpty(defaultFamilyName)) {
                defaultFamilyName = _systemFontFamilyName;
            }
            return FontFamilyDic.ContainsKey(defaultFamilyName) ?
                FontFamilyDic[defaultFamilyName] :
                FontFamilyDic[_systemFontFamilyName];
        }

        /// <summary>
        /// Get FontFamily according to the FontFamily name in the default language.
        /// </summary>
        /// <param name="defaultFamilyName">The FontFamily name in the default language.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item>The FontFamily with the defaultFamilyName, if it exist.</item>
        /// <item>The System FontFamily, if the defaultFamilyName is null or default, or the FontFamily does not exist.</item>
        /// </list>
        /// </returns>
        public static FontFamily GetFontFamily(string defaultFamilyName = null) {
            return GetFontFamilyPair(defaultFamilyName).Key;
        }

        /// <summary>
        /// Get the FontFamily name in the current language.
        /// </summary>
        /// <param name="fontFamily">The FontFamily.</param>
        /// <returns>The FontFamily name in the current language.</returns>
        public static string GetFontFamilyName(this FontFamily fontFamily) {
            if (fontFamily == null) {
                return null;
            }
            var names = fontFamily.FamilyNames;
            return _currentLang != null && names.ContainsKey(_currentLang) ? names[_currentLang] :
                names.ContainsKey(_defaultLang) ? names[_defaultLang] :
                names.FirstOrDefault().Value;
        }

        /// <summary>
        /// Prepare the FontFamilyPairs from the installed fonts.
        /// </summary>
        /// <returns>The FontFamilyPairs.</returns>
        private static FontFamilyPair[] PrepareFontFamilyPairs() {
            _currentLang = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            return Fonts.SystemFontFamilies
                .Select(family => new FontFamilyPair(family, family.GetFontFamilyName()))
                .OrderBy(pair => pair.Value)
                .ToArray();
        }

        /// <summary>
        /// Prepare the FontFamilyDic from the FontFamilyPairs.
        /// </summary>
        /// <returns>The FontFamilyDic.</returns>
        private static FontFamilyDic PrepareFontFamilyDic() {
            return FontFamilyPairs.ToDictionary(pair => pair.GetDefaultFamilyName(), pair => pair);
        }
    }
}
