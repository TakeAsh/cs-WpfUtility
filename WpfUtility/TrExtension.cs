using System;
using System.Resources;
using System.Windows.Markup;
using System.Xaml;

namespace WpfUtility {

    /// <summary>
    /// Translation Extension for XAML Markup
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>[XAML のマークアップ拡張を使った WPF での国際化 | プログラマーズ雑記帳](http://yohshiy.blog.fc2.com/blog-entry-242.html)</item>
    /// <item>[Internationalization in .NET and WPF - Lorenz Cuno Klopfenstein - Klopfenstein.net](http://www.klopfenstein.net/lorenz.aspx/internationalization-in-net-and-wpf-presentation-foundation)</item>
    /// <item>[Accessing "current class" from WPF custom MarkupExtension - Stack Overflow](http://stackoverflow.com/questions/3047448)</item>
    /// </list>
    /// </remarks>
    [MarkupExtensionReturnType(typeof(string))]
    public class TrExtension :
        MarkupExtension {

        const string NotFoundError = "#NotFound#";

        string _key;

        public TrExtension(string key) {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (String.IsNullOrEmpty(_key)) {
                return NotFoundError;
            }
            ResourceManager resourceManager;
            var rootObjectProvider = GetService<IRootObjectProvider>(serviceProvider);
            if (rootObjectProvider != null &&
                (resourceManager = ResourceHelper.GetResourceManager(rootObjectProvider.RootObject)) != null) {
                return resourceManager.GetString(_key) ?? _key;
            }
            return _key;
        }

        private static T GetService<T>(IServiceProvider serviceProvider) where T : class {
            return serviceProvider.GetService(typeof(T)) as T;
        }
    }
}