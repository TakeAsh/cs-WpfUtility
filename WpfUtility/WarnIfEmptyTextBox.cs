using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TakeAshUtility;

namespace WpfUtility {

    /// <summary>
    /// Draw Background with WarnBrush, if Text is empty or null.
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/27405927/">c# - WPF: Transfer attached properties from one element to another? - Stack Overflow</see>
    public class WarnIfEmptyTextBox : TextBox {

        private bool _isInitialized = false;
        private Border _border = new Border();
        private Brush _originalBackground = null;
        private Brush _placeholderBrush = null;
        private string _placeholder = null;
        public Brush WarnBrush { get; set; } = Brushes.LightPink;
        public string Placeholder {
            get { return _placeholder; }
            set {
                _placeholder = value;
                var visual = new TextBlock() {
                    Text = _placeholder,
                    Padding = new Thickness(5, 1, 1, 1),
                    Margin = new Thickness(10),
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                _placeholderBrush = new VisualBrush(visual) {
                    Stretch = Stretch.None,
                    TileMode = TileMode.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Center,
                };
            }
        }

        public WarnIfEmptyTextBox() : base() {
            BindProperties();
            this.Loaded += OnLoaded;
        }

        private void BindProperties() {
            new List<DependencyProperty>() {
                WidthProperty, MinWidthProperty, MaxWidthProperty,
                HeightProperty, MinHeightProperty, MaxHeightProperty,
            }.ForEach(p => _border.SetBinding(p, new Binding(p.ToString()) { Source = this, }));
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
            var textBox = sender as TextBox;
            var panel = textBox?.Parent as Panel;
            var contentControl = textBox?.Parent as ContentControl;
            var decorator = textBox?.Parent as Decorator;
            if (textBox == null
                || (panel == null && contentControl == null && decorator == null)
                || _isInitialized) { return; }
            _isInitialized = true;
            if (panel != null) {
                var index = panel.Children.IndexOf(textBox);
                panel.Children.Remove(textBox);
                panel.Children.Insert(index, _border);
            } else if (contentControl != null) {
                contentControl.Content = _border;
            } else if (decorator != null) {
                decorator.Child = _border;
            }
            _border.Child = textBox;
            TypeDescriptor.GetProperties(this)
                .Cast<PropertyDescriptor>()
                .ForEach(pd => {
                    var dpd = DependencyPropertyDescriptor.FromProperty(pd);
                    if (dpd == null || !dpd.IsAttached || dpd.IsReadOnly) { return; }
                    dpd.SetValue(_border, dpd.GetValue(this));
                });
            _originalBackground = textBox.Background;
            textBox.TextChanged += OnTextChanged;
            OnTextChanged(textBox);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e = null) {
            var textBox = sender as TextBox;
            if (textBox == null) { return; }
            if (String.IsNullOrEmpty(textBox.Text)) {
                _border.Background = WarnBrush;
                textBox.Background = _placeholderBrush;
                textBox.ToolTip = _placeholder;
            } else {
                _border.Background = null;
                textBox.Background = _originalBackground;
                textBox.ToolTip = $"{_placeholder}:\n{textBox.Text}";
            }
        }
    }
}
