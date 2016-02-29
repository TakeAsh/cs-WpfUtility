using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using TakeAsh;
using TakeAshUtility;

namespace WpfUtility {

    using _PrintOutputColorHelper = EnumHelper<PrintOutputColor>;

    [TypeConverter(typeof(EnumTypeConverter<PrintOutputColor>))]
    public enum PrintOutputColor {

        [ExtraProperties(PrintOutputColorHelper.ExPropIsUsual + ":'false'")]
        Unknown,

        [ExtraProperties(PrintOutputColorHelper.ExPropIsUsual + ":'true'")]
        Color,

        [ExtraProperties(PrintOutputColorHelper.ExPropIsUsual + ":'false'")]
        Grayscale,

        [ExtraProperties(PrintOutputColorHelper.ExPropIsUsual + ":'true'")]
        Monochrome,
    }

    public static class PrintOutputColorHelper {

        public const string ExPropIsUsual = "IsUsual";

        private static PrintOutputColor[] _values = AllValues.Where(color => color.IsUsual())
            .SafeToArray();

        public static PrintOutputColor Default {
            get { return PrintOutputColor.Color; }
        }

        public static PrintOutputColor[] AllValues {
            get { return _PrintOutputColorHelper.Values; }
        }

        public static PrintOutputColor[] Values {
            get { return _values; }
        }

        public static bool IsUsual(this PrintOutputColor color) {
            return color.GetExtraProperty(ExPropIsUsual).TryParse<bool>();
        }

        public static System.Printing.OutputColor ToSystemOutputColor(this PrintOutputColor color) {
            return color.ToString().TryParse<System.Printing.OutputColor>();
        }
    }
}
