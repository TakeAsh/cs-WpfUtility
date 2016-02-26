using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TakeAsh;
using TakeAshUtility;

namespace WpfUtility {

    [TypeConverter(typeof(EnumTypeConverter<PrintStatus>))]
    public enum PrintStatus {
        Ready,
        Printing,
    }
}
