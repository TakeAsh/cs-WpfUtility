using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using TakeAshUtility;

namespace WpfUtility {

    [TypeConverter(typeof(EnumTypeConverter<PageOrientation>))]
    public enum PageOrientation {
        Portrait,
        Landscape,
    }

    public static class PageOrientationHelper {

        public static PageOrientation Default {
            get { return default(PageOrientation); }
        }

        public static PageOrientation[] Values {
            get { return EnumHelper.GetValues<PageOrientation>(); }
        }

        public static System.Printing.PageOrientation ToSystemPageOrientation(this PageOrientation orientation) {
            return orientation.ToString().TryParse<System.Printing.PageOrientation>();
        }
    }
}
