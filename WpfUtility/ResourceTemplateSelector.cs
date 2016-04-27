using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfUtility {

    /// <summary>
    ///  Choose a DataTemplate from the resources based on the class name of data object.
    /// </summary>
    /// <typeparam name="T">The type of selected DataTemplate</typeparam>
    public class ResourceTemplateSelector<T> :
        DataTemplateSelector
        where T : DataTemplate {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="converter">The converter that convert the class name to the resource template name.</param>
        public ResourceTemplateSelector(Func<string, string> converter = null) {
            _converter = converter ?? new Func<string, string>(name => name);
        }

        private Func<string, string> _converter;

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            var element = container as FrameworkElement;
            if (element == null) {
                return base.SelectTemplate(item, container);
            }
            var name = item == null ?
                null :
                item.GetType().Name;
            var template = element.TryFindResource(_converter(name)) as T;
            return template == null ?
                base.SelectTemplate(item, container) :
                template;
        }
    }
}
