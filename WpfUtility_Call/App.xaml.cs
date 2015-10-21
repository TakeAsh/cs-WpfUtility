using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using WpfUtility;

namespace WpfUtility_Call {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        private static Properties.Settings _settings = WpfUtility_Call.Properties.Settings.Default;

        static App() {
            CultureManager.SetCulture(_settings.Culture);
        }
    }
}
