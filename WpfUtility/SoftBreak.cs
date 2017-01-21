using System;
using System.Windows;
using System.Windows.Controls.Ribbon;

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
            var text = e.NewValue as string;
            var handler = CreateEventHandler(text);
            var ribbonButton = sender as RibbonButton;
            if (ribbonButton != null) {
                if (!String.IsNullOrEmpty(text)) {
                    ribbonButton.SizeChanged += handler;
                } else {
                    ribbonButton.SizeChanged -= handler;
                }
                return;
            }
            var ribbonToggleButton = sender as RibbonToggleButton;
            if (ribbonToggleButton != null) {
                if (!String.IsNullOrEmpty(text)) {
                    ribbonToggleButton.SizeChanged += handler;
                } else {
                    ribbonToggleButton.SizeChanged -= handler;
                }
                return;
            }
        }

        private static SizeChangedEventHandler CreateEventHandler(string text) {
            return (sender, e) => {
                if (String.IsNullOrEmpty(text)) {
                    return;
                }
                var ribbonButton = sender as RibbonButton;
                if (ribbonButton != null) {
                    ribbonButton.Label = ReplaceTags(
                        text,
                        ribbonButton.ControlSizeDefinition.ImageSize == RibbonImageSize.Large
                    );
                    return;
                }
                var ribbonToggleButton = sender as RibbonToggleButton;
                if (ribbonToggleButton != null) {
                    ribbonToggleButton.Label = ReplaceTags(
                        text,
                        ribbonToggleButton.ControlSizeDefinition.ImageSize == RibbonImageSize.Large
                    );
                    return;
                }
            };
        }

        private static string ReplaceTags(string text, bool isLarge) {
            return text
                .Replace(WordBreakTag, isLarge ? " " : "")
                .Replace(SoftHyphenTag, isLarge ? "- " : "");
        }

        public static void SetLabel(UIElement element, string text) {
            element.SetValue(LabelProperty, text);
        }

        public static string GetLabel(UIElement element) {
            return element.GetValue(LabelProperty) as string;
        }
    }
}
