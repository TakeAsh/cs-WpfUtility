using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using TakeAsh;
using TakeAshUtility;

namespace WpfUtility {

    using _PageMediaSizeHelper = EnumHelper<PageMediaSize>;

    [TypeConverter(typeof(EnumTypeConverter<PageMediaSize>))]
    public enum PageMediaSize {

        [ExtraProperties(
            PageMediaSizeHelper.ExPropPageMediaSizeName + ":'ISOA3', " +
            PageMediaSizeHelper.ExPropWidth + ":'297', " +
            PageMediaSizeHelper.ExPropHeight + ":'420'"
        )]
        A3,

        [ExtraProperties(
            PageMediaSizeHelper.ExPropPageMediaSizeName + ":'ISOA4', " +
            PageMediaSizeHelper.ExPropWidth + ":'210', " +
            PageMediaSizeHelper.ExPropHeight + ":'297'"
        )]
        A4,

        [ExtraProperties(
            PageMediaSizeHelper.ExPropPageMediaSizeName + ":'ISOA5', " +
            PageMediaSizeHelper.ExPropWidth + ":'148', " +
            PageMediaSizeHelper.ExPropHeight + ":'210'"
        )]
        A5,

        [ExtraProperties(
            PageMediaSizeHelper.ExPropPageMediaSizeName + ":'JISB4', " +
            PageMediaSizeHelper.ExPropWidth + ":'257', " +
            PageMediaSizeHelper.ExPropHeight + ":'364'"
        )]
        B4,

        [ExtraProperties(
            PageMediaSizeHelper.ExPropPageMediaSizeName + ":'JISB5', " +
            PageMediaSizeHelper.ExPropWidth + ":'182', " +
            PageMediaSizeHelper.ExPropHeight + ":'257'"
        )]
        B5,
    }

    public static class PageMediaSizeHelper {

        public const string ExPropPageMediaSizeName = "PageMediaSizeName";
        public const string ExPropWidth = "Width";
        public const string ExPropHeight = "Height";

        public static PageMediaSize Default {
            get { return PageMediaSize.A4; }
        }

        public static PageMediaSize[] Values {
            get { return _PageMediaSizeHelper.Values; }
        }

        public static System.Printing.PageMediaSizeName ToSystemPageMediaSizeName(this PageMediaSize pageMediaSize) {
            return pageMediaSize.GetExtraProperty(ExPropPageMediaSizeName)
                .TryParse<System.Printing.PageMediaSizeName>(System.Printing.PageMediaSizeName.ISOA4);
        }

        public static double ToWidth(this PageMediaSize pageMediaSize) {
            return pageMediaSize.GetExtraProperty(ExPropWidth)
                .TryParse<double>() * XpsPrinter.MmToPixel;
        }

        public static double ToHeight(this PageMediaSize pageMediaSize) {
            return pageMediaSize.GetExtraProperty(ExPropHeight)
                .TryParse<double>() * XpsPrinter.MmToPixel;
        }

        public static System.Printing.PageMediaSize ToSystemPageMediaSize(
            this PageMediaSize pageMediaSize,
            PageOrientation orientation
        ) {
            return new System.Printing.PageMediaSize(
                pageMediaSize.ToSystemPageMediaSizeName(),
                (orientation == PageOrientation.Portrait ?
                    pageMediaSize.ToWidth() :
                    pageMediaSize.ToHeight()),
                (orientation == PageOrientation.Portrait ?
                    pageMediaSize.ToHeight() :
                    pageMediaSize.ToWidth())
            );
        }
    }
}
