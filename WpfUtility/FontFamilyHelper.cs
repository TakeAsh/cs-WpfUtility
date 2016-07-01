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

    public static class FontFamilyHelper {

        private static readonly XmlLanguage _defaultLang = XmlLanguage.GetLanguage("en-us");
        private static XmlLanguage _currentLang;
        private static FontFamilyPair[] _fontFamilyPairs;

        public static FontFamilyPair[] FontFamilyPairs {
            get { return _fontFamilyPairs ?? (_fontFamilyPairs = PrepareFontFamilyPairs()); }
        }

        public static string GetDefaultFamilyName(this FontFamilyPair pair) {
            var names = pair.Key.FamilyNames;
            return names.ContainsKey(_defaultLang) ?
                names[_defaultLang] :
                names.FirstOrDefault().Value;
        }

        public static FontFamilyPair GetFontFamilyPair(string defaultFamilyName = null) {
            if (String.IsNullOrEmpty(defaultFamilyName)) {
                defaultFamilyName = SystemFonts.MessageFontFamily.FamilyNames[_defaultLang];
            }
            return FontFamilyPairs.FirstOrDefault(pair => pair.GetDefaultFamilyName() == defaultFamilyName);
        }

        public static FontFamily GetFontFamily(string defaultFamilyName = null) {
            return GetFontFamilyPair(defaultFamilyName).Key;
        }

        public static string GetFontFamilyName(this FontFamily fontFamily) {
            if (fontFamily == null) {
                return null;
            }
            var names = fontFamily.FamilyNames;
            return _currentLang != null && names.ContainsKey(_currentLang) ? names[_currentLang] :
                names.ContainsKey(_defaultLang) ? names[_defaultLang] :
                names.FirstOrDefault().Value;
        }

        private static FontFamilyPair[] PrepareFontFamilyPairs() {
            _currentLang = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            return Fonts.SystemFontFamilies
                .Select(family => new FontFamilyPair(family, family.GetFontFamilyName()))
                .OrderBy(pair => pair.Value)
                .ToArray();
        }
    }
}
