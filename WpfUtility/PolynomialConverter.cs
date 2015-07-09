using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using TakeAshUtility;

namespace WpfUtility {

    public class PolynomialConverter :
        IValueConverter {

        /// <summary>
        /// Coefficients of Polynomial: a0, a1, a2, a3, ... in a0 + a1 * x + a2 * x^2 + a3 * x^3 + ...
        /// </summary>
        /// <remarks>
        /// [wpf - How do you change a bound value, reverse it, multiply it, subtract from it or add to it? - Stack Overflow](http://stackoverflow.com/questions/4969600)
        /// </remarks>
        public List<double> Coefficients { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is double)) {
                return 0;
            }
            var coefficients = parameter is List<double> ?
                parameter as List<double> :
                (parameter is string ?
                    (parameter as string).SplitTrim()
                        .Select(item => item.TryParse<double>())
                        .ToList() :
                    Coefficients);
            if (coefficients == null) {
                return 0;
            }
            var x0 = System.Convert.ToDouble(value);
            var x = 1.0;
            var output = 0.0;
            coefficients.ForEach(a => {
                output += a * x;
                x *= x0;
            });
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
