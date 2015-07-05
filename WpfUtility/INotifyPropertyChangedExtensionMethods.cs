using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace WpfUtility {

    /// <summary>
    /// implement PropertyChangedEventHandler caller
    /// </summary>
    /// <remarks>
    /// [How to get a delegate object from an EventInfo? - Stack Overflow](http://stackoverflow.com/questions/3783267)
    /// </remarks>
    public static class INotifyPropertyChangedExtensionMethods {

        const string PropertyChangedHandlerName = "PropertyChanged";

        public static void NotifyPropertyChanged(this INotifyPropertyChanged sender, string propertyName = "") { // [CallerMemberName]
            if (String.IsNullOrEmpty(propertyName)) {
                return;
            }
            sender.NotifyPropertyChanged(new[] { propertyName });
        }

        public static void NotifyPropertyChanged(this INotifyPropertyChanged sender, IEnumerable<string> propertyNames) {
            if (propertyNames == null) {
                return;
            }
            var handler = sender.GetDelegate(PropertyChangedHandlerName)
                .GetHandler<PropertyChangedEventHandler>();
            if (handler == null) {
                return;
            }
            propertyNames.Where(name => !String.IsNullOrEmpty(name))
                .ToList()
                .ForEach(name => handler(sender, new PropertyChangedEventArgs(name)));
        }

        public static Delegate GetDelegate(this Object obj, string eventHandlerName) {
            return obj == null || String.IsNullOrEmpty(eventHandlerName) ?
                null :
                obj.GetType()
                    .GetField(
                        eventHandlerName,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField
                    ).GetValue(obj) as Delegate;
        }

        public static T GetHandler<T>(this Delegate delgate)
            where T : class {

            return delgate == null ?
                null :
                delgate.GetInvocationList()
                    .Select(del => del as T)
                    .Where(handler => handler != null)
                    .FirstOrDefault();
        }
    }
}
