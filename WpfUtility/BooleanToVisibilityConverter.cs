using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfUtility {

    /// <summary>
    /// Boolean To Visibility Converter
    /// </summary>
    public sealed class BooleanToVisibilityConverter :
        BooleanConverter<Visibility> {

        public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) { }
    }

    /// <summary>
    /// Inverted Boolean To Visibility Converter
    /// </summary>
    public sealed class NotBooleanToVisibilityConverter :
        BooleanConverter<Visibility> {

        public NotBooleanToVisibilityConverter() : base(Visibility.Collapsed, Visibility.Visible) { }
    }
}
