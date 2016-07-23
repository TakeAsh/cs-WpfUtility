using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using TakeAshUtility;

namespace WpfUtility {

    [TypeConverter(typeof(EnumTypeConverter<PrintOutputColor>))]
    public enum PrintOutputColor {

        [EnumProperty(PrintOutputColorHelper.ExPropIsUsual + ":'false'")]
        Unknown,

        [EnumProperty(PrintOutputColorHelper.ExPropIsUsual + ":'true'")]
        Color,

        [EnumProperty(PrintOutputColorHelper.ExPropIsUsual + ":'false'")]
        Grayscale,

        [EnumProperty(PrintOutputColorHelper.ExPropIsUsual + ":'true'")]
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
            get { return EnumHelper.GetValues<PrintOutputColor>(); }
        }

        public static PrintOutputColor[] Values {
            get { return _values; }
        }

        public static bool IsUsual(this PrintOutputColor color) {
            return color.GetEnumProperty(ExPropIsUsual).TryParse<bool>();
        }

        public static System.Printing.OutputColor ToSystemOutputColor(this PrintOutputColor color) {
            return color.ToString().TryParse<System.Printing.OutputColor>();
        }
    }
}
