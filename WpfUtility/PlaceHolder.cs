using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Media;
using TakeAshUtility;

namespace WpfUtility {

    /// <summary>
    /// Behavior to show the place holder
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Suppoerted Controls: TextBox, ComboBox</item>
    /// <item>[TextBox でプレースホルダーを表示する方法 - present](http://tnakamura.hatenablog.com/entry/20100929/textbox_placeholder)</item>
    /// </list>
    /// </remarks>
    public static class Placeholder {

        /// <summary>
        /// Text property to be shown as the place holder
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(Placeholder),
            new PropertyMetadata(null, OnPlaceHolderChanged)
        );

        private static void OnPlaceHolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var placeHolder = e.NewValue as string;
            var textChangedHandler = CreateTextChangedEventHandler(placeHolder);
            var selectionChangedHandler = CreateSelectionChangedEventHandler(placeHolder);
            var ribbonGallerySelectionChangedHandler = CreateRibbonGallerySelectionChangedEventHandler(placeHolder);
            var textBox = sender as TextBox;
            var comboBox = sender as ComboBox;
            var ribbonComboBox = sender as RibbonComboBox;
            if (textBox != null) {
                if (String.IsNullOrEmpty(placeHolder)) {
                    textBox.TextChanged -= textChangedHandler;
                } else {
                    textBox.TextChanged += textChangedHandler;
                }
                DrawPlaceHolder(textBox, placeHolder, textBox.Text);
            } else if (comboBox != null) {
                if (String.IsNullOrEmpty(placeHolder)) {
                    comboBox.RemoveHandler(TextBox.TextChangedEvent, textChangedHandler);
                    comboBox.SelectionChanged -= selectionChangedHandler;
                } else {
                    comboBox.AddHandler(TextBox.TextChangedEvent, textChangedHandler);
                    comboBox.SelectionChanged += selectionChangedHandler;
                }
                DrawPlaceHolder(comboBox, placeHolder, comboBox.Text);
            } else if (ribbonComboBox != null) {
                if (String.IsNullOrEmpty(placeHolder)) {
                    ribbonComboBox.RemoveHandler(TextBox.TextChangedEvent, textChangedHandler);
                    ribbonComboBox.RemoveHandler(RibbonGallery.SelectionChangedEvent, ribbonGallerySelectionChangedHandler);
                } else {
                    ribbonComboBox.AddHandler(TextBox.TextChangedEvent, textChangedHandler);
                    ribbonComboBox.AddHandler(RibbonGallery.SelectionChangedEvent, ribbonGallerySelectionChangedHandler);
                }
                DrawPlaceHolder(ribbonComboBox, placeHolder, ribbonComboBox.Text);
            }
        }

        /// <summary>
        /// Returns TextChangedEventHandler that show the place holder only when Control.Text is null or empty
        /// </summary>
        /// <param name="placeHolder">Text that is shown as the place holder</param>
        /// <returns>TextChanged Event Handler</returns>
        private static TextChangedEventHandler CreateTextChangedEventHandler(string placeHolder) {
            return (sender, e) => {
                var textBox = sender as TextBox;
                var comboBox = sender as ComboBox;
                var ribbonComboBox = sender as RibbonComboBox;
                if (textBox != null) {
                    DrawPlaceHolder(textBox, placeHolder, textBox.Text);
                } else if (comboBox != null) {
                    DrawPlaceHolder(comboBox, placeHolder, comboBox.Text);
                } else if (ribbonComboBox != null) {
                    DrawPlaceHolder(ribbonComboBox, placeHolder, ribbonComboBox.Text);
                }
            };
        }

        private static SelectionChangedEventHandler CreateSelectionChangedEventHandler(string placeHolder) {
            return (sender, e) => {
                var comboBox = sender as ComboBox;
                var comboBoxItem = comboBox.SelectedItem as ComboBoxItem;
                if (comboBox == null) {
                    return;
                }
                var text = comboBox.IsEditable && !String.IsNullOrEmpty(comboBox.Text) ?
                    comboBox.Text :
                    (comboBoxItem != null ?
                        comboBoxItem.Content.SafeToString() :
                        comboBox.SelectedItem.SafeToString());
                DrawPlaceHolder(comboBox, placeHolder, text);
            };
        }

        private static RoutedPropertyChangedEventHandler<object> CreateRibbonGallerySelectionChangedEventHandler(string placeHolder) {
            return (sender, e) => {
                Mouse.Capture(null);
                var comboBox = sender as RibbonComboBox;
                if (comboBox == null) {
                    return;
                }
                DrawPlaceHolder(comboBox, placeHolder, comboBox.Text);
            };
        }

        /// <summary>
        /// Draw place holder of the element
        /// </summary>
        /// <param name="element">target element</param>
        /// <param name="placeHolder">palce holder</param>
        /// <param name="text">text value of the element</param>
        /// <remarks>
        /// [c# - Getting the Background of a UIElement or sender - Stack Overflow](http://stackoverflow.com/questions/16224920)
        /// </remarks>
        private static void DrawPlaceHolder(UIElement element, string placeHolder, string text) {
            var background = element.GetType().GetProperty("Background");
            var toolTip = element.GetType().GetProperty("ToolTip");
            if (background == null || toolTip == null) {
                return;
            }
            if (String.IsNullOrEmpty(text)) {
                background.SetValue(element, CreateVisualBrush(placeHolder), null);
                toolTip.SetValue(element, placeHolder, null);
            } else {
                background.SetValue(element, new SolidColorBrush(Colors.White), null);
                toolTip.SetValue(element, placeHolder + ":\n" + text, null);
            }
        }

        private static VisualBrush CreateVisualBrush(string placeHolder) {
            var visual = new TextBlock() {
                Text = placeHolder,
                Padding = new Thickness(5, 1, 1, 1),
                Margin = new Thickness(10),
                Foreground = new SolidColorBrush(Colors.Gray),
                //Background = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            return new VisualBrush(visual) {
                Stretch = Stretch.None,
                TileMode = TileMode.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Center,
            };
        }

        public static void SetText(UIElement element, string placeHolder) {
            element.SetValue(TextProperty, placeHolder);
        }

        public static string GetText(UIElement element) {
            return element.GetValue(TextProperty) as string;
        }
    }
}
