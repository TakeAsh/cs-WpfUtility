using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Data;
using TakeAshUtility;

namespace WpfUtility {

    /// <summary>
    /// Method To ValueConverter
    /// </summary>
    /// <remarks>
    /// [.net - Bind to a method in WPF? - Stack Overflow](http://stackoverflow.com/questions/502250/)
    /// </remarks>
    public sealed class MethodToValueConverter :
        IValueConverter {

        private struct MethodKeys {
            public Type Type;
            public string Name;
        }

        private static Dictionary<MethodKeys, MethodInfo> _methodInfos = new Dictionary<MethodKeys, MethodInfo>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var methodName = parameter as string;
            if (value == null || String.IsNullOrEmpty(methodName)) {
                return value;
            }

            var valueType = value.GetType();
            var key = new MethodKeys() {
                Type = valueType,
                Name = methodName,
            };
            if (!_methodInfos.ContainsKey(key)) {
                _methodInfos[key] = valueType.GetMethod(methodName, Type.EmptyTypes) ??
                    valueType.GetExtensionMethod(methodName) ??
                    valueType.BaseType.GetExtensionMethod(methodName);
            }
            var methodInfo = _methodInfos[key];
            if (methodInfo == null ||
                !targetType.IsAssignableFrom(methodInfo.ReturnType)) {
                return value;
            }
            if (!methodInfo.IsDefined(typeof(ExtensionAttribute), false)) {
                var normalParams = methodInfo.GetParameters()
                    .Select(param => param.DefaultValue)
                    .ToArray();
                return methodInfo.Invoke(value, normalParams);
            }
            var paramType = methodInfo.GetParameters().First().ParameterType;
            if (paramType.IsGenericParameter) {
                var extensionTypeParams = methodInfo.GetGenericArguments();
                extensionTypeParams[0] = valueType;
                methodInfo = methodInfo.MakeGenericMethod(extensionTypeParams);
            }
            var extensionParams = methodInfo.GetParameters()
                .Select(param => param.DefaultValue)
                .ToArray();
            extensionParams[0] = value;
            return methodInfo.Invoke(null, extensionParams);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }
    }
}
