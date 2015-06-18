﻿using System;
using System.Linq;
using System.Windows;
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

        public const string AssemblyKey = "TrExtension_Assembly";
        const string NotFoundError = "#NotFound#";

        string _key;

        public string Assembly { get; set; }

        public TrExtension(string key) {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (String.IsNullOrEmpty(_key)) {
                return NotFoundError;
            }
            IRootObjectProvider rootObjectProvider;
            var resourceManager = ResourceHelper.GetResourceManager(
                !String.IsNullOrEmpty(this.Assembly) ?
                    this.Assembly :
                    (Application.Current.TryFindResource(AssemblyKey) as string)
            ) ?? ResourceHelper.GetResourceManager(
                (rootObjectProvider = serviceProvider.GetService<IRootObjectProvider>()) != null ?
                    rootObjectProvider.RootObject :
                    null
            );
            return resourceManager != null ?
                (resourceManager.GetString(_key) ?? _key) :
                _key;
        }
    }

    public static class IServiceProviderExtensionMethods {

        public static T GetService<T>(this IServiceProvider serviceProvider)
            where T : class {

            return serviceProvider != null ?
                serviceProvider.GetService(typeof(T)) as T :
                null;
        }
    }
}
