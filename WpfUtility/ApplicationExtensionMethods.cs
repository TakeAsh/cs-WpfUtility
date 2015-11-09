using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TakeAshUtility;

namespace WpfUtility {

    public static class ApplicationExtensionMethods {

        public static T SafeTryFindResource<T>(this Application app, object key, T defaultValue = default(T)) {
            return app == null ?
                defaultValue :
                app.TryFindResource(key)
                    .SafeToObject(defaultValue);
        }
    }
}
