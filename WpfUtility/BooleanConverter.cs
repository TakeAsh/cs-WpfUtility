﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WpfUtility {

    /// <summary>
    /// The converter from the Boolean to the value of the T type.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    /// <remarks>
    /// [.net - How do I invert BooleanToVisibilityConverter? - Stack Overflow](http://stackoverflow.com/questions/534575/)
    /// </remarks>
    public class BooleanConverter<T> :
        IValueConverter {

        public BooleanConverter(T trueValue, T falseValue) {
            True = trueValue;
            False = falseValue;
        }

        public T True { get; set; }
        public T False { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is bool && ((bool)value) ?
                True :
                False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }
}
