using System;
using System.Windows;
using Microsoft.Windows.Controls.Ribbon;

namespace WpfUtility {
    
    public static class SoftBreak {

        public const string WordBreakTag = "[[WBR]]";
        public const string SoftHyphenTag = "[[SHY]]";

        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached(
            "Label",
            typeof(string),
            typeof(SoftBreak),
            new PropertyMetadata(null, OnLabelChanged)
        );

        private static void OnLabelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var button = sender as RibbonButton;
            if (button == null) {
                return;
            }
            var text = e.NewValue as string;
            var handler = CreateEventHandler(text);
            if (!String.IsNullOrEmpty(text)) {
                button.SizeChanged += handler;
            } else {
                button.SizeChanged -= handler;
            }
        }

        private static SizeChangedEventHandler CreateEventHandler(string text) {
            return (sender, e) => {
                var button = sender as RibbonButton;
                if (button == null || String.IsNullOrEmpty(text)) {
                    return;
                }
                var isLarge = button.ControlSizeDefinition.ImageSize == RibbonImageSize.Large;
                button.Label = text
                    .Replace(WordBreakTag, isLarge ? " " : "")
                    .Replace(SoftHyphenTag, isLarge ? "- " : "");
            };
        }

        public static void SetLabel(UIElement element, string text) {
            element.SetValue(LabelProperty, text);
        }

        public static string GetLabel(UIElement element) {
            return element.GetValue(LabelProperty) as string;
        }
    }
}
