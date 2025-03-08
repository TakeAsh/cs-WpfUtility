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

        private static readonly DependencyProperty IsInitializedProperty = DependencyProperty.RegisterAttached(
            "IsInitialized",
            typeof(bool),
            typeof(Placeholder)
        );

        private static readonly DependencyProperty OriginalBackgroundProperty = DependencyProperty.RegisterAttached(
            "OriginalBackground",
            typeof(Brush),
            typeof(Placeholder)
        );

        private static readonly DependencyProperty PlaceholderBrushProperty = DependencyProperty.RegisterAttached(
            "PlaceholderBrush",
            typeof(Brush),
            typeof(Placeholder)
        );

        private static void OnPlaceHolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var placeHolder = e.NewValue as string;
            var textChangedHandler = CreateTextChangedEventHandler();
            var selectionChangedHandler = CreateSelectionChangedEventHandler();
            var ribbonGallerySelectionChangedHandler = CreateRibbonGallerySelectionChangedEventHandler();
            var textBox = sender as TextBox;
            var comboBox = sender as ComboBox;
            var ribbonComboBox = sender as RibbonComboBox;
            if (textBox != null) {
                if (!GetIsInitialized(textBox)) {
                    SetIsInitialized(textBox, true);
                    SetOriginalBackground(textBox, textBox.Background);
                }
                SetPlaceholderBrush(textBox, CreateVisualBrush(placeHolder));
                if (String.IsNullOrEmpty(placeHolder)) {
                    textBox.TextChanged -= textChangedHandler;
                } else {
                    textBox.TextChanged += textChangedHandler;
                }
                DrawPlaceHolder(textBox, textBox.Text);
            } else if (comboBox != null) {
                if (!GetIsInitialized(comboBox)) {
                    SetIsInitialized(comboBox, true);
                    SetOriginalBackground(comboBox, comboBox.Background);
                }
                SetPlaceholderBrush(comboBox, CreateVisualBrush(placeHolder));
                if (String.IsNullOrEmpty(placeHolder)) {
                    comboBox.RemoveHandler(TextBox.TextChangedEvent, textChangedHandler);
                    comboBox.SelectionChanged -= selectionChangedHandler;
                } else {
                    comboBox.AddHandler(TextBox.TextChangedEvent, textChangedHandler);
                    comboBox.SelectionChanged += selectionChangedHandler;
                }
                DrawPlaceHolder(comboBox, comboBox.Text);
            } else if (ribbonComboBox != null) {
                if (!GetIsInitialized(ribbonComboBox)) {
                    SetIsInitialized(ribbonComboBox, true);
                    SetOriginalBackground(ribbonComboBox, ribbonComboBox.Background);
                }
                SetPlaceholderBrush(ribbonComboBox, CreateVisualBrush(placeHolder));
                if (String.IsNullOrEmpty(placeHolder)) {
                    ribbonComboBox.RemoveHandler(TextBox.TextChangedEvent, textChangedHandler);
                    ribbonComboBox.RemoveHandler(RibbonGallery.SelectionChangedEvent, ribbonGallerySelectionChangedHandler);
                } else {
                    ribbonComboBox.AddHandler(TextBox.TextChangedEvent, textChangedHandler);
                    ribbonComboBox.AddHandler(RibbonGallery.SelectionChangedEvent, ribbonGallerySelectionChangedHandler);
                }
                DrawPlaceHolder(ribbonComboBox, ribbonComboBox.Text);
            }
        }

        /// <summary>
        /// Returns TextChangedEventHandler that show the place holder only when Control.Text is null or empty
        /// </summary>
        /// <param name="placeHolder">Text that is shown as the place holder</param>
        /// <returns>TextChanged Event Handler</returns>
        private static TextChangedEventHandler CreateTextChangedEventHandler() {
            return (sender, e) => {
                var textBox = sender as TextBox;
                var comboBox = sender as ComboBox;
                var ribbonComboBox = sender as RibbonComboBox;
                if (textBox != null) {
                    DrawPlaceHolder(textBox, textBox.Text);
                } else if (comboBox != null) {
                    DrawPlaceHolder(comboBox, comboBox.Text);
                } else if (ribbonComboBox != null) {
                    DrawPlaceHolder(ribbonComboBox, ribbonComboBox.Text);
                }
            };
        }

        private static SelectionChangedEventHandler CreateSelectionChangedEventHandler() {
            return (sender, e) => {
                var comboBox = sender as ComboBox;
                var comboBoxItem = comboBox.SelectedItem as ComboBoxItem;
                if (comboBox == null) { return; }
                var text =
                    comboBox.IsEditable && !String.IsNullOrEmpty(comboBox.Text) ? comboBox.Text :
                    comboBoxItem != null ? comboBoxItem.Content.SafeToString() :
                    comboBox.SelectedItem.SafeToString();
                DrawPlaceHolder(comboBox, text);
            };
        }

        private static RoutedPropertyChangedEventHandler<object> CreateRibbonGallerySelectionChangedEventHandler() {
            return (sender, e) => {
                Mouse.Capture(null);
                var comboBox = sender as RibbonComboBox;
                if (comboBox == null) { return; }
                DrawPlaceHolder(comboBox, comboBox.Text);
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
        private static void DrawPlaceHolder(UIElement element, string text) {
            var background = element.GetType().GetProperty("Background");
            var toolTip = element.GetType().GetProperty("ToolTip");
            if (background == null || toolTip == null) { return; }
            if (String.IsNullOrEmpty(text)) {
                background.SetValue(element, GetPlaceholderBrush(element), null);
                toolTip.SetValue(element, GetText(element), null);
            } else {
                background.SetValue(element, GetOriginalBackground(element), null);
                toolTip.SetValue(element, GetText(element) + ":\n" + text, null);
            }
        }

        private static VisualBrush CreateVisualBrush(string placeHolder) {
            var visual = new TextBlock() {
                Text = placeHolder,
                Padding = new Thickness(5, 1, 1, 1),
                Margin = new Thickness(10),
                Foreground = Brushes.Gray,
                //Background = Brushes.White,
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

        private static void SetIsInitialized(UIElement element, bool initialized) {
            element.SetValue(IsInitializedProperty, initialized);
        }

        private static bool GetIsInitialized(UIElement element) {
            return (bool)element.GetValue(IsInitializedProperty);
        }

        private static void SetOriginalBackground(UIElement element, Brush brush) {
            element.SetValue(OriginalBackgroundProperty, brush);
        }

        private static Brush GetOriginalBackground(UIElement element) {
            return element.GetValue(OriginalBackgroundProperty) as Brush;
        }

        private static void SetPlaceholderBrush(UIElement element, Brush brush) {
            element.SetValue(PlaceholderBrushProperty, brush);
        }

        private static Brush GetPlaceholderBrush(UIElement element) {
            return element.GetValue(PlaceholderBrushProperty) as Brush;
        }
    }
}
